using System.Text.Json;
using System.Text.Json.Serialization;
using App.DAL.EF;
using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using NuGet.Versioning;

namespace WebApp.Helpers;

/*public class RouteInfoConverter : JsonConverter<RouteInfo> 
{
    /// <summary>
    /// Read/Deserialize JSON into RouteInfo objects
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override RouteInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject");
        }

        var routeInfo = new RouteInfo();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                // TODO: this is a nested object, so chances are we don'ˇt want to stop here ;)
                return routeInfo;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propName = (reader.GetString() ?? "").ToLower();
                reader.Read();

                switch (propName)
                {
                    case var _ when propName.Equals(nameof(RouteInfo.Id), StringComparison.OrdinalIgnoreCase):
                        routeInfo.Id = reader.GetGuid();
                        break;
                    case var _ when propName.Equals(nameof(RouteInfo), StringComparison.OrdinalIgnoreCase):
                    {
                        reader.Read(); // close the parent
                        while (reader.Read())
                        {
                            string childPropName = (reader.GetString() ?? "").ToLower();
                            reader.Read();
                            switch (childPropName)
                            {
                                case var _ when propName.Equals(nameof(RouteInfo.Id), StringComparison.OrdinalIgnoreCase):
                                    // Ignore this
                                    break;
                                
                                case var _ when propName.Equals(nameof(RouteInfo.From), StringComparison.OrdinalIgnoreCase):
                                    // This is an object
                                    reader.Read();
                                    reader.
                                    break;
                            }
                        }
                        
                        break;
                    }
                }
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, RouteInfo value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}*/

public static class PriceListHelper
{
    private const string Address = "https://cosmos-odyssey.azurewebsites.net/api/v1.0/TravelPrices";

    public static async Task<PriceList> RefreshPriceList(AppDbContext context, HttpClient client)
    {

        HttpResponseMessage response = await client.GetAsync(Address);
        response.EnsureSuccessStatusCode();


        var priceListDto = await response.Content.ReadAsAsync<PriceListHelper.PriceListDto>();

        // TODO: check if this is already in the database, by first lLoading all the items
        await context.PriceLists.LoadAsync();
        await context.Companies.LoadAsync();

        // TODO: If it is (in the database) then skip
        var existingPriceList = await context.PriceLists.FindAsync(priceListDto.Id);
        if (existingPriceList == null)
        {
            // Insert missing Companies
            foreach (var company in PriceListHelper.ExtractCompanies(priceListDto))
            {
                var existingCompany = await context.Companies.Where(x => x.Name.Equals(company.Name))
                    .FirstOrDefaultAsync();
                if (existingCompany == null)
                    context.Companies.Add(company);
            }

            await context.SaveChangesAsync();
            // Now we can add the PriceList
            var priceList = PriceListHelper.FromDto(priceListDto, context.Companies.ToList());
            context.PriceLists.Add(priceList);
            await context.SaveChangesAsync();

            var oldLists = context.PriceLists
                .OrderByDescending(p => p.ValidUntil)
                .Skip(15);
            await oldLists.ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            return priceList;
        }

        return existingPriceList;
    }

    /// <summary>
    /// Map back from DTO to PriceList
    /// </summary>
    /// <param name="dto">PriceList DTO to parse</param>
    /// <param name="companies">List of previously inserted company entities, see <see cref="ExtractCompanies"/></param>
    /// <returns>PriceList ready to insert to the DbContext is necessary</returns>
    public static PriceList FromDto(PriceListDto dto, List<Company> companies)
    {
        var output = new PriceList
        {
            Id = dto.Id,
            ValidUntil = dto.ValidUntil,
            Legs = dto.Legs!.Select(leg => new RouteInfo
            {
                Id = leg.Id,
                FromId = leg.RouteInfo!.From!.Id,
                From = leg.RouteInfo!.From!.Name!,
                ToId = leg.RouteInfo!.To!.Id,
                To = leg.RouteInfo.To.Name,
                Distance = (int)leg.RouteInfo.Distance,
                Providers = leg.Providers!.Select(provider => new Provider
                {
                    Id = provider.Id,
                    // I deliberately want this to fail ;)
                    // but it shouldn't happen due to previously extracting and inserting the missing company records.
                    CompanyId = companies.First(x => x.Name.Equals(provider.Company!.Name))!.Id,
                    // DO NOT use object reference, will create duplicates
                    //Company = provider.Company,
                    FlightStart = provider.FlightStart,
                    FlightEnd = provider.FlightEnd,
                    Price = provider.Price,
                    TravelTime = provider.FlightEnd.Subtract(provider.FlightStart)
                }).ToList()
            }).ToList()
        };
        return output;
    }

    public static IEnumerable<Company> ExtractCompanies (PriceListDto dto)
    {
        //var output = dto.Legs.SelectMany(leg =>
        //    leg.Providers.SelectMany(provider => provider.Company)
        //).DistinctBy(x => x.Id);
        
        var companies = new List<Company>();
        foreach(var leg in dto.Legs!)
        foreach (var provider in leg.Providers!)
        {
            if(!companies.Any(c => c.Id == provider.Company!.Id))
                companies.Add(provider.Company!);
        }

        return companies;
    }
    public class PriceListDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset ValidUntil { get; set; }
        public ICollection<LegDto>? Legs { get; set; }
    }

    public class LegDto
    {
        public Guid Id { get; set; }
        public RouteInfoDto? RouteInfo { get; set; }
        public ICollection<ProviderDto>? Providers { get; set; }
    }
    
    public class RouteInfoDto
    {
        public Guid Id { get; set; }
        public NamedItemDto? From { get; set; }
        public NamedItemDto? To { get; set; }
        public long Distance { get; set; }
    }
    
    public class NamedItemDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
    
    public class ProviderDto
    {
        public Guid Id { get; set; }
        public Company? Company { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset FlightStart { get; set; }
        public DateTimeOffset FlightEnd { get; set; }
    }
}