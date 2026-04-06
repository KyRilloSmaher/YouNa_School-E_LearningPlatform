using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNaSchool.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletLedgerEntries_Wallets_WalletId",
                table: "WalletLedgerEntries");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderReferenceId",
                table: "WalletRecharges",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletRecharges_ProviderReferenceId",
                table: "WalletRecharges",
                column: "ProviderReferenceId",
                unique: true,
                filter: "[ProviderReferenceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletLedgerEntries_Wallets_WalletId",
                table: "WalletLedgerEntries",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletRecharges_Wallets_WalletId",
                table: "WalletRecharges",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletLedgerEntries_Wallets_WalletId",
                table: "WalletLedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletRecharges_Wallets_WalletId",
                table: "WalletRecharges");

            migrationBuilder.DropIndex(
                name: "IX_WalletRecharges_ProviderReferenceId",
                table: "WalletRecharges");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderReferenceId",
                table: "WalletRecharges",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "OutboxMessages",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletLedgerEntries_Wallets_WalletId",
                table: "WalletLedgerEntries",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
