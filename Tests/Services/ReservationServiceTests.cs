using Moq;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services;
using Parking_Zone.Services.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly IEnumerable<Reservation> testReservations = new List<Reservation>()
        {
            new()
            {
                StartTime = DateTime.Now.AddDays(-44),
                Duration = 5,
                ParkingSlot = new ParkingSlot()
                {
                    Category = Parking_Zone.Enums.SlotCategoryEnum.Standard
                }
            },
            new()
            {
                StartTime = DateTime.Now.AddDays(-27),
                Duration = 4,
                ParkingSlot = new ParkingSlot()
                {
                    Category = Parking_Zone.Enums.SlotCategoryEnum.Business
                }
            },
            new()
            {
                StartTime = DateTime.Now.AddDays(-6),
                Duration = 2,
                ParkingSlot = new ParkingSlot()
                {
                    Category = Parking_Zone.Enums.SlotCategoryEnum.Standard
                }
            },
            new()
            {
                StartTime = DateTime.Now.AddDays(-1),
                Duration = 1,
                ParkingSlot = new ParkingSlot()
                {
                    Category = Parking_Zone.Enums.SlotCategoryEnum.Business
                }
            },
            new()
            {
                StartTime = DateTime.Now.AddHours(-2),
                Duration = 3,
                ParkingSlot = new ParkingSlot()
                {
                    Category = Parking_Zone.Enums.SlotCategoryEnum.Standard
                }
            }
        };

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
