using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSaltAttributeInAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "AppUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
