using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IrigationAPP.Data.Migrations
{
    public partial class migration7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTime",
                table: "Schedule");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Schedule",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValveId",
                table: "Schedule",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "ValveId",
                table: "Schedule");

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentTime",
                table: "Schedule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
