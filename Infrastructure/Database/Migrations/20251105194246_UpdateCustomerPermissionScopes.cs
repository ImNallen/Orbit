using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerPermissionScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update customer permissions from Assigned to Global scope
            migrationBuilder.Sql(@"
                UPDATE permissions
                SET scope = 'Global'
                WHERE name IN ('customers:create', 'customers:read', 'customers:update', 'customers:delete');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert customer permissions back to Assigned scope
            migrationBuilder.Sql(@"
                UPDATE permissions
                SET scope = 'Assigned'
                WHERE name IN ('customers:create', 'customers:read', 'customers:update', 'customers:delete');
            ");
        }
    }
}
