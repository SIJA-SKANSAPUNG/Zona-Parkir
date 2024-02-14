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
        private readonly Guid _testReservationId = Guid.Parse("9479c041-ee52-47f5-a51f-d9ab022e01a2");

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

        #region Prolong
        [Fact]
        public void GivenReservationId_WhenGetProlongIsCalled_ThenServiceIsCalledOnceAndReturnedProlongVM()
        {
            //Arrange
            var now = DateTime.Now;
            var reservation = new Reservation()
            {
                Id = _testReservationId,
                ParkingSlot = _testSlot,
                StartTime = now.AddHours(-1),
                Duration = 3
            };

            var prolongVM = new ProlongVM()
            {
                ReservationId = _testReservationId,
                ExtraHours = 1
            };

            var expectedProlongVM = new ProlongVM()
            {
                ReservationId = _testReservationId,
                StartTime = now.AddHours(-1).ToString(),
                SlotNumber = _testSlot.Number,
                EndDateTime = now.AddHours(2).ToString(),
                ZoneAddress = _testSlot.ParkingZone.Address
            };

            mockReservationService
                .Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(reservation);

            //Act
            var result = controller.Prolong(_testReservationId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(expectedProlongVM), JsonSerializer.Serialize((result as ViewResult).Model));
            mockReservationService.Verify(service => service.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void GivenProlongVMWithNotActiveReservation_WhenPostProlongIsCalled_ThenServiceCalledAndReturnedInvalidVM()
        {
            //Arrange
            var reservation = new Reservation()
            {
                Id = _testReservationId,
                ParkingSlot = _testSlot,
                StartTime = DateTime.Now.AddHours(-6),
                Duration = 1
            };

            var prolongVM = new ProlongVM()
            {
                ReservationId = _testReservationId,
            };

            mockReservationService
                .Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(reservation);

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            mockReservationService.Verify(service => service.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void GivenProlongWithNotFreeSlot_WhenPostProlongIsCalled_ThenServiceCalledAndReturnedInvalidVM()
        {
            //Arrange
            var reservation = new Reservation()
            {
                Id = _testReservationId,
                ParkingSlot = _testSlot,
                StartTime = DateTime.Now.AddHours(-1),
                Duration = 2
            };

            var prolongVM = new ProlongVM()
            {
                ReservationId = _testReservationId,
                ExtraHours = 2
            };

            mockReservationService
                .Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(reservation);

            mockSlotService
                .Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(_testSlot);

            mockSlotService
                .Setup(service => service.IsSlotFree(_testSlot, reservation.StartTime.AddHours(reservation.Duration), prolongVM.ExtraHours))
                .Returns(false);

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            mockReservationService.Verify(service => service.GetById(It.IsAny<Guid>()), Times.Once);
            mockSlotService.Verify(service => service.IsSlotFree(_testSlot, reservation.StartTime.AddHours(reservation.Duration), prolongVM.ExtraHours));
        }

        [Fact]
        public void GivenProlongVM_WhenPostProlongIsCalled_ThenServiceIsCalledTwiceAndReturnedRedirectToActionResult()
        {
            //Arrange
            var testStartTime = DateTime.Now.AddHours(-1);

            var reservation = new Reservation()
            {
                Id = _testReservationId,
                ParkingSlot = _testSlot,
                StartTime = testStartTime,
                Duration = 2
            };

            var prolongVM = new ProlongVM()
            {
                ReservationId = _testReservationId,
                ExtraHours = 1
            };

            mockReservationService
                .Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(reservation);

            mockSlotService
                .Setup(service => service.GetById(It.IsAny<Guid>()))
                .Returns(_testSlot);

            mockSlotService
                .Setup(service => service.IsSlotFree(_testSlot, reservation.StartTime.AddHours(reservation.Duration), prolongVM.ExtraHours))
                .Returns(true);

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockReservationService.Verify(service => service.GetById(It.IsAny<Guid>()), Times.Once);
            mockReservationService.Verify(service => service.Prolong(reservation, prolongVM.ExtraHours), Times.Once);
            mockSlotService.Verify(service => service.IsSlotFree(_testSlot, reservation.StartTime.AddHours(reservation.Duration), prolongVM.ExtraHours));
        }

        [Fact]
        public void GivenReservationId_WhenGetProlongIsCalled_ThenServiceIsCalledOnceReturnedNotFoundResult()
        {
            var reservationId = Guid.NewGuid();

            mockReservationService
                .Setup(service => service.GetById(It.IsAny<Guid>()));

            //Act
            var result = controller.Prolong(reservationId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            mockReservationService.Verify(service => service.GetById(It.IsAny<Guid>()), Times.Once);
        }
        #endregion
    }
}