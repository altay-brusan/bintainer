using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tables (components, categories, footprints, bom_imports) already exist
            // in the inventory schema from InventoryDbContext migrations.
            // This is a baseline migration — no DDL needed.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Do not drop tables on rollback — they were originally created
            // by InventoryDbContext migrations.
        }
    }
}
