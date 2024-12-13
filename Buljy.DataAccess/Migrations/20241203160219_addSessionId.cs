using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_CommerceWeb.Migrations
{
    /// <inheritdoc />
    public partial class addSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "orderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "orderHeaders");
        }
    }
}
