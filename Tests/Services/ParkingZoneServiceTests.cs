using Moq;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class ParkingZoneServiceTests
    {
        private readonly Guid _testId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");

        private readonly ParkingZone _testParkingZone = new ParkingZone()
        {
            Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Name = "Sharafshon",
            Address = "Andijon",
            Description = "Arzon"
        };

        #region GetAll
        [Fact]
        public void GivenNothing_WhenGetAllIsCalled_ThenRepositoryIsCalledOnceAndReturnedExpectedZones()
        {
            //Arrange
            var parkingZones = new List<ParkingZone>()
            {
                new ParkingZone()
                {
                    Id = _testId,
                    Name = "Sharafshon",
                    Address = "Andijon",
                    Description = "Arzon"
                },
                new ParkingZone()
                {
                    Id = _testId,
                    Name = "R7",
                    Address = "Farg'ona",
                    Description = "Qimmat"
                }
            };
            var mockRepository = new Mock<IParkingZoneRepository>();

            mockRepository
                .Setup(repo => repo.GetAll())
                .Returns(parkingZones);

            var service = new ParkingZoneService(mockRepository.Object);

            //Act
            var result = service.GetAll();

            //Asert
            Assert.Equal(JsonSerializer.Serialize(result), JsonSerializer.Serialize(parkingZones));
            mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        }
        #endregion

        #region GetById
        [Fact]
        public void GivenId_WhenGetByIdIsCalled_ThenRepositoryIsCalledOnceAndReturnedExpectedParkingZone()
        {
            //Arrange
            var mockRepository = new Mock<IParkingZoneRepository>();

            mockRepository
                .Setup(repo => repo.GetById(_testId))
                .Returns(_testParkingZone);

            var service = new ParkingZoneService(mockRepository.Object);

            //Act
            var result = service.GetById(_testId);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(_testParkingZone), JsonSerializer.Serialize(result));
            mockRepository.Verify(repo => repo.GetById(_testId), Times.Once);
        }
        #endregion

        #region Insert
        [Fact]
        public void GivenParkingZoneModel_WhenInsertIsCalled_ThenRepositoryIsCalledTwice()
        {
            //Arrange
            var parkingZone = new ParkingZone();

            var mockRepository = new Mock<IParkingZoneRepository>();

            mockRepository
                .Setup(repo => repo.Insert(parkingZone));
            mockRepository
                .Setup(repo => repo.Save());

            var service = new ParkingZoneService(mockRepository.Object);

            //Act
            service.Insert(parkingZone);

            //Assert
            mockRepository.Verify(repo => repo.Insert(parkingZone), Times.Once);
            mockRepository.Verify(repo => repo.Save(), Times.Once);
        }
        #endregion

        #region Update
        [Fact]
        public void GivenParkingZoneModel_WhenUpdateIsCalled_ThenRepositoryIsCalledTwice()
        {
            //Arrange
            var mockRepository = new Mock<IParkingZoneRepository>();

            mockRepository
                .Setup(repo => repo.Update(_testParkingZone));
            mockRepository
                .Setup(repo => repo.Save());

            var service = new ParkingZoneService(mockRepository.Object);

            //Act
            service.Update(_testParkingZone);

            //Assert
            mockRepository.Verify(repo => repo.Update(_testParkingZone), Times.Once);
            mockRepository.Verify(repo => repo.Save(), Times.Once);
        }
        #endregion

        #region Delete
        [Fact]
        public void GivenParkingZoneModel_WhenDeleteIsCalled_ThenRepositoryIsCalledTwice()
        {
            //Arrange
            var mockRepository = new Mock<IParkingZoneRepository>();
            mockRepository
                .Setup(repo => repo.Delete(_testParkingZone));
            mockRepository
                .Setup(repo => repo.Save());

            var service = new ParkingZoneService(mockRepository.Object);

            //Act
            service.Delete(_testParkingZone);

            //Assert
            mockRepository.Verify(repo => repo.Delete(_testParkingZone), Times.Once);
            mockRepository.Verify(repo => repo.Save(), Times.Once);
        }
        #endregion

        #region GetCurrentCarsPlateNumbersByZone
        [Fact]
        public void GivenZoneModel_WhenGetCurrentCarsPlateNumbersByZoneIsCalled_ThenReturnedPlateNumbersOfOngoingReservations()
        {
            //Arrange
            var _testParkingZone = new ParkingZone()
            {
                ParkingSlots = new List<ParkingSlot>
                {
                    new()
                    {
                        Reservations = new List<Reservation>
                        {
                            new()
                            {
                                StartTime = DateTime.Now.AddHours(-1),
                                Duration = 2,
                                VehicleNumber = "888AAA"
                            },
                            new()
                            {
                                StartTime = DateTime.Now.AddHours(-5),
                                Duration = 2,
                                VehicleNumber = "P369UA"
                            }
                        }
                    },
                    new()
                    {
                        Reservations = new List<Reservation>()
                        {
                            new()
                            {
                                StartTime = DateTime.Now.AddHours(-2),
                                Duration = 3,
                                VehicleNumber = "B443LA"
                            }
                        }
                    }
                }
            };
            var expectedPlateNumbers = new List<string>()
            {
                "888AAA", "B443LA"
            };

            var mockRepository = new Mock<IParkingZoneRepository>();
            var service = new ParkingZoneService(mockRepository.Object);

            //Act
            var result = service.GetCurrentCarsPlateNumbersByZone(_testParkingZone);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedPlateNumbers), JsonSerializer.Serialize(result));
        }
        #endregion

        #region GetZoneFinanceDataByPeriod
        [Fact]
        public void GivenStartInclusiveEndExclusiveAndZoneWithNotAllCategory_WhenGetZoneFinanceDataByPeriodIsCalled_ThenReturnedZeroForNotExistingCategory()
        {
            //Arrange
            var zone = new ParkingZone()
            {
                ParkingSlots = new List<ParkingSlot>()
                {
                    new()
                    {
                        Category = SlotCategoryEnum.Standard,
                        Reservations = new Collection<Reservation>()
                        {
                            new()
                            {
                                StartTime = DateTime.Now.AddDays(-30),
                                Duration = 10
                            }
                        }
                    },
                    new()
                    {
                        Category = SlotCategoryEnum.Standard,
                        Reservations = new Collection<Reservation>()
                        {
                            new()
                            {
                                StartTime = DateTime.Now.AddDays(-20),
                                Duration = 5
                            }
                        }
                    }
                }
            };

            var expectedZoneData = new ZoneFinanceData()
            {
                CategoryHours = new Dictionary<SlotCategoryEnum, int>()
                {
                    { SlotCategoryEnum.Standard, 15 },
                    { SlotCategoryEnum.Business, 0 },
                }
            };

            var service = new ParkingZoneService(Mock.Of<IParkingZoneRepository>());

            //Act
            var result = service.GetZoneFinanceDataByPeriod(DateTime.MinValue, DateTime.MaxValue, zone);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedZoneData), JsonSerializer.Serialize(result));
        }

        [Theory]
        [MemberData(nameof(GetData), parameters: 5)]
        public void GivenStartInclusiveEndExclusiveAndZone_WhenGetZoneFinanceDataByPeriodIsCalled_ThenReturnedZoneFinanceData(DateTime startInclusive, DateTime endExclusive, ZoneFinanceData expectedZoneData)
        {
            //Arrange
            var zone = GetTestZone();

            var service = new ParkingZoneService(Mock.Of<IParkingZoneRepository>());

            //Act
            var result = service.GetZoneFinanceDataByPeriod(startInclusive, endExclusive, zone);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedZoneData), JsonSerializer.Serialize(result));
        }

        public static IEnumerable<object[]> GetData(int numTests)
        {
            var expectedZoneDataForAll = new ZoneFinanceData()
            {
                CategoryHours = new Dictionary<SlotCategoryEnum, int>()
                {
                    { SlotCategoryEnum.Standard, 140 },
                    { SlotCategoryEnum.Business, 140 }
                }
            };
            var expectedZoneDataForMonth = new ZoneFinanceData()
            {
                CategoryHours = new Dictionary<SlotCategoryEnum, int>()
                {
                    { SlotCategoryEnum.Standard, 40 },
                    { SlotCategoryEnum.Business, 40 }
                }
            };
            var expectedZoneDataForWeek = new ZoneFinanceData()
            {
                CategoryHours = new Dictionary<SlotCategoryEnum, int>()
                {
                    { SlotCategoryEnum.Standard, 10 },
                    { SlotCategoryEnum.Business, 10 }
                }
            };
            var expectedZoneDataForYesterday = new ZoneFinanceData()
            {
                CategoryHours = new Dictionary<SlotCategoryEnum, int>()
                {
                    { SlotCategoryEnum.Standard, 2 },
                    { SlotCategoryEnum.Business, 2 }
                }
            };
            var expectedZoneDataForToday = new ZoneFinanceData()
            {
                CategoryHours = new Dictionary<SlotCategoryEnum, int>()
                {
                    { SlotCategoryEnum.Standard, 1 },
                    { SlotCategoryEnum.Business, 1 }
                }
            };

            var now = DateTime.Now.Date;
            var allData = new List<object[]>
            {
                new object[] { DateTime.MinValue.Date, now.AddDays(1), expectedZoneDataForAll},
                new object[] { now.AddDays(-29), now.AddDays(1), expectedZoneDataForMonth},
                new object[] { now.AddDays(-6), now.AddDays(1), expectedZoneDataForWeek},
                new object[] { now.AddDays(-1), now, expectedZoneDataForYesterday},
                new object[] { now, now.AddDays(1), expectedZoneDataForToday},
            };

            return allData;
        }

        public ParkingZone GetTestZone()
        {
            return new ParkingZone()
            {
                ParkingSlots = new Collection<ParkingSlot>()
                {
                        new()
                        {
                            Category = SlotCategoryEnum.Standard,
                            Reservations = new Collection<Reservation>()
                            {
                               new()
                               {
                                  StartTime = DateTime.Now.AddDays(-270),
                                  Duration = 100
                               }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Business,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-270),
                                    Duration = 100
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Standard,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-27),
                                    Duration = 30
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Business,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-27),
                                    Duration = 30
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Standard,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-3),
                                    Duration = 7
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Business,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-3),
                                    Duration = 7
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Standard,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-1),
                                    Duration = 2
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Business,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now.AddDays(-1),
                                    Duration = 2
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Standard,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now,
                                    Duration = 1
                                }
                            }
                        },
                        new()
                        {
                            Category = SlotCategoryEnum.Business,
                            Reservations = new Collection<Reservation>()
                            {
                                new()
                                {
                                    StartTime = DateTime.Now,
                                    Duration = 1
                                }
                            }
                        }
                    }
            };
        }
        #endregion
    }
}
