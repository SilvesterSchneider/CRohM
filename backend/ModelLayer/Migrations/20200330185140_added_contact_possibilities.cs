using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelLayer.Migrations
{
    public partial class added_contact_possibilities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContactId",
                table: "Organizations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactPossibilities",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Faxnumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPossibilities", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ConcurrencyStamp",
                value: "26e010a8-b273-47de-a42c-96f40cf75806");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ContactId",
                table: "Organizations",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_ContactPossibilities_ContactId",
                table: "Organizations",
                column: "ContactId",
                principalTable: "ContactPossibilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_ContactPossibilities_ContactId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "ContactPossibilities");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_ContactId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Organizations");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ConcurrencyStamp",
                value: "aedbf3bf-6104-4ffa-97b6-e7fad2ccc65b");
        }
    }
}
