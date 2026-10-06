using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterprisePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCreatedAtCursorIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_customers_CreatedAt_Id",
                table: "customers",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customers_CreatedAt_Id",
                table: "customers");
        }
    }
}
