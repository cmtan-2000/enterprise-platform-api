using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterprisePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NameIdempotencyConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_idempotencyrecords",
                table: "idempotencyrecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_idempotency_records",
                table: "idempotencyrecords",
                column: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_idempotency_records",
                table: "idempotencyrecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_idempotencyrecords",
                table: "idempotencyrecords",
                column: "Key");
        }
    }
}
