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
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class ParkingSlotServiceTests
    {
        private readonly Guid _testSlotId = Guid.Parse("ab8e46f4-a343-4571-a1a5-14892bccc7f5");
        private readonly Guid _testZoneId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");
        private readonly Guid _testReservationId = Guid.Parse("ff871131-215c-4a2a-8d50-0223108c1b55");

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

        #region SlotExistsWithThisNumber
        [Fact]
        public void GivenNumberThatAlreadyExistsInDbAndZoneId_WhenSlotWithThisNumberExistsIsCalled_ThenRepositoryIsCalledOnceAndReturnedTrue()
        {
            //Arrange
            var slots = new List<ParkingSlot>()
            {
                new ParkingSlot()
                {
                    Number = 1,
                    ParkingZoneId = _testZoneId,
                },
                new ParkingSlot()
                {
                    ParkingZoneId = _testZoneId
                }
            };

            mockSlotRepository
                .Setup(repo => repo.GetAll())
                .Returns(slots);

            //Act
            var result = service.SlotExistsWithThisNumber(1, _testSlotId, _testZoneId);

            //Assert
            Assert.True(result);
            mockSlotRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GivenNumberThatNotExistsInDbAndZoneId_WhenSlotWithThisNumberExistsIsCalled_ThenRepositoryIsCalledOnceAndReturnedFalse()
        {
            //Arrange
            var slots = new List<ParkingSlot>()
            {
                new ParkingSlot()
                {
                    Number = 1,
                    ParkingZoneId = _testZoneId,
                },
                new ParkingSlot()
                {
                    Number = 2,
                    ParkingZoneId = _testZoneId
                }
            };

            mockSlotRepository
                .Setup(repo => repo.GetAll())
                .Returns(slots);

            //Act
            var result = service.SlotExistsWithThisNumber(5, _testSlotId, _testZoneId);

            //Assert
            Assert.False(result);
            mockSlotRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GivenNumberAndIdOfExistingSlotAndZoneId_WhenSlotWithThisNumberExistsIsCalled_ThenRepositoryIsCalledOnceAndReturnedFalse()
        {
            //Arrange
            var slots = new List<ParkingSlot>()
            {
                new ParkingSlot()
                {
                    Id = _testSlotId,
                    Number = 1,
                    ParkingZoneId = _testZoneId,
                },
                new ParkingSlot()
                {
                    Number = 2,
                    ParkingZoneId = _testZoneId
                }
            };

            mockSlotRepository
                .Setup(repo => repo.GetAll())
                .Returns(slots);

            //Act
            var result = service.SlotExistsWithThisNumber(1, _testSlotId, _testZoneId);

            //Assert
            Assert.False(result);
            mockSlotRepository.Verify(repo => repo.GetAll(), Times.Once);
        }
        #endregion

        #region GetAllSlotsByZoneIdForReservation
        [Fact]
        public void GivenZoneIdStartTimeAndDuration_WhenGetFreeByZoneIdAndTimePeriodCalled_ThenOnlyFreeAndAvailableSlotsAreReturned()
        {
            //Arrange
            var testStartTime = new DateTime(2024, 1, 27, 18, 00, 00);
            var testDuration = 2;

            var freeSlots = new List<ParkingSlot>()
            {
                new()
                {
                        IsAvailableForBooking = true,
                        ParkingZoneId = _testZoneId,
                        Reservations = new List<Reservation>()
                        {
                            new()
                            {
                                Id = _testReservationId,
                                StartTime = new DateTime(2024, 1, 27, 21, 00, 00),
                                Duration = 2
                            }
                        }
                },
                new()
                {
                        IsAvailableForBooking = true,
                        ParkingZoneId = _testZoneId,
                        Reservations = new List<Reservation>()
                        {
                            new()
                            {
                                Id = _testReservationId,
                                StartTime = new DateTime(2024, 1, 27, 13, 00, 00),
                                Duration = 5
                            }
                        }
                }
            };

            var bookedSlots = new List<ParkingSlot>()
            {
                new()
                {
                        IsAvailableForBooking = true,
                        ParkingZoneId = _testZoneId,
                        Reservations = new List<Reservation>()
                        {
                            new()
                            {
                                Id = _testReservationId,
                                StartTime = new DateTime(2024, 1, 27, 18, 00, 00),
                                Duration = 3
                            }
                        }
                },
                new()
                {
                        IsAvailableForBooking = false,
                        ParkingZoneId = _testZoneId,
                        Reservations = new List<Reservation>()
                        {
                            new()
                            {
                                Id = _testReservationId,
                                StartTime = new DateTime(2024, 1, 27, 19, 00, 00),
                                Duration = 1
                            }
                        }
                }
            };

            var all_slots = freeSlots.Concat(bookedSlots);

            mockSlotRepository
                .Setup(repo => repo.GetAllWithReservations())
                .Returns(all_slots);

            //Act
            var result = service.GetFreeByZoneIdAndTimePeriod(_testZoneId, testStartTime, testDuration);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(freeSlots), JsonSerializer.Serialize(result));
            mockSlotRepository.Verify(repo => repo.GetAllWithReservations(), Times.Once);
        }
        #endregion

        #region IsSlotFree
        public static IEnumerable<object[]> TestData()
        {
            var slot = new ParkingSlot()
            {
                Id = new Guid("d4b5425b-a731-4f5d-a61c-f9441fc388d5"),
                Reservations = new List<Reservation>()
                {
                    new()
                    {
                        StartTime = new DateTime(2024, 1, 27, 16, 00, 00),
                        Duration = 2
                    }
                }
            };

            yield return new object[] { slot, new DateTime(2024, 1, 27, 16, 00, 00), 2, false };
            yield return new object[] { slot, new DateTime(2024, 1, 27, 17, 00, 00), 3, false };
            yield return new object[] { slot, new DateTime(2024, 1, 27, 18, 00, 00), 2, true };
            yield return new object[] { slot, new DateTime(2024, 1, 27, 8, 00, 00), 5, true };
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void GivenSlotWithReservations_WhenIsSlotFreeIsCalled_ThenExpectedResultReturned(ParkingSlot slot, DateTime startTime, int duration, bool expectedResult)
        {
            //Arrange

            //Act
            var result = service.IsSlotFree(slot, startTime, duration);

            //Assert
            Assert.Equal(result, expectedResult);
        }
        #endregion
    }
}
