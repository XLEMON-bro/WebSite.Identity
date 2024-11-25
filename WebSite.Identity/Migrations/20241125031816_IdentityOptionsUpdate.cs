using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebSite.Identity.Migrations
{
    /// <inheritdoc />
    public partial class IdentityOptionsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bd3dfb8-86ff-4d26-9e1b-776484e52280");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aa88c523-ca95-4c23-a81d-e68a39dcf614");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1fe60284-03f1-4267-9ec3-c50af380acbf", null, "Admin", "ADMIN" },
                    { "26e6f3ad-9c6e-4beb-b4f9-86a0b200e7bd", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1fe60284-03f1-4267-9ec3-c50af380acbf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26e6f3ad-9c6e-4beb-b4f9-86a0b200e7bd");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bd3dfb8-86ff-4d26-9e1b-776484e52280", null, "User", "USER" },
                    { "aa88c523-ca95-4c23-a81d-e68a39dcf614", null, "Admin", "ADMIN" }
                });
        }
    }
}
