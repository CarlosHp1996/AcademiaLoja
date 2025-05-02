using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLoja.Infra.Migrations
{
    /// <inheritdoc />
    public partial class EnumAcessory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Accessories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Model",
                table: "Accessories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Accessories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Accessories");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Accessories");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Accessories");
        }
    }
}
