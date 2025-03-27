using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking_Zone.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                column: "NormalizedName",
                value: "ADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                column: "NormalizedName",
                value: "USER");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84b7a79d-ed34-4ef8-a467-0003b80ae65b", true, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAECTXlG6sp+pGTCmUfbs11VplfumuNF4BPExMIAdA16I2YFzTCcie53E9V6fOwJW+NA==", "1d77d4af-3802-4344-9df1-11d66de584b7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7880da20-f2b3-4b00-9b8b-b58fbf5f382f", true, "USER@USER.COM", "USER@USER.COM", "AQAAAAIAAYagAAAAEFZRRIy4gzQxezan4CmeGN9xFqWJgkc+SxjZEOslIS6muw/ZztIW0c/OiLrPYa0kbw==", "afc8627c-fffe-404c-87f7-156ae71f2ba9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dcc78484-000d-430d-bf19-9ffa251b0feb", false, null, null, "AQAAAAIAAYagAAAAEJ31KUDcion8T+Hu9rUua+K/czDN/REt+8/bNEItM+xrMQQbanJfTaylfFQQ8d28AA==", "46d5e891-bb12-40da-89de-f22cb15613c5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "993064ae-1a67-4f1a-b2a0-20492722aa31", false, null, null, "AQAAAAIAAYagAAAAEJah3r1Rp3C82ZucqG2ZnyJ8wmYinV6+5w/06Bpd1qQcz8Nu9bPIKfgGPHM0B3NVHw==", "5bfb5138-a7da-41c8-a842-3d5996fb5480" });
        }
    }
}
