using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONO.Infrasturcture.Migrations
{
    /// <inheritdoc />
    public partial class addNewColumnInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Access",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NoEmail",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecial",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpecial",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Access",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "NoEmail",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
