﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using Parking_Zone.Areas.Admin.Controllers;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class ParkingSlotControllerTests
    {
        private readonly Guid _testParkingZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");
        private readonly Guid _testParkingSlotId = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5");

        private readonly Mock<IParkingZoneService> mockParkingZoneService;
        private readonly Mock<IParkingSlotService> mockParkingSlotService;
        private readonly ParkingSlotController controller;
        public ParkingSlotControllerTests()
        {
            mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingSlotService = new Mock<IParkingSlotService>();
            controller = new ParkingSlotController(mockParkingZoneService.Object, mockParkingSlotService.Object);
        }

        private readonly ParkingSlot _testParkingSlot = new ParkingSlot()
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
        private readonly ParkingZone _testParkingZone = new ParkingZone()
        {
            Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Name = "Sharafshon",
            Address = "Andijon",
            Description = "Arzon"
        };
        private readonly IEnumerable<ParkingSlot> _testParkingSlots = new List<ParkingSlot>()
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
        public void GivenParkingZoneId_WhenIndexIsCalled_ThenParkingZoneAndParkingSlotServicesCallledOnceAndReturnedNotEmptyViewResult()
        {
            //Arrange
            mockParkingZoneService.Setup(service => service.GetById(_testParkingZoneId)).Returns(_testParkingZone);
            mockParkingSlotService.Setup(service => service.GetByParkingZoneId(_testParkingZoneId)).Returns(_testParkingSlots);

            //Act
            var result = controller.Index(_testParkingZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull((result as ViewResult).Model);
            mockParkingZoneService.Verify(s => s.GetById(_testParkingZoneId), Times.Once);
            mockParkingSlotService.Verify(s => s.GetByParkingZoneId(_testParkingZoneId), Times.Once);
        }
        #endregion
    }
}
