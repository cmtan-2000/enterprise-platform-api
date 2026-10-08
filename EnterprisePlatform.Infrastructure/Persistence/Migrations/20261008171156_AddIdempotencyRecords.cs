using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterprisePlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_addresses_AddressId",
                table: "addresses");

            migrationBuilder.DropIndex(
                name: "IX_addresses_AddressId",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "addresses");

            migrationBuilder.CreateTable(
                name: "IdempotencyRecords",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRecords", x => x.Key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "addresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_addresses_AddressId",
                table: "addresses",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_addresses_AddressId",
                table: "addresses",
                column: "AddressId",
                principalTable: "addresses",
                principalColumn: "Id");
        }
    }
}
