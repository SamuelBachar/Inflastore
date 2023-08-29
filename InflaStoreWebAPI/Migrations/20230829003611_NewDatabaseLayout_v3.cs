using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InflaStoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseLayout_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_ItemsPrices_ItemPriceId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_NavigationShopDatas_NavigationShopDataId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemsPrices_ItemPriceId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemPriceId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ItemPriceId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_NavigationShopDataId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ItemPriceId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemPriceId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NavigationShopDataId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Region_Id",
                table: "Companies");

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.AddColumn<int>(
                name: "ItemPriceId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemPriceId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NavigationShopDataId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Region_Id",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemPriceId",
                table: "Items",
                column: "ItemPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ItemPriceId",
                table: "Companies",
                column: "ItemPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_NavigationShopDataId",
                table: "Companies",
                column: "NavigationShopDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_ItemsPrices_ItemPriceId",
                table: "Companies",
                column: "ItemPriceId",
                principalTable: "ItemsPrices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_NavigationShopDatas_NavigationShopDataId",
                table: "Companies",
                column: "NavigationShopDataId",
                principalTable: "NavigationShopDatas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemsPrices_ItemPriceId",
                table: "Items",
                column: "ItemPriceId",
                principalTable: "ItemsPrices",
                principalColumn: "Id");
        }
    }
}
