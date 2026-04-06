using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNaSchool.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ProcessedOn",
                table: "OutboxMessages");

            migrationBuilder.EnsureSchema(
                name: "Wallets");

            migrationBuilder.RenameTable(
                name: "Wallets",
                newName: "Wallets",
                newSchema: "Wallets");

            migrationBuilder.RenameTable(
                name: "WalletRecharges",
                newName: "WalletRecharges",
                newSchema: "Wallets");

            migrationBuilder.RenameTable(
                name: "WalletLedgerEntries",
                newName: "WalletLedgerEntries",
                newSchema: "Wallets");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "OutboxMessages",
                newSchema: "Wallets");

            migrationBuilder.RenameColumn(
                name: "ProcessedOn",
                schema: "Wallets",
                table: "OutboxMessages",
                newName: "PublishedAt");

            migrationBuilder.RenameColumn(
                name: "OccurredOn",
                schema: "Wallets",
                table: "OutboxMessages",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Exchange",
                schema: "Wallets",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "Wallets",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoutingKey",
                schema: "Wallets",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exchange",
                schema: "Wallets",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "Wallets",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RoutingKey",
                schema: "Wallets",
                table: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "Wallets",
                schema: "Wallets",
                newName: "Wallets");

            migrationBuilder.RenameTable(
                name: "WalletRecharges",
                schema: "Wallets",
                newName: "WalletRecharges");

            migrationBuilder.RenameTable(
                name: "WalletLedgerEntries",
                schema: "Wallets",
                newName: "WalletLedgerEntries");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "Wallets",
                newName: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "PublishedAt",
                table: "OutboxMessages",
                newName: "ProcessedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OutboxMessages",
                newName: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn",
                table: "OutboxMessages",
                column: "ProcessedOn");
        }
    }
}
