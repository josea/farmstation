using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmStationDb.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "Notifications",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "now()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Notifications");
        }
    }
}
