using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelLayer.Migrations
{
    public partial class _004_contacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ContactPossibilities");

            migrationBuilder.DropColumn(
                name: "Faxnumber",
                table: "ContactPossibilities");

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "ContactPossibilities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "ContactPossibilities",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ConcurrencyStamp",
                value: "f2ef23d0-0f8b-4502-98eb-bdf1330bf9cd");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fax",
                table: "ContactPossibilities");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "ContactPossibilities");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ContactPossibilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faxnumber",
                table: "ContactPossibilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ConcurrencyStamp",
                value: "26e010a8-b273-47de-a42c-96f40cf75806");
        }
    }
}
