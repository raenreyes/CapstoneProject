﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapstoneProject.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    ProductDescription = table.Column<string>(type: "TEXT", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProductImage = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "OrderHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderTotal = table.Column<double>(type: "REAL", nullable: false),
                    SessionId = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentIntentId = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    StreetAddress = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHeaders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ProductDescription", "ProductImage", "ProductName", "ProductPrice" },
                values: new object[,]
                {
                    { 1, "A deep dive into the C# programming language.", "/images/csharp-depth.jpg", "C# in Depth", 39.99m },
                    { 2, "A guide to writing clean, maintainable, and efficient code.", "/images/clean-code.jpg", "Clean Code", 34.99m },
                    { 3, "Classic book on software craftsmanship and best practices.", "/images/pragmatic-programmer.jpg", "The Pragmatic Programmer", 44.99m },
                    { 4, "A foundational book on design patterns in software development.", "/images/design-patterns.jpg", "Design Patterns: Elements of Reusable Object-Oriented Software", 49.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_ProductId",
                table: "OrderHeaders",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderHeaders");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
