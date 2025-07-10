using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONO.Infrasturcture.Migrations
{
    /// <inheritdoc />
    public partial class addedReservedColumnToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reserved",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reserved",
                table: "Products");
        }
    }
}
