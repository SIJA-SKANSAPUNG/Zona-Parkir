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
            Reservations = new List<Reservation>()
            {
                new()
            }
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
            var expectedCreateSlotVM = new ParkingSlotCreateVM()
            {
                ParkingZoneName = "Sharafshon",
                ParkingZoneId = _testZoneId
            };

            mockZoneService
                .Setup(service => service.GetById(_testZoneId))
                .Returns(_testZone);

            //Act
            var result = controller.Create(_testZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull((result as ViewResult).Model);
            Assert.IsAssignableFrom<ParkingSlotCreateVM>((result as ViewResult).Model);
            Assert.Equal(JsonSerializer.Serialize(expectedCreateSlotVM), JsonSerializer.Serialize((result as ViewResult).Model));
            mockZoneService.Verify(s => s.GetById(_testZoneId), Times.Once);
        }

        [Fact]
        public void GivenNullZoneId_WhenGetCreateIsCalled_ThenZoneServiceIsCalledOnceAndReturnedBadRequest()
        {
            //Arrange
            mockZoneService
                .Setup(service => service.GetById(_testZoneId));

            //Act
            var result = controller.Create(_testZoneId);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            mockZoneService.Verify(service => service.GetById(_testZoneId), Times.Once);
        }

        [Fact]
        public void GivenValidModel_WhenPostCreateIsCalled_ThenParkingSlotServiceIsCalledOnceAndRedirectedToIndexView()
        {
            //Arrange
            var slotCreateVM = new ParkingSlotCreateVM()
            {
                Number = 1,
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true
            };

            mockSlotService
                .Setup(service => service.Insert(It.IsAny<ParkingSlot>()));

            mockZoneService
                .Setup(service => service.GetById(_testZoneId))
                .Returns(_testZone);

            //Act
            var result = controller.Create(slotCreateVM);

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
            var slotCreateVM = new ParkingSlotCreateVM()
            {
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true
            };

            controller.ModelState.AddModelError("Number", "Number of Slot is Required");

            //Act
            var result = controller.Create(slotCreateVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(slotCreateVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenModelWithAlreadyExistingNumber_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var slotCreateVM = new ParkingSlotCreateVM()
            {
                Number = 1,
                ParkingZoneId = _testZoneId,
                IsAvailableForBooking = true
            };

            mockSlotService
                .Setup(service => service.SlotExistsWithThisNumber(slotCreateVM.Number, null, _testZoneId))
                .Returns(true);

            //Act
            var result = controller.Create(slotCreateVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            mockSlotService.Verify(service => service.SlotExistsWithThisNumber(slotCreateVM.Number, null, _testZoneId), Times.Once);
        }

        [Fact]
        public void GivenModelWithNullZoneId_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var slotCreateVM = new ParkingSlotCreateVM()
            {
                Number = 1,
                IsAvailableForBooking = true
            };

            controller.ModelState.AddModelError("ParkingZoneId", "Parking Zone is Required");

            //Act
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void GivenModelWithNullCategory_WhenPostCreateIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var slotCreateVM = new ParkingSlotCreateVM()
            {
                Number = 1,
                ParkingZoneId = _testZoneId,
                IsAvailableForBooking = true
            };

            controller.ModelState.AddModelError("Category", "Category of Slot is Required");

            //Act
            var result = controller.Create(slotCreateVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(slotCreateVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenValidModelWithNotExistingZoneId__WhenPostCreateIsCalled_ThenServiceIsCalledOnceAndReturnedBadRequest()
        {
            //Arrange
            var slotVM = new ParkingSlotCreateVM()
            {
                ParkingZoneId = _testZoneId
            };

            mockZoneService
                .Setup(service => service.GetById(_testZoneId));

            //Act
            var result = controller.Create(slotVM);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockZoneService.Verify(service => service.GetById(_testZoneId), Times.Once());
        }
        #endregion

        #region Edit
        [Fact]
        public void GivenIdOfNotExistingSlot_WhenEditIsCalled_ThenServiceIsCalledOnceAndReturnedNotFoundResult()
        {
            //Arrange

            //Act
            var result = controller.Edit(_testSlotId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenId_WhenGetEditIsCalled_ThenNotEmptyViewResultIsReturned()
        {
            //Arrange
            var expextedParkingSlotEditVm = new ParkingSlotEditVM()
            {
                Id = _testSlotId,
                Number = 1,
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true,
                ParkingZoneName = "Sharafshon"
            };

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            //Act
            var result = controller.Edit(_testSlotId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ParkingSlotEditVM>((result as ViewResult).Model);

            var model = (result as ViewResult).Model as ParkingSlotEditVM;
            Assert.Equal(JsonSerializer.Serialize(expextedParkingSlotEditVm), JsonSerializer.Serialize(model));
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenIdAndModelWithDifferentId_WhenPostEditIsCalled_ThenReturnedNotFoundResult()
        {
            //Arrange
            var parkingSlotEditVM = new ParkingSlotEditVM()
            {
                Id = Guid.NewGuid()
            };

            //Act
            var result = controller.Edit(_testSlotId, parkingSlotEditVM);

            //Assert
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void GivenIdOfNotExistingSlotAndValidModel_WhenEditIsCalled_ThenReturnedNotFoundResultAndSlotServiceCalledOnce()
        {
            //Arrange
            var parkingSlotEditVM = new ParkingSlotEditVM()
            {
                Id = _testSlotId,
                Number = 22,
                Category = SlotCategoryEnum.Standard,
                IsAvailableForBooking = true,
                ParkingZoneName = "Sharafshon"
            };

            //Act
            var result = controller.Edit(_testSlotId, parkingSlotEditVM);

            //Assert
            Assert.True(result is NotFoundResult);
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenIdAndModelWithNullNumber_WhenPostEditIsCalled_ThenEditViewIsReturnedAndModelStateIsInValid()
        {
            //Arrange
            var parkingSlotEditVM = new ParkingSlotEditVM()
            {
                Id = _testSlotId,
                Category = SlotCategoryEnum.Standard,
                IsAvailableForBooking = true,
                ParkingZoneName = "Sharafshon"
            };

            controller.ModelState.AddModelError("Number", "Number is Required");

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            //Act
            var result = controller.Edit(_testSlotId, parkingSlotEditVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(JsonSerializer.Serialize(parkingSlotEditVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenIdAndModelWithNullCategory_WhenPostEditIsCalled_ThenEditViewIsReturnedAndModelStateIsInValid()
        {
            //Arrange
            var parkingSlotEditVM = new ParkingSlotEditVM()
            {
                Id = _testSlotId,
                Number = 22,
                IsAvailableForBooking = true,
                ParkingZoneName = "Sharafshon"
            };

            controller.ModelState.AddModelError("Category", "Category is Required");

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            //Act
            var result = controller.Edit(_testSlotId, parkingSlotEditVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(JsonSerializer.Serialize(parkingSlotEditVM), JsonSerializer.Serialize((result as ViewResult).Model));
        }

        [Fact]
        public void GivenIdAndValidModel_WhenPostEditIsCalled_ThenServiceIsCalledTwiceAndRedirectedToIndexView()
        {
            //Arrange
            var parkingSlotEditVM = new ParkingSlotEditVM()
            {
                Id = _testSlotId,
                Number = 1,
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true,
            };

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            mockSlotService
                .Setup(service => service.Update(It.IsAny<ParkingSlot>()));

            //Act
            var result = controller.Edit(_testSlotId, parkingSlotEditVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.Equal("ParkingSlot", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
            mockSlotService.Verify(s => s.Update(It.IsAny<ParkingSlot>()), Times.Once);
        }

        [Fact]
        public void GivenModelWithAlreadyExistingNumber_WhenPostEditIsCalled_ThenCreateViewReturnedAndModelStateIsInvalid()
        {
            //Arrange
            var slotVM = new ParkingSlotEditVM()
            {
                Id = _testSlotId,
                Number = 1,
                ParkingZoneId = _testZoneId,
                IsAvailableForBooking = true
            };

            mockSlotService
                .Setup(service => service.SlotExistsWithThisNumber(slotVM.Number, _testSlotId, _testZoneId))
                .Returns(true);
            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            //Act
            var result = controller.Edit(_testSlotId, slotVM);

            //Assert
            Assert.False(controller.ModelState.IsValid);
            mockSlotService.Verify(service => service.SlotExistsWithThisNumber(slotVM.Number, _testSlotId, slotVM.ParkingZoneId), Times.Once);
        }
        #endregion

        #region Details
        [Fact]
        public void GivenSlotId_WhenDetailsIsCalled_ThenServiceIsCalledOnceAndReturnedViewResultWithDetailsParkingSlotVmModel()
        {
            //Arrange
            var expectedParkingSlotDetailsVM = new ParkingSlotDetailsVM()
            {
                Id = _testSlotId,
                Number = 1,
                ParkingZoneName = "Sharafshon",
                ParkingZoneId = _testZoneId,
                Category = SlotCategoryEnum.Business,
                IsAvailableForBooking = true,
            };

            mockSlotService
                .Setup(service => service.GetById(_testSlotId)).Returns(_testSlot);

            //Act
            var result = controller.Details(_testSlotId);

            //Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ParkingSlotDetailsVM>((result as ViewResult).Model);
            Assert.Equal(JsonSerializer.Serialize(expectedParkingSlotDetailsVM), JsonSerializer.Serialize(model));
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenIdOfNotExistingParkingSlot_WhenDetailsIsCalled_ThenServiceIsCalledOnceAndReturnedNotFoundResult()
        {
            //Arrange

            //Act
            var result = controller.Details(_testSlotId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }
        #endregion

        #region Delete
        [Fact]
        public void GivenIdOfNotExistingSlot_WhenGetDeleteIsCalled_ThenReturnedNotFoundResultAndServiceIsCalledOnce()
        {
            //Arrange

            //Act
            var result = controller.Delete(_testSlotId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenId_WhenGetDeleteIsCalled_ThenNotEmptyViewResultIsReturned()
        {
            //Arrange
            var expectedSlotVM = new ParkingSlotDetailsVM()
            {
                Id = _testSlotId,
                Number = 1,
                IsAvailableForBooking = true,
                Category = SlotCategoryEnum.Business,
                ParkingZoneId = _testZoneId,
                ParkingZoneName = "Sharafshon"
            };

            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            //Act
            var result = controller.Delete(_testSlotId);

            //Assert
            Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ParkingSlotDetailsVM>((result as ViewResult).Model);
            Assert.Equal(JsonSerializer.Serialize(model), JsonSerializer.Serialize(expectedSlotVM));
        }

        [Fact]
        public void GivenIdOfNotExistingSlot_WhenDeleteConfirmedIsCalled_ThenNotFoundResultIsReturnedAndServiceIsCalledOnce()
        {
            //Arrange

            //Act
            var result = controller.DeleteConfirmed(_testSlotId, _testZoneId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
        }

        [Fact]
        public void GivenIds_WhenDeleteConfirmedIsCalled_ThenSlotServiceIsCalledTwiceAndRedirectedToIndexView()
        {
            //Arrange
            mockSlotService
                .Setup(service => service.GetById(_testSlotId))
                .Returns(_testSlot);

            mockSlotService
                .Setup(service => service.Delete(It.IsAny<ParkingSlot>()));

            //Act
            var result = controller.DeleteConfirmed(_testSlotId, _testZoneId);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;

            Assert.Equal("ParkingSlot", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            mockSlotService.Verify(s => s.GetById(_testSlotId), Times.Once);
            mockSlotService.Verify(s => s.Delete(It.IsAny<ParkingSlot>()), Times.Once);
        }
        #endregion
    }
}
