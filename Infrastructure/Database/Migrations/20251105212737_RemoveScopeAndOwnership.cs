using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveScopeAndOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_locations_manager_id",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "ix_locations_owner_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "location_role_id",
                table: "user_location_assignments");

            migrationBuilder.DropColumn(
                name: "scope",
                table: "permissions");

            migrationBuilder.DropColumn(
                name: "manager_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "locations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "location_role_id",
                table: "user_location_assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "scope",
                table: "permissions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "ix_locations_manager_id",
                table: "locations",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_owner_id",
                table: "locations",
                column: "owner_id");
        }
    }
}
