using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationBasedAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "current_location_context_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "manager_id",
                table: "locations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "locations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_location_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_primary_location = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    assigned_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    terminated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_location_assignments", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_current_location_context_id",
                table: "users",
                column: "current_location_context_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_manager_id",
                table: "locations",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_owner_id",
                table: "locations",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_location_assignments_location_id",
                table: "user_location_assignments",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_location_assignments_status",
                table: "user_location_assignments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_user_location_assignments_user_id",
                table: "user_location_assignments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_location_assignments_user_location",
                table: "user_location_assignments",
                columns: new[] { "user_id", "location_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_location_assignments");

            migrationBuilder.DropIndex(
                name: "ix_users_current_location_context_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_locations_manager_id",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "ix_locations_owner_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "current_location_context_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "manager_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "locations");
        }
    }
}
