﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapstoneProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketIsComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Tickets");
        }
    }
}
