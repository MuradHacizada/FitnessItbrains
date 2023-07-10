using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp1.Migrations
{
    public partial class Updatedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Products_ProductId",
                table: "BasketItems");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceLife",
                table: "Packages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceYear",
                table: "Packages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "PacPrice",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "OrderItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackageName",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "BasketItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PackagPrice",
                table: "BasketItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "BasketItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_PackageId",
                table: "OrderItem",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_PackageId",
                table: "BasketItems",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Packages_PackageId",
                table: "BasketItems",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Products_ProductId",
                table: "BasketItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Packages_PackageId",
                table: "OrderItem",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Packages_PackageId",
                table: "BasketItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Products_ProductId",
                table: "BasketItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Packages_PackageId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_PackageId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_PackageId",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "PriceLife",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PriceYear",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PacPrice",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PackageName",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PackagPrice",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "BasketItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OrderItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "BasketItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Products_ProductId",
                table: "BasketItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
