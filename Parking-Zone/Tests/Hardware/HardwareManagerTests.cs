using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Parking_Zone.Hardware;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Tests.Hardware
{
    public class HardwareManagerTests
    {
        private readonly Mock<ILogger<HardwareManager>> _mockLogger;
        private readonly HardwareManager _manager;

        public HardwareManagerTests()
        {
            _mockLogger = new Mock<ILogger<HardwareManager>>();
            _manager = new HardwareManager(_mockLogger.Object);
        }

        [Fact]
        public async Task InitializeDeviceAsync_WithValidConfig_ReturnsTrue()
        {
            // Arrange
            var config = new CameraConfiguration
            {
                DeviceId = "CAM001",
                Name = "Entry Gate Camera",
                Type = "IP Camera",
                Model = "AXIS P1448-LE",
                Port = "COM1",
                BaudRate = 9600,
                Resolution = "1920x1080",
                Format = "JPEG"
            };

            // Act
            var result = await _manager.InitializeDeviceAsync(config);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InitializeDeviceAsync_WithNullPort_LogsWarningAndReturnsFalse()
        {
            // Arrange
            var config = new CameraConfiguration
            {
                DeviceId = "CAM001",
                Name = "Entry Gate Camera",
                Type = "IP Camera",
                Model = "AXIS P1448-LE",
                Port = null,
                BaudRate = 9600
            };

            // Act
            var result = await _manager.InitializeDeviceAsync(config);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Port not specified")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task SendCommandAsync_WithValidDevice_ReturnsTrue()
        {
            // Arrange
            var deviceId = "CAM001";
            var command = "CAPTURE";

            // First initialize the device
            var config = new CameraConfiguration
            {
                DeviceId = deviceId,
                Name = "Entry Gate Camera",
                Type = "IP Camera",
                Model = "AXIS P1448-LE",
                Port = "COM1",
                BaudRate = 9600
            };
            await _manager.InitializeDeviceAsync(config);

            // Act
            var result = await _manager.SendCommandAsync(deviceId, command);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendCommandAsync_WithNonExistentDevice_ReturnsFalse()
        {
            // Arrange
            var deviceId = "NONEXISTENT";
            var command = "CAPTURE";

            // Act
            var result = await _manager.SendCommandAsync(deviceId, command);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Device not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ReadResponseAsync_WithValidDevice_ReturnsResponse()
        {
            // Arrange
            var deviceId = "CAM001";
            
            // First initialize the device
            var config = new CameraConfiguration
            {
                DeviceId = deviceId,
                Name = "Entry Gate Camera",
                Type = "IP Camera",
                Model = "AXIS P1448-LE",
                Port = "COM1",
                BaudRate = 9600
            };
            await _manager.InitializeDeviceAsync(config);

            // Act
            var response = await _manager.ReadResponseAsync(deviceId);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public async Task IsDeviceOperationalAsync_WithInitializedDevice_ReturnsTrue()
        {
            // Arrange
            var deviceId = "CAM001";
            
            // First initialize the device
            var config = new CameraConfiguration
            {
                DeviceId = deviceId,
                Name = "Entry Gate Camera",
                Type = "IP Camera",
                Model = "AXIS P1448-LE",
                Port = "COM1",
                BaudRate = 9600
            };
            await _manager.InitializeDeviceAsync(config);

            // Act
            var result = await _manager.IsDeviceOperationalAsync(deviceId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DisconnectDeviceAsync_WithConnectedDevice_ReturnsTrue()
        {
            // Arrange
            var deviceId = "CAM001";
            
            // First initialize the device
            var config = new CameraConfiguration
            {
                DeviceId = deviceId,
                Name = "Entry Gate Camera",
                Type = "IP Camera",
                Model = "AXIS P1448-LE",
                Port = "COM1",
                BaudRate = 9600
            };
            await _manager.InitializeDeviceAsync(config);

            // Act
            var result = await _manager.DisconnectDeviceAsync(deviceId);

            // Assert
            Assert.True(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Device disconnected")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
} 