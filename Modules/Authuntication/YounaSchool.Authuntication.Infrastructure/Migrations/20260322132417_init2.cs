using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouNaSchool.Authuntication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableEmailNotifications",
                schema: "Auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EnableInAppNotifications",
                schema: "Auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EnablePushNotifications",
                schema: "Auth",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableEmailNotifications",
                schema: "Auth",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableInAppNotifications",
                schema: "Auth",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnablePushNotifications",
                schema: "Auth",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
