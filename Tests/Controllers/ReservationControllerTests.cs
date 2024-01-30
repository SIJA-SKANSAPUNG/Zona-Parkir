using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Parking_Zone.Areas.User.Controllers;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels;
using Parking_Zone.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class ReservationControllerTests
    {
        private readonly Guid _testZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");

        private readonly Mock<IParkingZoneService> mockZoneService;
        private readonly Mock<IParkingSlotService> mockSlotService;

        private readonly ReservationController controller;

        private readonly ParkingZone _testZone = new ParkingZone()
        {
            Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Name = "Sharafshon",
            Address = "Andijon",
            Description = "Arzon"
        };

        public ReservationControllerTests()
        {
            mockZoneService = new Mock<IParkingZoneService>();
            mockSlotService = new Mock<IParkingSlotService>();
            controller = new ReservationController(mockZoneService.Object, mockSlotService.Object);
        }

        [Fact]
        public void GivenNothing_WhenGetFreeSlotsCalled_ThenZoneServiceCalledOnceAndReturnedViewResultWithViewModel()
        {
            //Arrange
            var expectedReservationVM = new FreeSlotsVM()
            {
                ParkingZones = new SelectList(new List<ParkingZone>()
                {
                    _testZone
                }, "Id", "Name")
            };

            mockZoneService
                .Setup(service => service.GetAll())
                .Returns(new List<ParkingZone>() { _testZone });

            //Act
            var result = controller.FreeSlots();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(expectedReservationVM), JsonSerializer.Serialize((result as ViewResult).Model));
            mockZoneService.Verify(service => service.GetAll(), Times.Once);
        }

        [Fact]
        public void GivenReservationVMModel_WhenPostFreeSlotsCalled_ThenServiceCalledOnceAndReturnedNotEmptyViewResult()
        {
            //Arrange
            var testStartTime = new DateTime(2024, 1, 27, 18, 00, 00);
            var reservationVM = new FreeSlotsVM()
            {
                StartTime = testStartTime,
                Duration = 3,
                ParkingZoneId = _testZoneId
            };

            mockSlotService
                .Setup(service => service.GetFreeByZoneIdAndTimePeriod(_testZoneId, testStartTime, 3));

            //Act
            var result = controller.FreeSlots(reservationVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result as ViewResult);
            mockSlotService.Verify(service => service.GetFreeByZoneIdAndTimePeriod(_testZoneId, testStartTime, 3), Times.Once);
        }
    }
}