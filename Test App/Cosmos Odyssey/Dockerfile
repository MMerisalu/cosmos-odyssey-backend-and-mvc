FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src


# copy csproj and restore as distinct layers
COPY *.sln .

# copy all the projects from host to image
# Base
COPY Base.Contracts.Domain/*.csproj ./Base.Contracts.Domain/
COPY Base.Domain/*.csproj ./Base.Domain/

# App
COPY App.DAL.EF/*.csproj ./App.DAL.EF/
COPY App.Domain/*.csproj ./App.Domain/
COPY WebApp/*.csproj ./WebApp/

RUN dotnet restore

# copy everything else and build app
# Base
COPY Base.Contracts.Domain/. ./Base.Contracts.Domain/
COPY Base.Domain/. ./Base.Domain/

# App
COPY App.DAL.EF/. ./App.DAL.EF/
COPY App.Domain/. ./App.Domain/
COPY WebApp/. ./WebApp/

WORKDIR /src/WebApp
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
EXPOSE 80

# Set the ASPNETCORE_ENVIRONMENT environment variable to Development.
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /src/WebApp/out ./

ENTRYPOINT ["dotnet", "WebApp.dll"]