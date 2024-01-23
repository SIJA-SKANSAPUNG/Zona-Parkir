using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking_Zone.Data.Migrations
{
    /// <inheritdoc />
    public partial class User_model_and_ParkingSlot_model_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSlot_ParkingZones_ParkingZoneId",
                table: "ParkingSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParkingSlot",
                table: "ParkingSlot");

            migrationBuilder.RenameTable(
                name: "ParkingSlot",
                newName: "ParkingSlots");

            migrationBuilder.RenameIndex(
                name: "IX_ParkingSlot_ParkingZoneId",
                table: "ParkingSlots",
                newName: "IX_ParkingSlots_ParkingZoneId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FullName",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParkingSlots",
                table: "ParkingSlots",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSlots_ParkingZones_ParkingZoneId",
                table: "ParkingSlots",
                column: "ParkingZoneId",
                principalTable: "ParkingZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSlots_ParkingZones_ParkingZoneId",
                table: "ParkingSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParkingSlots",
                table: "ParkingSlots");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "ParkingSlots",
                newName: "ParkingSlot");

            migrationBuilder.RenameIndex(
                name: "IX_ParkingSlots_ParkingZoneId",
                table: "ParkingSlot",
                newName: "IX_ParkingSlot_ParkingZoneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParkingSlot",
                table: "ParkingSlot",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSlot_ParkingZones_ParkingZoneId",
                table: "ParkingSlot",
                column: "ParkingZoneId",
                principalTable: "ParkingZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
