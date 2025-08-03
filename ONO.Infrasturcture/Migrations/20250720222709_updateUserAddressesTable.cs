using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONO.Infrasturcture.Migrations
{
    /// <inheritdoc />
    public partial class updateUserAddressesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Street",
                table: "UserAddresses",
                newName: "Governorate");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "UserAddresses",
                newName: "FullAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Governorate",
                table: "UserAddresses",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "FullAddress",
                table: "UserAddresses",
                newName: "Country");
        }
    }
}
