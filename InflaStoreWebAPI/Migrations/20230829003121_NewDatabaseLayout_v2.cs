using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InflaStoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseLayout_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Items_ItemId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_ItemId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Units");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_ItemId",
                table: "Units",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Items_ItemId",
                table: "Units",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }
    }
}
