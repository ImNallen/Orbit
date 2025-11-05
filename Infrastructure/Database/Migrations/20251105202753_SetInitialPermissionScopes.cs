using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SetInitialPermissionScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set Global scope for permissions that should be global
            migrationBuilder.Sql(@"
                UPDATE permissions
                SET scope = 'Global'
                WHERE name IN (
                    'customers:create', 'customers:read', 'customers:update', 'customers:delete',
                    'roles:create', 'roles:read', 'roles:update', 'roles:delete',
                    'permissions:read'
                );
            ");

            // Set Assigned scope for location-based permissions
            migrationBuilder.Sql(@"
                UPDATE permissions
                SET scope = 'Assigned'
                WHERE name IN (
                    'locations:create', 'locations:read', 'locations:update', 'locations:delete',
                    'users:create', 'users:read', 'users:update', 'users:delete',
                    'inventory:create', 'inventory:read', 'inventory:update', 'inventory:delete',
                    'products:create', 'products:read', 'products:update', 'products:delete',
                    'sessions:read'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert all permissions to empty string
            migrationBuilder.Sql(@"
                UPDATE permissions
                SET scope = ''
                WHERE scope IN ('Global', 'Assigned');
            ");
        }
    }
}
