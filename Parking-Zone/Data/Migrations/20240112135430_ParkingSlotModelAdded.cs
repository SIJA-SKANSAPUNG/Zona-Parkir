using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking_Zone.Data.Migrations
{
    /// <inheritdoc />
    public partial class ParkingSlotModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingSlot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    IsAvailableForBooking = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ParkingZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingSlot_ParkingZones_ParkingZoneId",
                        column: x => x.ParkingZoneId,
                        principalTable: "ParkingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSlot_ParkingZoneId",
                table: "ParkingSlot",
                column: "ParkingZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingSlot");
        }
    }
}
