using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterprisePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdempotencyRecordsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IdempotencyRecords",
                table: "IdempotencyRecords");

            migrationBuilder.RenameTable(
                name: "IdempotencyRecords",
                newName: "idempotencyrecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_idempotencyrecords",
                table: "idempotencyrecords",
                column: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_idempotencyrecords",
                table: "idempotencyrecords");

            migrationBuilder.RenameTable(
                name: "idempotencyrecords",
                newName: "IdempotencyRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdempotencyRecords",
                table: "IdempotencyRecords",
                column: "Key");
        }
    }
}
