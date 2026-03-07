using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartsAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "part_id",
                schema: "inventory",
                table: "compartments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                schema: "inventory",
                table: "compartments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_categories_categories_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "inventory",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "footprints",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_footprints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parts",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    part_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    manufacturer_part_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    detailed_description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    provider_part_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    footprint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    attributes = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false)
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
                name: "ix_compartments_part_id",
                schema: "inventory",
                table: "compartments",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_id",
                schema: "inventory",
                table: "categories",
                column: "parent_id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_compartments_parts_part_id",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.DropTable(
                name: "parts",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "footprints",
                schema: "inventory");

            migrationBuilder.DropIndex(
                name: "ix_compartments_part_id",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.DropColumn(
                name: "part_id",
                schema: "inventory",
                table: "compartments");

            migrationBuilder.DropColumn(
                name: "quantity",
                schema: "inventory",
                table: "compartments");
        }
    }
}
