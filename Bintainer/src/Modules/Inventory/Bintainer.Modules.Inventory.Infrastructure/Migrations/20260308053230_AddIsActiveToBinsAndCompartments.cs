using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToBinsAndCompartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "inventory",
                table: "compartments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "inventory",
                table: "bins",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "inventory",
                table: "bins");
        }
    }
}
