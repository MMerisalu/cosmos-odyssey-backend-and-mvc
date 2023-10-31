using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class AllowedLongFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteInfos_Reservations_ReservationId",
                table: "RouteInfos");

            migrationBuilder.DropIndex(
                name: "IX_RouteInfos_ReservationId",
                table: "RouteInfos");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "RouteInfos");

            // DELETE the existing Reservations because the conversion for the TotalFlightTime will not be valid
            // And this is test data anyway, I do not really care ;P
            migrationBuilder.Sql("DELETE FROM [Reservations]");
            
            migrationBuilder.AlterColumn<string>(
                name: "TotalFlightTime",
                table: "Reservations",
                type: "nvarchar(48)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "TravelTime",
                table: "Providers",
                type: "nvarchar(48)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "FlightRoute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightRoute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightRoute_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlightRoute_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightRoute_ProviderId",
                table: "FlightRoute",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightRoute_ReservationId",
                table: "FlightRoute",
                column: "ReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightRoute");

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationId",
                table: "RouteInfos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TotalFlightTime",
                table: "Reservations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(48)");

            migrationBuilder.AlterColumn<long>(
                name: "TravelTime",
                table: "Providers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(48)");

            migrationBuilder.CreateIndex(
                name: "IX_RouteInfos_ReservationId",
                table: "RouteInfos",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteInfos_Reservations_ReservationId",
                table: "RouteInfos",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }
    }
}
