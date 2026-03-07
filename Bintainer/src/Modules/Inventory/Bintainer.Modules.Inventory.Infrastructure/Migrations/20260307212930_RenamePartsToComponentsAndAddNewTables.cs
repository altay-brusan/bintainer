using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePartsToComponentsAndAddNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename parts table to components (preserving data)
            migrationBuilder.RenameTable(
                name: "parts",
                schema: "inventory",
                newName: "components",
                newSchema: "inventory");

            // Add new columns to components table
            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                schema: "inventory",
                table: "components",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "manufacturer",
                schema: "inventory",
                table: "components",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "low_stock_threshold",
                schema: "inventory",
                table: "components",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Rename FK and indexes on components table
            migrationBuilder.DropForeignKey(
                name: "fk_parts_categories_category_id",
                schema: "inventory",
                table: "components");

            migrationBuilder.DropForeignKey(
                name: "fk_parts_footprints_footprint_id",
                schema: "inventory",
                table: "components");

            migrationBuilder.RenameIndex(
                name: "ix_parts_category_id",
                schema: "inventory",
                table: "components",
                newName: "ix_components_category_id");

            migrationBuilder.RenameIndex(
                name: "ix_parts_footprint_id",
                schema: "inventory",
                table: "components",
                newName: "ix_components_footprint_id");

            migrationBuilder.AddForeignKey(
                name: "fk_components_categories_category_id",
                schema: "inventory",
                table: "components",
                column: "category_id",
                principalSchema: "inventory",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_components_footprints_footprint_id",
                schema: "inventory",
                table: "components",
                column: "footprint_id",
                principalSchema: "inventory",
                principalTable: "footprints",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            // Rename primary key
            migrationBuilder.Sql("ALTER TABLE inventory.components RENAME CONSTRAINT pk_parts TO pk_components;");

            // Rename compartments.part_id → component_id
            migrationBuilder.DropForeignKey(
                name: "fk_compartments_parts_part_id",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.RenameColumn(
                name: "part_id",
                schema: "inventory",
                table: "compartments",
                newName: "component_id");

            migrationBuilder.RenameIndex(
                name: "ix_compartments_part_id",
                schema: "inventory",
                table: "compartments",
                newName: "ix_compartments_component_id");

            migrationBuilder.AddForeignKey(
                name: "fk_compartments_components_component_id",
                schema: "inventory",
                table: "compartments",
                column: "component_id",
                principalSchema: "inventory",
                principalTable: "components",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            // Create new tables
            migrationBuilder.CreateTable(
                name: "bom_imports",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    total_lines = table.Column<int>(type: "integer", nullable: false),
                    matched_count = table.Column<int>(type: "integer", nullable: false),
                    new_count = table.Column<int>(type: "integer", nullable: false),
                    total_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bom_imports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movements",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    component_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    compartment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_compartment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movements", x => x.id);
                    table.ForeignKey(
                        name: "fk_movements_compartments_compartment_id",
                        column: x => x.compartment_id,
                        principalSchema: "inventory",
                        principalTable: "compartments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_movements_components_component_id",
                        column: x => x.component_id,
                        principalSchema: "inventory",
                        principalTable: "components",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_movements_compartment_id",
                schema: "inventory",
                table: "movements",
                column: "compartment_id");

            migrationBuilder.CreateIndex(
                name: "ix_movements_component_id",
                schema: "inventory",
                table: "movements",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "ix_movements_date",
                schema: "inventory",
                table: "movements",
                column: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_compartments_components_component_id",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.DropTable(
                name: "bom_imports",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "movements",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "components",
                schema: "inventory");

            migrationBuilder.RenameColumn(
                name: "component_id",
                schema: "inventory",
                table: "compartments",
                newName: "part_id");

            migrationBuilder.RenameIndex(
                name: "ix_compartments_component_id",
                schema: "inventory",
                table: "compartments",
                newName: "ix_compartments_part_id");

            migrationBuilder.CreateTable(
                name: "parts",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attributes = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    detailed_description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    footprint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    manufacturer_part_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    part_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    provider_part_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parts", x => x.id);
                    table.ForeignKey(
                        name: "fk_parts_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "inventory",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_parts_footprints_footprint_id",
                        column: x => x.footprint_id,
                        principalSchema: "inventory",
                        principalTable: "footprints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_parts_category_id",
                schema: "inventory",
                table: "parts",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_parts_footprint_id",
                schema: "inventory",
                table: "parts",
                column: "footprint_id");

            migrationBuilder.AddForeignKey(
                name: "fk_compartments_parts_part_id",
                schema: "inventory",
                table: "compartments",
                column: "part_id",
                principalSchema: "inventory",
                principalTable: "parts",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
