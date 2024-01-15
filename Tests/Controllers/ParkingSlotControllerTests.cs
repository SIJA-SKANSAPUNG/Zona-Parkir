using Microsoft.AspNetCore.Mvc;
using Moq;
using Parking_Zone.Areas.Admin.Controllers;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class ParkingSlotControllerTests
    {
        private readonly Guid _testZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");
        private readonly Guid _testSlotId = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5");

        private readonly Mock<IParkingZoneService> mockZoneService;
        private readonly Mock<IParkingSlotService> mockSlotService;

        private readonly ParkingSlotController controller;

        public ParkingSlotControllerTests()
        {
            mockZoneService = new Mock<IParkingZoneService>();
            mockSlotService = new Mock<IParkingSlotService>();
            controller = new ParkingSlotController(mockZoneService.Object, mockSlotService.Object);
        }

        private readonly ParkingSlot _testSlot = new ParkingSlot()
        {
            Id = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5"),
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
        };

        private readonly ParkingZone _testZone = new ParkingZone()
        {
            Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Name = "Sharafshon",
            Address = "Andijon",
            Description = "Arzon"
        };

        private readonly IEnumerable<ParkingSlot> _testSlots = new List<ParkingSlot>()
        {
            new ParkingSlot()
            {
                Id = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5"),
                Number = 1,
                ParkingZone = new ParkingZone() { },
                ParkingZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true
            },
            new ParkingSlot()
            {
                Id = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5"),
                Number = 1,
                ParkingZone = new ParkingZone() { },
                ParkingZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true
            },
        };

        #region Index
        [Fact]
        public void GivenParkingZoneId_WhenIndexIsCalled_ThenParkingZoneAndParkingSlotServicesCalledOnceAndReturnedNotEmptyViewResult()
        {
            //Arrange
            mockZoneService
                .Setup(service => service.GetById(_testZoneId))
                .Returns(_testZone);

            mockSlotService
                .Setup(service => service.GetByParkingZoneId(_testZoneId))
                .Returns(_testSlots);

            //Act
            var result = controller.Index(_testZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull((result as ViewResult).Model);
            Assert.Equal(2, _testSlots.Count());
            mockZoneService.Verify(s => s.GetById(_testZoneId), Times.Once);
            mockSlotService.Verify(s => s.GetByParkingZoneId(_testZoneId), Times.Once);
        }

        public void GivenNullParkingZoneId_WhenIndexIsCalled_ThenParkingZoneServiceIsCalledOnceAndActionReturnedBadRequest()
        {
            //Arrange

            //Act
            var result = controller.Index(Guid.Parse(""));

            //Assert
            Assert.IsType<BadRequestResult>(result);
            mockZoneService.Verify(s => s.GetById(Guid.Parse("")), Times.Once);
        }
        #endregion

        #region Create
        [Fact]
        public void GivenParkingZoneId_WhenGetCreateIsCalled_ThenParkingZoneServiceIsCalledAndReturnedNotEmptyViewResult()
        {
            //Arrange
            mockZoneService.Setup(service => service.GetById(_testZoneId)).Returns(_testZone);

            //Act
            var result = controller.Create(_testZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull((result as ViewResult).Model);
            mockZoneService.Verify(s => s.GetById(_testZoneId), Times.Once);
        }

        [Fact]
        public void GivenValidModel_WhenPostCreateIsCalled_ThenParkingSlotServiceIsCalledOnceAndRedirectedToIndexView()
        {
            //Arrange
            var parkingSlotCreateVM = new ParkingSlotCreateVM()
            {
                Number = 1,
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true
            };

            mockSlotService.Setup(service => service.Insert(It.IsAny<ParkingSlot>()));

            //Act
            var result = controller.Create(parkingSlotCreateVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Equal("ParkingSlot", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockSlotService.Verify(s => s.Insert(It.IsAny<ParkingSlot>()), Times.Once);
        }

        [Fact]
        public void GivenModelWithNullNumber_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var parkingSlotCreateVM = new ParkingSlotCreateVM()
            {
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true
            };

            controller.ModelState.AddModelError("Number", "Number of Slot is Required");

            //Act
            var result = controller.Create(parkingSlotCreateVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(parkingSlotCreateVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenModelWithNullCategory_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var parkingSlotCreateVM = new ParkingSlotCreateVM()
            {
                Number = 1,
                ParkingZoneId = _testZoneId,
                IsAvailableForBooking = true
            };

            controller.ModelState.AddModelError("Category", "Category of Slot is Required");

            //Act
            var result = controller.Create(parkingSlotCreateVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(parkingSlotCreateVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }
        #endregion
    }
}
