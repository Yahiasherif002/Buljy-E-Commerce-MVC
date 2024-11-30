using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_CommerceWeb.Migrations
{
    /// <inheritdoc />
    public partial class seedCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "companies",
                columns: new[] { "Id", "Address", "City", "Country", "Email", "Name", "Phone", "PostalCode" },
                values: new object[,]
                {
                    { 1, "123 Main St", "New York", "USA", "", "Company 1", "123-456-7890", null },
                    { 2, "456 Elm St", "Los Angeles", "USA", "", "Company 2", "123-456-7890", null },
                    { 3, "789 Oak St", "Chicago", "USA", "", "Company 3", "123-456-7890", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
