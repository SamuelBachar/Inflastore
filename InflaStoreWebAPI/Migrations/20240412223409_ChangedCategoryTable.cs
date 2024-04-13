using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InflaStoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company_Id",
                table: "Regions");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Categories",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "Company_Id",
                table: "Regions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
