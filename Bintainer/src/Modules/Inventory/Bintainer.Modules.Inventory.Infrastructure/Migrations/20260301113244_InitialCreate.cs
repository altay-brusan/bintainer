using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.CreateTable(
                name: "inventories",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "storage_units",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    columns = table.Column<int>(type: "integer", nullable: false),
                    rows = table.Column<int>(type: "integer", nullable: false),
                    compartment_count = table.Column<int>(type: "integer", nullable: false),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_storage_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_storage_units_inventories_inventory_id",
                        column: x => x.inventory_id,
                        principalSchema: "inventory",
                        principalTable: "inventories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bins",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    column = table.Column<int>(type: "integer", nullable: false),
                    row = table.Column<int>(type: "integer", nullable: false),
                    storage_unit_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bins", x => x.id);
                    table.ForeignKey(
                        name: "fk_bins_storage_units_storage_unit_id",
                        column: x => x.storage_unit_id,
                        principalSchema: "inventory",
                        principalTable: "storage_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "compartments",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    bin_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_compartments", x => x.id);
                    table.ForeignKey(
                        name: "fk_compartments_bins_bin_id",
                        column: x => x.bin_id,
                        principalSchema: "inventory",
                        principalTable: "bins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bins_storage_unit_id",
                schema: "inventory",
                table: "bins",
                column: "storage_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_compartments_bin_id",
                schema: "inventory",
                table: "compartments",
                column: "bin_id");

            migrationBuilder.CreateIndex(
                name: "ix_storage_units_inventory_id",
                schema: "inventory",
                table: "storage_units",
                column: "inventory_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compartments",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "bins",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "storage_units",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "inventories",
                schema: "inventory");
        }
    }
}
