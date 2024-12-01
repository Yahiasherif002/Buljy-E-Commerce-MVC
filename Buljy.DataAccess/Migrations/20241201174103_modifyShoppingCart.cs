using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_CommerceWeb.Migrations
{
    /// <inheritdoc />
    public partial class modifyShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_UserId",
                table: "shoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_shoppingCarts_UserId",
                table: "shoppingCarts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "shoppingCarts");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "shoppingCarts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCarts_ApplicationUserId",
                table: "shoppingCarts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_ApplicationUserId",
                table: "shoppingCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_ApplicationUserId",
                table: "shoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_shoppingCarts_ApplicationUserId",
                table: "shoppingCarts");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "shoppingCarts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "shoppingCarts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCarts_UserId",
                table: "shoppingCarts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_UserId",
                table: "shoppingCarts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
