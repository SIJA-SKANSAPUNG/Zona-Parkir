using Moq;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly string testUserId = "bf6b864c-7d75-4854-9730-7c14bb4732a1";
        private readonly Mock<IReservationRepository> mockReservationRepository;
        private readonly ReservationService service;

        public ReservationServiceTests()
        {
            mockReservationRepository = new Mock<IReservationRepository>();
            service = new ReservationService(mockReservationRepository.Object);
        }

        #region GetByAppUserId
        [Fact]
        public void GivenAppUserId_WhenGetByAppUserIdIsCalled_ThenAllReservationsReturnedBelongsToASingleUser()
        {
            //Arrange
            var reservations = new List<Reservation>()
            {
                new(){ AppUserId = testUserId },
                new(){ AppUserId = new Guid().ToString() },
                new(){ AppUserId = testUserId }
            };
            var expectedReservations = new List<Reservation>()
            {
                new(){ AppUserId = testUserId },
                new(){ AppUserId = testUserId }
            };

            mockReservationRepository
                .Setup(repo => repo.GetAll())
                .Returns(reservations);

            //Act
            var result = service.GetByAppUserId(testUserId);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedReservations), JsonSerializer.Serialize(result));
            mockReservationRepository.Verify(repo => repo.GetAll(), Times.Once);
        }
        #endregion
    }
}
