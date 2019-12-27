using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelLayer.Migrations
{
    public partial class _002IdProblem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Addresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Addresses");
        }
    }
}
