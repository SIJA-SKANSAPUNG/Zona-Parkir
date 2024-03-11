using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parking_Zone.Data.Migrations
{
    /// <inheritdoc />
    public partial class RolesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "Email", "FullName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "edf8cd4d-3f5c-46c3-8b45-9c7f97e5251b", "admin@admin.com", "Adminov Admin", "AQAAAAIAAYagAAAAEDxdKV4C8C+KzXjOFGcNoZgIAUa17djBkCSX0TjofBxiVpZWb9FLmAgJzkDLknfRXw==", "509a15ed-d7f2-4c6b-9d57-0b109378c427", "admin@admin.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "Email", "FullName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c00e0504-27e0-483f-bf7e-5c3dbbb08e75", "user@user.com", "Userov User", "AQAAAAIAAYagAAAAELr9q4B05ZubG0qajBQxk/HPHMaHTtv0QDKIhqI1pnlsLw1BAnEPCWTcjmuQUAEHkw==", "f2e704f6-b695-4878-8cf7-93da070f5160", "user@user.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                columns: new[] { "ConcurrencyStamp", "Email", "FullName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "b7772dc8-5b2d-4100-a612-77acdf577f9b", "admin@gmail.com", "Admin User", "AQAAAAIAAYagAAAAEDw20B0/CpmETD51AK5rLWiiMglTYRuZ0fGIbE5IOUYgwTsFBtpXa6mtsfSQtW1+bg==", "052cf798-d371-4dc2-be5c-b16f01de4a55", "admin@gmail.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d47b3c1e-1310-409d-b893-0a662a64c35d",
                columns: new[] { "ConcurrencyStamp", "Email", "FullName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "046b62f4-4526-4287-b7fc-71eedbaf389c", "user@example.com", "Normal User", "AQAAAAIAAYagAAAAEIGZOPx+H1NWkh2gHIf2mV+ZDPT5EKtKA8WEv+lfIq9lBfTlGxxPMI5zN7GAuAZKFA==", "c4a155bd-a245-452d-a553-52ac9fd30aae", "user@example.com" });
        }
    }
}
