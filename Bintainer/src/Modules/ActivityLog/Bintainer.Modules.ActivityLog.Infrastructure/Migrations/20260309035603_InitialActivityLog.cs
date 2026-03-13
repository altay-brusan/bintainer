using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bintainer.Modules.ActivityLog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "activity");

            migrationBuilder.CreateTable(
                name: "activities",
                schema: "activity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_activities", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_activities_entity_type",
                schema: "activity",
                table: "activities",
                column: "entity_type");

            migrationBuilder.CreateIndex(
                name: "ix_activities_entity_type_entity_id",
                schema: "activity",
                table: "activities",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_activities_timestamp",
                schema: "activity",
                table: "activities",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "ix_activities_user_id",
                schema: "activity",
                table: "activities",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activities",
                schema: "activity");
        }
    }
}
