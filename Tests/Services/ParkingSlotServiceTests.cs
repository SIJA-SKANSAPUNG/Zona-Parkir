using Microsoft.AspNetCore.Mvc;
using Moq;
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
        private readonly Guid _testSlotId = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5");
        private readonly Guid _testZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");

        private readonly Mock<IParkingSlotRepository> mockSlotRepository;

        private readonly ParkingSlotService service;

        public ParkingSlotServiceTests()
        {
            mockSlotRepository = new Mock<IParkingSlotRepository>();
            service = new ParkingSlotService(mockSlotRepository.Object);
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

        #region GetByZoneId
        [Fact]
        public void GivenParkingZoneId_WhenGetByParkingZoneId_ThenRepositoryIsCalledTwiceAndReturnedExpectedSlots()
        {
            //Arrange
            var testParkingSlots = new List<ParkingSlot>()
            {
                _testSlot,
                _testSlot,
                new()
            };
            mockSlotRepository
                .Setup(repo => repo.GetAll())
                .Returns(testParkingSlots);

            //Act
            var result = service.GetByParkingZoneId(_testZoneId);

            //Assert
            Assert.Equal(2, result.Count());
            mockSlotRepository.Verify(repo => repo.GetAll(), Times.Once);
        }
        #endregion

        #region Insert
        [Fact]
        public void GivenSlotModel_WhenInsertIsCalled_ThenRepositoryIsCalledTwice()
        {
            //Arrange
            mockSlotRepository.Setup(repo => repo.Insert(_testSlot));
            mockSlotRepository.Setup(repo => repo.Save());

            //Act
            service.Insert(_testSlot);

            //Assert
            mockSlotRepository.Verify(repo => repo.Insert(_testSlot), Times.Once);
            mockSlotRepository.Verify(repo => repo.Save(), Times.Once);
        }
        #endregion
    }
}
