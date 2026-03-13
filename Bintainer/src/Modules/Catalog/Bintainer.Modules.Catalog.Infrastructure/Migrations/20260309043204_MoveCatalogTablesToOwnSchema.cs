using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveCatalogTablesToOwnSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.RenameTable(
                name: "footprints",
                schema: "inventory",
                newName: "footprints",
                newSchema: "catalog");

            migrationBuilder.RenameTable(
                name: "components",
                schema: "inventory",
                newName: "components",
                newSchema: "catalog");

            migrationBuilder.RenameTable(
                name: "categories",
                schema: "inventory",
                newName: "categories",
                newSchema: "catalog");

            migrationBuilder.RenameTable(
                name: "bom_imports",
                schema: "inventory",
                newName: "bom_imports",
                newSchema: "catalog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.RenameTable(
                name: "footprints",
                schema: "catalog",
                newName: "footprints",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "components",
                schema: "catalog",
                newName: "components",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "categories",
                schema: "catalog",
                newName: "categories",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "bom_imports",
                schema: "catalog",
                newName: "bom_imports",
                newSchema: "inventory");
        }
    }
}
