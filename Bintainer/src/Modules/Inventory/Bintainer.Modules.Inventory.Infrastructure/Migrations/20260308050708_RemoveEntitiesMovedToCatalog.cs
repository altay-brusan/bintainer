using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEntitiesMovedToCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tables (components, categories, footprints, bom_imports) are now managed
            // by CatalogDbContext. Only drop FK constraints that InventoryDbContext owned.

            migrationBuilder.DropForeignKey(
                name: "fk_compartments_components_component_id",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.DropForeignKey(
                name: "fk_movements_components_component_id",
                schema: "inventory",
                table: "movements");

            migrationBuilder.DropIndex(
                name: "ix_compartments_component_id",
                schema: "inventory",
                table: "compartments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_compartments_component_id",
                schema: "inventory",
                table: "compartments",
                column: "component_id");

            migrationBuilder.AddForeignKey(
                name: "fk_compartments_components_component_id",
                schema: "inventory",
                table: "compartments",
                column: "component_id",
                principalSchema: "inventory",
                principalTable: "components",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_movements_components_component_id",
                schema: "inventory",
                table: "movements",
                column: "component_id",
                principalSchema: "inventory",
                principalTable: "components",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
