using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_CommerceWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyProductCategoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CategoryId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 6,
                column: "CategoryId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 7,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 8,
                column: "CategoryId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 9,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "Id",
                keyValue: 10,
                column: "CategoryId",
                value: 4);

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId",
                table: "products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_CategoryId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "products");
        }
    }
}
