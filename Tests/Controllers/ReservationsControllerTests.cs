﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Parking_Zone.Areas.User.Controllers;
using Parking_Zone.Controllers;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tests.Controllers
{
    public class ReservationsControllerTests
    {
        private readonly Guid _testZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");
        private readonly Guid _testSlotId = Guid.Parse("756f4ba1-85cb-4998-997b-bb61ced58071");

        private readonly Mock<IParkingZoneService> mockZoneService;
        private readonly Mock<IParkingSlotService> mockSlotService;
        private readonly Mock<IReservationService> mockReservationService;

        private readonly ReservationsController controller;

        private readonly ParkingZone _testZone = new ParkingZone()
        {
            Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Name = "Sharafshon",
            Address = "Andijon",
            Description = "Arzon"
        };

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

        public ReservationsControllerTests()
        {
            mockZoneService = new Mock<IParkingZoneService>();
            mockSlotService = new Mock<IParkingSlotService>();
            mockReservationService = new Mock<IReservationService>();
            controller = new ReservationsController(mockZoneService.Object, mockSlotService.Object, mockReservationService.Object);
        }

        #region FreeSlots
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
            DateTime testStartTime = new DateTime(2024, 01, 27, 18, 00, 00);

            var freeSlots = new List<ParkingSlot>()
            {
                new()
                {
                    ParkingZoneId = _testZoneId
                }
            };

            var freeSlotVM = new FreeSlotsVM()
            {
                StartTime = testStartTime.ToString(),
                Duration = 3,
                ParkingZoneId = _testZoneId
            };

            var expectedSlotsVM = new FreeSlotsVM()
            {
                ParkingZoneId = _testZoneId,
                StartTime = testStartTime.ToString(),
                Duration = 3,
                ParkingSlots = freeSlots.Select(s => new ParkingSlotListItemVM(s)),
                ParkingZones = new SelectList(new List<ParkingZone>() { }, "Id", "Name")
            };

            mockSlotService
                .Setup(service => service.GetFreeByZoneIdAndTimePeriod(_testZoneId, testStartTime, 3))
                .Returns(freeSlots);

            //Act
            var result = controller.FreeSlots(freeSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result as ViewResult);
            Assert.Equal(JsonSerializer.Serialize(expectedSlotsVM), JsonSerializer.Serialize((result as ViewResult).Model));
            mockSlotService.Verify(service => service.GetFreeByZoneIdAndTimePeriod(_testZoneId, testStartTime, 3), Times.Once);
        }
        #endregion

        #region Reserve
        [Fact]
        public void GivenSlotIdStartTimeAndDuration_WhenGetReserveIsCalled_ThenServiceCalledOnceAndReturnedNotEmptyViewResult()
        {
            //Arrange
            var testStartTime = "2024-01-27 18:00:00";
            var duration = 2;
            var expectedReserveVM = new ReserveVM(_testSlot, testStartTime, duration);

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            //Act
            var result = controller.Reserve(_testSlotId, testStartTime, 2);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(expectedReserveVM), JsonSerializer.Serialize((result as ViewResult).Model));
            mockSlotService.Verify(service => service.GetById(_testSlotId), Times.Once);
        }

        private ClaimsPrincipal CreateMockClaimsPrincipal()
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, "AppUserId")
            };
            var identity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public void GivenReserveVM_WhenPostReserveIsCalled_ThenServicesAreCalledThreeTimes()
        {
            //Arrange
            var testStartTime = "2024-01-27 18:00:00";
            var reserveVM = new ReserveVM(_testSlot, testStartTime, 2);
            reserveVM.VehicleNumber = "777AAA";

            var mockClaimsPrincipal = CreateMockClaimsPrincipal();

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
            };

            controller.ControllerContext = controllerContext;

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);
            mockSlotService
                .Setup(service => service.IsSlotFree(_testSlot, DateTime.Parse(testStartTime), 2))
                .Returns(true);
            mockReservationService
                .Setup(service => service.Insert(It.IsAny<Reservation>()));

            //Act
            var result = controller.Reserve(reserveVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockSlotService.Verify(service => service.GetById(_testSlotId), Times.Once);
            mockSlotService.Verify(service => service.IsSlotFree(_testSlot, DateTime.Parse(testStartTime), 2), Times.Once);
            mockReservationService.Verify(service => service.Insert(It.IsAny<Reservation>()), Times.Once);
        }

        [Fact]
        public void GivenReserveVMWithNullVehicleNumber_WhenPostReserveIsCalled_ThenViewReturnedModelStateIsInvalid()
        {
            //Arrange
            var testStartTime = "2024-01-27 18:00:00";
            var reserveVM = new ReserveVM(_testSlot, testStartTime, 2);
            reserveVM.VehicleNumber = null;

            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "apptUserId")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = testUser }
            };

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            controller.ModelState.AddModelError("VehicleNumber", "Vehicle Number is Required");

            //Act
            var result = controller.Reserve(reserveVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void GivenIdOfNotExistingSlot_WhenGetReserveIsCalled_ThenReturnedNotFoundResulted()
        {
            //Arrange
            var testStartTime = "2024-01-27 18:00:00";
            var duration = 2;

            mockSlotService
                .Setup(service => service.GetById(_testSlotId));

            //Act
            var result = controller.Reserve(_testSlotId, testStartTime, duration);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            mockSlotService.Verify(service => service.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenReserveVMWithNotExistingSlotId_WhenPostReserveIsCalled_ThenReturnedNotFoundResult()
        {
            //Arrange
            var reserveVM = new ReserveVM()
            {
                SlotId = _testSlotId,
                Duration = 2,
                StartTime = "2024-01-27 18:00:00"
            };

            mockSlotService
                .Setup(service => service.GetById(reserveVM.SlotId));

            //Act
            var result = controller.Reserve(reserveVM);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            mockSlotService.Verify(service => service.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenReserveVMWithNotFreeSlot_WhenPostReserveIsCalled_ThenReturnedModelStateIsInvalid()
        {
            //Arrange
            var testStartTime = "2024-01-27 18:00:00";
            var duration = 2;
            var reserveVM = new ReserveVM()
            {
                SlotId = _testSlotId,
                Duration = 2,
                StartTime = "2024-01-27 18:00:00"
            };
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, "appUserId")
            }, "mock"));

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);
            mockSlotService
                .Setup(service => service.IsSlotFree(_testSlot, DateTime.Parse(testStartTime), duration))
                .Returns(false);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = testUser }
            };

            //Act
            var result = controller.Reserve(reserveVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            mockSlotService.Verify(service => service.GetById(_testSlotId), Times.Once);
            mockSlotService.Verify(service => service.IsSlotFree(_testSlot, DateTime.Parse(testStartTime), duration), Times.Once);
        }
        #endregion
    }
}