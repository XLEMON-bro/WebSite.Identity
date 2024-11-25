using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebSite.Identity.Migrations
{
    /// <inheritdoc />
    public partial class IdentityOptionsUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "263eb96a-1d72-454e-915a-feab73352575", null, "User", "USER" },
                    { "5c90f99f-a6e8-4c73-a356-e54983270029", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "263eb96a-1d72-454e-915a-feab73352575");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c90f99f-a6e8-4c73-a356-e54983270029");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1fe60284-03f1-4267-9ec3-c50af380acbf", null, "Admin", "ADMIN" },
                    { "26e6f3ad-9c6e-4beb-b4f9-86a0b200e7bd", null, "User", "USER" }
                });
        }
    }
}
