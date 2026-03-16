using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshNFluffy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderRequestOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "OrderRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderRequests_UserId",
                table: "OrderRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRequests_AspNetUsers_UserId",
                table: "OrderRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRequests_AspNetUsers_UserId",
                table: "OrderRequests");

            migrationBuilder.DropIndex(
                name: "IX_OrderRequests_UserId",
                table: "OrderRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OrderRequests");
        }
    }
}
