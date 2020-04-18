using Microsoft.EntityFrameworkCore.Migrations;

namespace ModelLayer.Migrations
{
    public partial class manyToManyOrganizationContacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Organizations_OrganizationId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_OrganizationId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Contacts");

            migrationBuilder.CreateTable(
                name: "OrganizationContacts",
                columns: table => new
                {
                    OrganizationId = table.Column<long>(nullable: false),
                    ContactId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationContacts", x => new { x.OrganizationId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_OrganizationContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationContacts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ConcurrencyStamp",
                value: "b0b4ca58-9b98-4c83-add4-622dd8cbca93");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationContacts_ContactId",
                table: "OrganizationContacts",
                column: "ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationContacts");

            migrationBuilder.AddColumn<long>(
                name: "OrganizationId",
                table: "Contacts",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ConcurrencyStamp",
                value: "6783c552-abc1-41d7-a693-02ecddfef8e3");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_OrganizationId",
                table: "Contacts",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Organizations_OrganizationId",
                table: "Contacts",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
