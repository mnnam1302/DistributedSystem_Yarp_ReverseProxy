using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authorization.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdateUseRemovePasswordSalt : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PasswordSalt",
            table: "AppUsers");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PasswordSalt",
            table: "AppUsers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }
}
