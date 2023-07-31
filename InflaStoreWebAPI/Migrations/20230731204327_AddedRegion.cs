using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InflaStoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Region",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "Users");
        }
    }
}
