﻿using Moq;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class ParkingSlotServiceTests
    {
        private readonly Guid _testParkingSlotId = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5");
        private readonly Guid _testParkingZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");
        private readonly Mock<IParkingSlotRepository> mockParkingSlotRepository;
        private readonly ParkingSlotService service;
        public ParkingSlotServiceTests()
        {
            mockParkingSlotRepository = new Mock<IParkingSlotRepository>();
            service = new ParkingSlotService(mockParkingSlotRepository.Object);
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

        #region GetByZoneId
        [Fact]
        public void GivenParkingZoneId_WhenGetByParkingZoneId_ThenRepositoryIsCalledTwiceAndReturnedExpectedZones()
        {
            //Arrange
            var expectedParkingSlots = new List<ParkingSlot>()
            {
                _testParkingSlot,
                _testParkingSlot,
                _testParkingSlot
            };
            mockParkingSlotRepository.Setup(repo => repo.GetAll()).Returns(expectedParkingSlots);

            //Act
            var result = service.GetByParkingZoneId(_testParkingZoneId);

            //Assert
            Assert.Equal(3, expectedParkingSlots.Count);
            mockParkingSlotRepository.Verify(repo => repo.GetAll(), Times.Once);
        }
        #endregion
    }
}
