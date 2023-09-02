using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InflaStoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseLayout_changedCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Companies");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Companies",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
