using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmStationDb.Migrations
{
    /// <inheritdoc />
    public partial class AddLastFarmingLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Statuses",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFarming",
                table: "Farms",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "LastFarming",
                table: "Farms");
        }
    }
}
