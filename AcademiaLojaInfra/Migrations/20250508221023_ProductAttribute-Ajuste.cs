using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLoja.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ProductAttributeAjuste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Accessory",
                table: "ProductAttributes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Brand",
                table: "ProductAttributes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "ProductAttributes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Flavor",
                table: "ProductAttributes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Objective",
                table: "ProductAttributes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accessory",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "Flavor",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "Objective",
                table: "ProductAttributes");
        }
    }
}
