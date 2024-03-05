using Microsoft.AspNetCore.Mvc;
using Moq;
using Parking_Zone.Areas.Admin.Controllers;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.Services.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Controllers.Admin
{
    public class ReservationsControllerTests
    {
        private readonly Mock<IReservationService> mockReservationService;
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

        public ReservationsControllerTests()
        {
            mockReservationService = new Mock<IReservationService>();
        }

        #region Index
        [Fact]
        public void GivenAllTimePeriod_WhenIndexIsCalled_ThenReturnedNotEmptyViewResult()
        {
            //Arrange
            var period = "all_time";
            var expectedSummaryHours = new ReservationHoursSummaryVM()
            {
                StandardHours = 10,
                BusinessHours = 5
            };

            var controller = new ReservationsController(mockReservationService.Object);

            mockReservationService
                .Setup(s => s.GetAll())
                .Returns(testReservations);
            mockReservationService
                .Setup(s => s.GetStandardAndBusinessHoursByPeriod(testReservations, period))
                .Returns(expectedSummaryHours);

            //Act
            var result = controller.Index(period);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualSummaryHours = jsonResult.Value as ReservationHoursSummaryVM;

            Assert.NotNull(actualSummaryHours);
            Assert.Equal(expectedSummaryHours.StandardHours, actualSummaryHours.StandardHours);
            Assert.Equal(expectedSummaryHours.BusinessHours, actualSummaryHours.BusinessHours);
        }

        [Fact]
        public void GivenLast30DaysPeriod_WhenIndexIsCalled_ThenReturnedReservationSummaryHoursFor30Days()
        {
            //Arrange
            var period = "last_30_days";
            var expectedSummaryHours = new ReservationHoursSummaryVM()
            {
                StandardHours = 5,
                BusinessHours = 5
            };

            var controller = new ReservationsController(mockReservationService.Object);

            mockReservationService
                .Setup(s => s.GetAll())
                .Returns(testReservations);
            mockReservationService
                .Setup(s => s.GetStandardAndBusinessHoursByPeriod(testReservations, period))
                .Returns(expectedSummaryHours);

            //Act
            var result = controller.Index(period);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualSummaryHours = jsonResult.Value as ReservationHoursSummaryVM;

            Assert.NotNull(actualSummaryHours);
            Assert.Equal(expectedSummaryHours.StandardHours, actualSummaryHours.StandardHours);
            Assert.Equal(expectedSummaryHours.BusinessHours, actualSummaryHours.BusinessHours);
        }

        [Fact]
        public void GivenLast7DaysPeriod_WhenIndexIsCalled_ThenReturnedReservationSummaryHoursFor7Days()
        {
            //Arrange
            var period = "last_7_days";
            var expectedSummaryHours = new ReservationHoursSummaryVM()
            {
                StandardHours = 5,
                BusinessHours = 1
            };

            var controller = new ReservationsController(mockReservationService.Object);

            mockReservationService
                .Setup(s => s.GetAll())
                .Returns(testReservations);
            mockReservationService
                .Setup(s => s.GetStandardAndBusinessHoursByPeriod(testReservations, period))
                .Returns(expectedSummaryHours);

            //Act
            var result = controller.Index(period);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualSummaryHours = jsonResult.Value as ReservationHoursSummaryVM;

            Assert.NotNull(actualSummaryHours);
            Assert.Equal(expectedSummaryHours.StandardHours, actualSummaryHours.StandardHours);
            Assert.Equal(expectedSummaryHours.BusinessHours, actualSummaryHours.BusinessHours);
        }

        [Fact]
        public void GivenYesterdayPeriod_WhenIndexIsCalled_ThenReturnedReservationSummaryHoursForYesterday()
        {
            //Arrange
            var period = "yesterday";
            var expectedSummaryHours = new ReservationHoursSummaryVM()
            {
                StandardHours = 0,
                BusinessHours = 1
            };

            var controller = new ReservationsController(mockReservationService.Object);

            mockReservationService
                .Setup(s => s.GetAll())
                .Returns(testReservations);
            mockReservationService
                .Setup(s => s.GetStandardAndBusinessHoursByPeriod(testReservations, period))
                .Returns(expectedSummaryHours);

            //Act
            var result = controller.Index(period);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualSummaryHours = jsonResult.Value as ReservationHoursSummaryVM;

            Assert.NotNull(actualSummaryHours);
            Assert.Equal(expectedSummaryHours.StandardHours, actualSummaryHours.StandardHours);
            Assert.Equal(expectedSummaryHours.BusinessHours, actualSummaryHours.BusinessHours);
        }

        [Fact]
        public void GivenTodayPeriod_WhenIndexIsCalled_ThenReturnedReservationSummaryHoursForToday()
        {
            //Arrange
            var period = "today";
            var expectedSummaryHours = new ReservationHoursSummaryVM()
            {
                StandardHours = 3,
                BusinessHours = 0
            };

            var controller = new ReservationsController(mockReservationService.Object);

            mockReservationService
                .Setup(s => s.GetAll())
                .Returns(testReservations);
            mockReservationService
                .Setup(s => s.GetStandardAndBusinessHoursByPeriod(testReservations, period))
                .Returns(expectedSummaryHours);

            //Act
            var result = controller.Index(period);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualSummaryHours = jsonResult.Value as ReservationHoursSummaryVM;

            Assert.NotNull(actualSummaryHours);
            Assert.Equal(expectedSummaryHours.StandardHours, actualSummaryHours.StandardHours);
            Assert.Equal(expectedSummaryHours.BusinessHours, actualSummaryHours.BusinessHours);
        }
        #endregion
    }
}
