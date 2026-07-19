using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Albatros.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationAndReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "ApplicationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiration",
                table: "ApplicationUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiration",
                table: "ApplicationUsers");
        }
    }
}
