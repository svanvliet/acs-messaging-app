using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SVV.MessagingApp.Migrations
{
    public partial class AddContactModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    PrimaryNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
