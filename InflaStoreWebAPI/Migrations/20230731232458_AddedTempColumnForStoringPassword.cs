using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InflaStoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedTempColumnForStoringPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempResetPasswordHashWithSalt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempResetPasswordHashWithSalt",
                table: "Users");
        }
    }
}
