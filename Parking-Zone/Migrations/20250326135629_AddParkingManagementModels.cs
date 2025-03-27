using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking_Zone.Migrations
{
    /// <inheritdoc />
    public partial class AddParkingManagementModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParkingGates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: false),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParkingZoneId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingGates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingGates_ParkingZones_ParkingZoneId",
                        column: x => x.ParkingZoneId,
                        principalTable: "ParkingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlateNumber = table.Column<string>(type: "text", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhotoEntry = table.Column<byte[]>(type: "bytea", nullable: false),
                    PhotoExit = table.Column<byte[]>(type: "bytea", nullable: false),
                    VehicleType = table.Column<string>(type: "text", nullable: false),
                    TicketBarcode = table.Column<string>(type: "text", nullable: false),
                    IsInside = table.Column<bool>(type: "boolean", nullable: false),
                    FeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "text", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    OperatorName = table.Column<string>(type: "text", nullable: false),
                    ParkingZoneId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingTransactions_ParkingZones_ParkingZoneId",
                        column: x => x.ParkingZoneId,
                        principalTable: "ParkingZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParkingTransactions_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "272f163f-764d-4a23-9c87-a4272f67dc10", "AQAAAAIAAYagAAAAEE/yQ2wB1xkj0J8h4Vt9Wp/8RcF4yHlBZqXmh3uWpPp2I+gX3FFtU0SzAh4FrfhXsA==", "298f6a0b-4ff7-41f7-8e97-be973430f38b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cddc1bf2-222f-4da4-b32b-f506a3f1f825", "AQAAAAIAAYagAAAAEDLfXW/rNJjKaFUydf2/Rfo5m1OFLfftrtvSXxEW3wFbSjQgFmqBKZd/bhrvpnw8Sg==", "667a81b8-1ef7-4436-9c06-ec355854dc3e" });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingGates_ParkingZoneId",
                table: "ParkingGates",
                column: "ParkingZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingTransactions_ParkingZoneId",
                table: "ParkingTransactions",
                column: "ParkingZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingTransactions_VehicleId",
                table: "ParkingTransactions",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParkingGates");

            migrationBuilder.DropTable(
                name: "ParkingTransactions");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c27cdaa6-56fe-44cd-a9a5-641658fab774", "AQAAAAIAAYagAAAAEC9Jw89CDWWtUHKM/be9UkSB/y00GHK1wNRcKUj2NcQVS30lxiMn9SLYyyrTdeg9pg==", "bd1da5a5-9f1d-44bc-98ab-aa6145a21cdc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "53add4a6-9935-468f-a2cb-2d210129343d", "AQAAAAIAAYagAAAAENYprlfxuQNAUCF7R3UQ/NbmEw2pW0qJMCGaubMFx6p4IZ291Ler9y4vxZxE9Zl9FQ==", "c24be699-a84d-409b-9d16-79bfae1a1fd7" });
        }
    }
}
