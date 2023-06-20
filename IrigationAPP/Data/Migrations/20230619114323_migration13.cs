using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IrigationAPP.Data.Migrations
{
    public partial class migration13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IrrigationTime",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    irrigationTime = table.Column<double>(nullable: false),
                    time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationTime", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IrrigationTime");

            // You might want to reconsider what to do in the Down method, as the original code
            // recreated the IrrigationMode table, which you said doesn't exist in your database.
        }
    }
}

