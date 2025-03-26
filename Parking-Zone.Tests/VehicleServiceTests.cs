using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Parking_Zone.Data;
using Parking_Zone.Services;
using Parking_Zone.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Parking_Zone.Tests
{
    public class VehicleServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<ILogger<VehicleService>> _mockLogger;
        private readonly VehicleService _service;

        public VehicleServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockLogger = new Mock<ILogger<VehicleService>>();
            _service = new VehicleService(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RecordEntry_WithValidInput_CreatesVehicle()
        {
            // Arrange
            var plateNumber = "AB123CD";
            var vehicleType = "car";
            var photoEntry = new byte[] { 1, 2, 3 };
            var parkingZoneId = Guid.NewGuid();

            var mockDbSet = new Mock<DbSet<Vehicle>>();
            _mockContext.Setup(c => c.Vehicles)
                       .Returns(mockDbSet.Object);

            // Act
            var result = await _service.RecordEntry(plateNumber, vehicleType, photoEntry, parkingZoneId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(plateNumber, result.PlateNumber);
            Assert.Equal(vehicleType, result.VehicleType);
            Assert.True(result.IsInside);
            mockDbSet.Verify(d => d.Add(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public async Task RecordExit_WithValidInput_UpdatesVehicle()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var photoExit = new byte[] { 1, 2, 3 };
            var vehicle = new Vehicle
            {
                Id = vehicleId,
                PlateNumber = "AB123CD",
                IsInside = true
            };

            var mockDbSet = new Mock<DbSet<Vehicle>>();
            mockDbSet.Setup(d => d.FindAsync(vehicleId))
                    .ReturnsAsync(vehicle);

            _mockContext.Setup(c => c.Vehicles)
                       .Returns(mockDbSet.Object);

            // Act
            var result = await _service.RecordExit(vehicleId, photoExit);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsInside);
            Assert.NotNull(result.ExitTime);
            Assert.Equal(photoExit, result.PhotoExit);
        }

        [Fact]
        public async Task IsVehicleInside_WithInsideVehicle_ReturnsTrue()
        {
            // Arrange
            var plateNumber = "AB123CD";
            var vehicles = new List<Vehicle>
            {
                new Vehicle { PlateNumber = plateNumber, IsInside = true }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Vehicle>>();
            mockDbSet.As<IQueryable<Vehicle>>().Setup(m => m.Provider).Returns(vehicles.Provider);
            mockDbSet.As<IQueryable<Vehicle>>().Setup(m => m.Expression).Returns(vehicles.Expression);
            mockDbSet.As<IQueryable<Vehicle>>().Setup(m => m.ElementType).Returns(vehicles.ElementType);
            mockDbSet.As<IQueryable<Vehicle>>().Setup(m => m.GetEnumerator()).Returns(vehicles.GetEnumerator());

            _mockContext.Setup(c => c.Vehicles).Returns(mockDbSet.Object);

            // Act
            var result = await _service.IsVehicleInside(plateNumber);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GenerateTicketBarcode_ReturnsValidBarcode()
        {
            // Arrange
            var vehicle = new Vehicle();

            // Act
            var result = await _service.GenerateTicketBarcode(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("PKR", result);
            Assert.Equal(21, result.Length); // Format: PKR-YYYYMMDDHHMMSS-XXXX
        }
    }
} 