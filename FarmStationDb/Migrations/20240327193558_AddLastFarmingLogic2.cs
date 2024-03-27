using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmStationDb.Migrations
{
    /// <inheritdoc />
    public partial class AddLastFarmingLogic2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastNotificationFarmingStatus",
                table: "Farms",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastNotificationFarmingStatus",
                table: "Farms");
        }
    }
}
