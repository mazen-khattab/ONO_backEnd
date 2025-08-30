using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONO.Infrasturcture.Migrations
{
    /// <inheritdoc />
    public partial class renamedTemporaryReservationToGuestCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryReservations_Products_ProductId",
                table: "TemporaryReservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemporaryReservations",
                table: "TemporaryReservations");

            migrationBuilder.RenameTable(
                name: "TemporaryReservations",
                newName: "GuestsCart");

            migrationBuilder.RenameIndex(
                name: "IX_TemporaryReservations_ProductId",
                table: "GuestsCart",
                newName: "IX_GuestsCart_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuestsCart",
                table: "GuestsCart",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestsCart_Products_ProductId",
                table: "GuestsCart",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestsCart_Products_ProductId",
                table: "GuestsCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuestsCart",
                table: "GuestsCart");

            migrationBuilder.RenameTable(
                name: "GuestsCart",
                newName: "TemporaryReservations");

            migrationBuilder.RenameIndex(
                name: "IX_GuestsCart_ProductId",
                table: "TemporaryReservations",
                newName: "IX_TemporaryReservations_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemporaryReservations",
                table: "TemporaryReservations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryReservations_Products_ProductId",
                table: "TemporaryReservations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
