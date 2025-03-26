using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Parking_Zone.Data;
using Parking_Zone.Services;
using Parking_Zone.Models;
using Microsoft.EntityFrameworkCore;

namespace Parking_Zone.Tests
{
    public class ParkingFeeServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<ILogger<ParkingFeeService>> _mockLogger;
        private readonly ParkingFeeService _service;

        public ParkingFeeServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockLogger = new Mock<ILogger<ParkingFeeService>>();
            _service = new ParkingFeeService(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CalculateFee_WithValidInput_ReturnsCorrectFee()
        {
            // Arrange
            var entryTime = DateTime.UtcNow.AddHours(-2);
            var exitTime = DateTime.UtcNow;
            var vehicleType = "car";
            var parkingZoneId = Guid.NewGuid();

            var mockFeeConfig = new FeeConfiguration
            {
                Id = Guid.NewGuid(),
                VehicleType = vehicleType,
                BaseFee = 5000m,
                ParkingZoneId = parkingZoneId
            };

            var mockDbSet = new Mock<DbSet<FeeConfiguration>>();
            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync(mockFeeConfig);

            _mockContext.Setup(c => c.FeeConfigurations)
                       .Returns(mockDbSet.Object);

            // Act
            var result = await _service.CalculateFee(entryTime, exitTime, vehicleType, parkingZoneId);

            // Assert
            Assert.Equal(10000m, result); // 2 hours * 5000 base fee
        }

        [Fact]
        public async Task GetBaseFee_WithExistingConfig_ReturnsConfiguredFee()
        {
            // Arrange
            var vehicleType = "car";
            var parkingZoneId = Guid.NewGuid();
            var expectedFee = 5000m;

            var mockFeeConfig = new FeeConfiguration
            {
                Id = Guid.NewGuid(),
                VehicleType = vehicleType,
                BaseFee = expectedFee,
                ParkingZoneId = parkingZoneId
            };

            var mockDbSet = new Mock<DbSet<FeeConfiguration>>();
            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync(mockFeeConfig);

            _mockContext.Setup(c => c.FeeConfigurations)
                       .Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetBaseFee(vehicleType, parkingZoneId);

            // Assert
            Assert.Equal(expectedFee, result);
        }

        [Fact]
        public async Task GetBaseFee_WithNonExistingConfig_ReturnsDefaultFee()
        {
            // Arrange
            var vehicleType = "car";
            var parkingZoneId = Guid.NewGuid();

            var mockDbSet = new Mock<DbSet<FeeConfiguration>>();
            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                    .ReturnsAsync((FeeConfiguration)null);

            _mockContext.Setup(c => c.FeeConfigurations)
                       .Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetBaseFee(vehicleType, parkingZoneId);

            // Assert
            Assert.Equal(5000m, result); // Default fee for car
        }
    }
} 