using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Parking_Zone.Areas.User.Controllers;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class ReservationControllerTests
    {
        private readonly Guid _testSlotId = Guid.Parse("756f4ba1-85cb-4998-997b-bb61ced58071");

        private readonly Mock<IParkingZoneService> mockZoneService;
        private readonly Mock<IParkingSlotService> mockSlotService;
        private readonly Mock<IReservationService> mockReservationService;

        private readonly ReservationController controller;

        private readonly ParkingSlot _testSlot = new ParkingSlot()
        {
            Id = Guid.Parse("756f4ba1-85cb-4998-997b-bb61ced58071"),
            Number = 1,
            ParkingZone = new ParkingZone()
            {
                Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Arzon"
            },
            ParkingZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Category = SlotCategoryEnum.Business,
            IsAvailableForBooking = true,
            Reservations = new List<Reservation>()
            {
                new Reservation()
                {
                    StartTime = DateTime.Parse("2024-01-27 18:00:00"),
                    Duration = 4
                }
            }
        };

        public ReservationControllerTests()
        {
            mockZoneService = new Mock<IParkingZoneService>();
            mockSlotService = new Mock<IParkingSlotService>();
            mockReservationService = new Mock<IReservationService>();
            controller = new ReservationController(mockZoneService.Object, mockSlotService.Object, mockReservationService.Object);
        }

        #region Index
        [Fact]
        public void GivenUserId_WhenIndexIsCalled_ThenReservationListItemVMsReturned()
        {
            //Arrange
            var testUserId = "7d9b25d9-8efc-445e-bbf5-47a1b6cf4fb5";

            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, "7d9b25d9-8efc-445e-bbf5-47a1b6cf4fb5")
            }, "mock"));

            var reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = DateTime.Now.AddHours(-2),
                    Duration = 4,
                    ParkingSlot = _testSlot
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(-6),
                    Duration = 2,
                    ParkingSlot = _testSlot
                }
            };
            var expectedReservationVM = new List<ReservationListItemVM>()
            {
                new(reservations[0]),
                new(reservations[1])
            };

            mockReservationService
                .Setup(service => service.GetByAppUserId(testUserId))
                .Returns(reservations);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = testUser }
            };

            //Act
            var result = controller.Index();


            //Arrange
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(expectedReservationVM), JsonSerializer.Serialize((result as ViewResult).Model));
            mockReservationService.Verify(service => service.GetByAppUserId(testUserId), Times.Once);
        }
        #endregion
    }
}