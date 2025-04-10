﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using Parking_Zone.Areas.Admin;
using Parking_Zone.Areas.Admin.Controllers;
using Parking_Zone.Controllers;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingZone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Controllers.Admin
{
    public class ParkingZoneControllerTests
    {
        private readonly Guid _testId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");
        private readonly Mock<IParkingZoneService> mockZoneService;
        private readonly ParkingZoneController controller;

        public ParkingZoneControllerTests()
        {
            mockZoneService = new Mock<IParkingZoneService>();
            controller = new ParkingZoneController(mockZoneService.Object);
        }

        private readonly ParkingZone _testParkingZone = new ParkingZone()
        {
            Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
            Name = "Sharafshon",
            Address = "Andijon",
            Description = "Arzon",
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
                            StartTime = DateTime.Now.AddHours(-2),
                            Duration = 3,
                            VehicleNumber = "B443LA"
                        },
                        new()
                        {
                            StartTime = DateTime.Now.AddHours(-5),
                            Duration = 2,
                            VehicleNumber = "P369UA"
                        }
                    }
                }
            }
        };

        #region Index
        [Fact]
        public void GivenNothing_WhenIndexIsCalled_ThenServiceIsCalledOnceAndReturnedNotEmptyViewResult()
        {
            //Arrange
            mockZoneService
                .Setup(service => service.GetAll());

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull((result as ViewResult).Model);
            mockZoneService.Verify(service => service.GetAll(), Times.Once);
        }
        #endregion

        #region Details
        [Fact]
        public void GivenId_WhenGetDetailsIsCalled_ThenServiceIsCalledOnceAndReturnedViewResultWithDetailsParkingZoneVmModel()
        {
            //Arrange
            var expectedParkingZoneDetailsVM = new ParkingZoneDetailsVM()
            {
                Id = _testId,
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Arzon"
            };

            mockZoneService
                .Setup(service => service.GetById(_testId))
                .Returns(_testParkingZone);

            //Act
            var result = controller.Details(_testId);

            //Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ParkingZoneDetailsVM>((result as ViewResult).Model);
            Assert.Equal(JsonSerializer.Serialize(expectedParkingZoneDetailsVM), JsonSerializer.Serialize(model));
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenIdOfNotExistingParkingZone_WhenGetDetailsIsCalled_ThenServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange

            //Act
            var result = controller.Details(_testId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }
        #endregion

        #region Create
        [Fact]
        public void GivenNothing_WhenGetCreateIsCalled_ThenEmptyViewResultIsReturned()
        {
            //Arrange
            var controller = new ParkingZoneController(mockZoneService.Object);

            //Act
            var result = controller.Create();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.Null((result as ViewResult).Model);
        }

        [Fact]
        public void GivenValidModel_WhenPostCreateIsCalled_ThenServiceIsCalledOnceAndRedirectedToIndexView()
        {
            //Arrange
            var parkingZoneVM = new ParkingZoneCreateVM()
            {
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Qimmat"
            };

            mockZoneService
                .Setup(service => service.Insert(It.IsAny<ParkingZone>()));

            //Act
            var result = controller.Create(parkingZoneVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockZoneService.Verify(service => service.Insert(It.IsAny<ParkingZone>()), Times.Once);
        }

        [Fact]
        public void GivenModelWithNullName_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var parkingZoneVM = new ParkingZoneCreateVM()
            {
                Address = "Andijon",
                Description = "Qimmat"
            };

            controller.ModelState.AddModelError("Name", "Name is required");

            //Act
            var result = controller.Create(parkingZoneVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(parkingZoneVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenModelWithNullAddress_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var parkingZoneVM = new ParkingZoneCreateVM()
            {
                Name = "Sharafshon",
                Description = "Qimmat"
            };

            controller.ModelState.AddModelError("Address", "Address is required");

            //Act
            var result = controller.Create(parkingZoneVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(parkingZoneVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }
        #endregion

        #region Edit
        [Fact]
        public void GivenIdOfNotExistingParkingZone_WhenEditIsCalled_ThenReturnedNotFoundResultAndServiceIsCalledOnce()
        {
            //Arrange

            //Act
            var result = controller.Edit(_testId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenId_WhenGetEditIsCalled_ThenNotEmptyViewResultIsReturned()
        {
            //Arrange
            var expectedParkingZoneEditVM = new ParkingZoneEditVM()
            {
                Id = _testId,
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Arzon"
            };

            mockZoneService
                .Setup(service => service.GetById(_testId))
                .Returns(_testParkingZone);

            //Act
            var result = controller.Edit(_testId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParkingZoneEditVM>((result as ViewResult).Model);

            var model = (result as ViewResult).Model as ParkingZoneEditVM;

            Assert.Equal(JsonSerializer.Serialize(expectedParkingZoneEditVM), JsonSerializer.Serialize(model));
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenIdAndModelWithDifferentId_WhenPostEditIsCalled_ThenReturnedNotFoundResult()
        {
            //Arrange
            var parkingZoneEditVM = new ParkingZoneEditVM()
            {
                Id = Guid.NewGuid()
            };

            //Act
            var result = controller.Edit(_testId, parkingZoneEditVM);

            //Assert
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void GivenIdOfNotExistingParkingZoneAndValidModel_WhenPostEditIsCalled_ThenReturnedNotFoundAndServiceIsCalledOnce()
        {
            //Arrange
            var parkingZoneEditVM = new ParkingZoneEditVM()
            {
                Id = _testId,
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Arzon"
            };

            //Act
            var result = controller.Edit(_testId, parkingZoneEditVM);

            //Asert
            Assert.True(result is NotFoundResult);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenIdAndModelWithNullName_WhenPostEditIsCalled_ThenEditViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var parkingZoneEditVM = new ParkingZoneEditVM()
            {
                Id = _testId,
                Name = null,
                Address = "Andijon",
                Description = "Arzon"
            };

            controller.ModelState.AddModelError("Name", "Name is required");

            //Act
            var result = controller.Edit(_testId, parkingZoneEditVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(JsonSerializer.Serialize(parkingZoneEditVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenIdAndModelWithNullAddress_WhenPostEditIsCalled_ThenEditViewReturnedAndModelStateIsInvelid()
        {
            //Arrange
            var _testId = new Guid("dd09a090-b0f6-4369-b24a-656843d227bc");

            var pakingZoneEditVM = new ParkingZoneEditVM()
            {
                Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Arzon"
            };

            var mockService = new Mock<IParkingZoneService>();

            controller.ModelState.AddModelError("Address", "Address is required");

            //Act
            var result = controller.Edit(_testId, pakingZoneEditVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(JsonSerializer.Serialize(pakingZoneEditVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenIdAndValidModel_WhenPostEditIsCalled_ThenServiceIsCalledTwiceAndRedirectedToIndexView()
        {
            //Arrange
            var _testId = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc");

            var testParkingZone = new ParkingZoneEditVM()
            {
                Id = Guid.Parse("dd09a090-b0f6-4369-b24a-656843d227bc"),
                Name = "Sharafshon",
                Address = "Andijon",
                Description = "Arzon"
            };

            mockZoneService
                .Setup(service => service.GetById(_testId))
                .Returns(_testParkingZone);
            mockZoneService
                .Setup(service => service.Update(It.IsAny<ParkingZone>()));

            //Act
            var result = controller.Edit(_testId, testParkingZone);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
            mockZoneService.Verify(service => service.Update(It.IsAny<ParkingZone>()), Times.Once);
        }
        #endregion

        #region Delete
        [Fact]
        public void GivenId_WhenGetDeleteIsCalledAndServiceIsReturnedNull_ThenReturnedNotFoundResultAndServiceIsCalledOnce()
        {
            //Arrange
            mockZoneService
                .Setup(service => service.GetById(_testId));

            //Act
            var result = controller.Delete(_testId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenId_WhenGetDeleteIsCalled_ThenNotEmptyViewResultIsReturned()
        {
            //Arrange
            string testName = "Sharafshon";
            string testAdress = "Andijon";
            string testDescription = "Arzon";

            mockZoneService
                .Setup(service => service.GetById(_testId))
                .Returns(_testParkingZone);

            //Act
            var result = controller.Delete(_testId);

            //Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ParkingZone>((result as ViewResult).Model);
            Assert.Equal(_testId, model.Id);
            Assert.Equal(testName, model.Name);
            Assert.Equal(testAdress, model.Address);
            Assert.Equal(testDescription, model.Description);
        }

        [Fact]
        public void GivenIdOfExistingZone_WhenPostDeleteConfirmedIsCalled_ThenReturnedNotFoundServiceIsCalledOnce()
        {
            //Arrange

            //Act
            var result = controller.DeleteConfirmed(_testId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once());
        }

        [Fact]
        public void GivenId_WhenPostDeleteConfirmedIsCalled_ThenServiceCalledTwiceAndRedirectedToIndexView()
        {
            //Arrange
            mockZoneService
                .Setup(service => service.GetById(_testId))
                .Returns(_testParkingZone);

            mockZoneService
                .Setup(service => service.Delete(It.IsAny<ParkingZone>()));

            //Act
            var result = controller.DeleteConfirmed(_testId);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Null(redirectToActionResult.ControllerName);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
            mockZoneService.Verify(service => service.Delete(It.IsAny<ParkingZone>()), Times.Once);
        }
        #endregion

        #region GetCurrentCars
        [Fact]
        public void GivenIdOfNotExistingZone_WhenGetCurrentCarsIsCalled_ThenReturnedNotFoundResult()
        {
            //Arrange
            mockZoneService
                .Setup(service => service.GetById(_testId));

            //Act
            var result = controller.CurrentCars(_testId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenIdOfExistingZone_WhenGetCurrentCarsIsCalled_ThenReturnedPlateNumbersWhichAreInUseInZone()
        {
            //Arrange
            var expectedPlateNumbers = new List<string>()
            {
                "888AAA", "B443LA"
            };

            mockZoneService
                .Setup(service => service.GetById(_testId))
                .Returns(_testParkingZone);
            mockZoneService
                .Setup(service => service.GetCurrentCarsPlateNumbersByZone(_testParkingZone))
                .Returns(new List<string>() { "888AAA", "B443LA" });

            //Act
            var result = controller.CurrentCars(_testId);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedPlateNumbers), JsonSerializer.Serialize((result as ViewResult).Model));
            mockZoneService.Verify(service => service.GetById(_testId), Times.Once);
        }
        #endregion

        #region GetZoneFinanceData
        [Fact]
        public void GivenTimePeriodAndIdOfNotExistingZone_WhenFinanceSummaryIsCalled_ThenServiceIsCalledOnceAndReturnedNotFoundResult()
        {
            //Arrange
            var period = PeriodOptionsEnum.AllTime;

            mockZoneService
                .Setup(s => s.GetById(_testId));

            //Act
            var result = controller.GetZoneFinanceData(period, _testId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(s => s.GetById(_testId), Times.Once);
        }

        [Fact]
        public void GivenPeriodOptionAndZoneId_WhenGetZoneFinanceDataIsCalled_ThenServicesCalledwiceAndReturnedJsonResult()
        {
            //Arrange
            var testPeriodOption = PeriodOptionsEnum.Today;
            var _testStartInclusive = DateTime.Now.Date;
            var _testEndExclusive = DateTime.Now.Date;

            var zone = new ParkingZone()
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
                               StartTime = DateTime.Now.AddHours(-1),
                               Duration = 33,
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
                               StartTime = DateTime.Now.AddHours(1),
                               Duration = 22,
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
                               StartTime = DateTime.Now.AddHours(-30),
                               Duration = 11,
                           }
                       }
                   }
               }
            };

            var expectedZoneData = new ZoneFinanceData();

            mockZoneService
                .Setup(s => s.GetById(_testId))
                .Returns(zone);
            mockZoneService
                .Setup(s => s.GetZoneFinanceDataByPeriod(_testStartInclusive, _testEndExclusive, zone))
                .Returns(expectedZoneData);

            //Act
            var result = controller.GetZoneFinanceData(testPeriodOption, _testId);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            mockZoneService.Verify(s => s.GetById(_testId), Times.Once);
            mockZoneService.Verify(s => s.GetZoneFinanceDataByPeriod(_testStartInclusive, _testEndExclusive, zone), Times.Once);
            Assert.Equal(JsonSerializer.Serialize(expectedZoneData), JsonSerializer.Serialize(jsonResult.Value));
        }
        #endregion
    }
}
