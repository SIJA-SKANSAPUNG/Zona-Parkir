using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking_Zone.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84b7a79d-ed34-4ef8-a467-0003b80ae65b", "AQAAAAIAAYagAAAAECTXlG6sp+pGTCmUfbs11VplfumuNF4BPExMIAdA16I2YFzTCcie53E9V6fOwJW+NA==", "1d77d4af-3802-4344-9df1-11d66de584b7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7880da20-f2b3-4b00-9b8b-b58fbf5f382f", "AQAAAAIAAYagAAAAEFZRRIy4gzQxezan4CmeGN9xFqWJgkc+SxjZEOslIS6muw/ZztIW0c/OiLrPYa0kbw==", "afc8627c-fffe-404c-87f7-156ae71f2ba9" });
        }
    }
}
