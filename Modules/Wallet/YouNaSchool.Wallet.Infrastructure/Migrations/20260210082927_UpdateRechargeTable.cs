using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNaSchool.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRechargeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientSecret",
                table: "WalletRecharges",
                newName: "ProviderReferenceId");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentProvider",
                table: "WalletRecharges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ClientPaymentToken",
                table: "WalletRecharges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientPaymentToken",
                table: "WalletRecharges");

            migrationBuilder.RenameColumn(
                name: "ProviderReferenceId",
                table: "WalletRecharges",
                newName: "ClientSecret");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentProvider",
                table: "WalletRecharges",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
