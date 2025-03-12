using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Products_ProductId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_ProductId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "OrderHeaders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_IdentityUserId",
                table: "OrderHeaders",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_IdentityUserId",
                table: "OrderHeaders",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_IdentityUserId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_IdentityUserId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_ProductId",
                table: "OrderHeaders",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Products_ProductId",
                table: "OrderHeaders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
