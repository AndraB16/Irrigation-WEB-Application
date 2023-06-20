using Microsoft.EntityFrameworkCore.Migrations;

namespace IrigationAPP.Data.Migrations
{
    public partial class initialsetup2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "collectorId",
                table: "DataRead",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "collectorId",
                table: "DataRead");
        }
    }
}
