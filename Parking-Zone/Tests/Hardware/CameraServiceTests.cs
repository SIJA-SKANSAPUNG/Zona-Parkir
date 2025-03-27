using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Parking_Zone.Hardware;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Xunit;

namespace Parking_Zone.Tests.Hardware
{
    public class CameraServiceTests
    {
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<Parking_Zone.Services.CameraService>> _mockLogger;
        private readonly Mock<Parking_Zone.Hardware.IHardwareManager> _mockHardwareManager;
        private readonly Mock<Parking_Zone.Services.IIPCameraService> _mockIpCameraService;

        public CameraServiceTests()
        {
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<Parking_Zone.Services.CameraService>>();
            _mockHardwareManager = new Mock<Parking_Zone.Hardware.IHardwareManager>();
            _mockIpCameraService = new Mock<Parking_Zone.Services.IIPCameraService>();
        }

        [Fact]
        public async Task InitializeCameraAsync_WithValidConfig_ReturnsTrue()
        {
            // Arrange
            var config = new Parking_Zone.Models.CameraConfiguration
            {
                Id = 1,
                DeviceId = "CAM001",
                Name = "Entry Gate Camera",
                Type = "IP",
                BaudRate = 9600,
                Resolution = "1920x1080",
                Format = "JPEG",
                IpAddress = "192.168.1.100",
                Port = 80,
                Username = "admin",
                Password = "password",
                IsActive = true
            };

            _mockHardwareManager
                .Setup(m => m.SendCommandAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(true);

            _mockHardwareManager
                .Setup(m => m.ReadResponseAsync(It.IsAny<string>()))
                .ReturnsAsync("Success");

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act
            var result = await mockCameraService.Object.InitializeCameraAsync(config);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InitializeCameraAsync_WhenHardwareManagerFails_ReturnsFalse()
        {
            // Arrange
            var config = new Parking_Zone.Models.CameraConfiguration
            {
                Id = 1,
                DeviceId = "CAM001",
                Name = "Entry Gate Camera",
                Type = "IP",
                BaudRate = 9600,
                Resolution = "1920x1080",
                Format = "JPEG",
                IpAddress = "192.168.1.100",
                Port = 80,
                Username = "admin",
                Password = "password",
                IsActive = true
            };

            _mockHardwareManager
                .Setup(m => m.InitializeDeviceAsync(It.IsAny<Parking_Zone.Models.CameraConfiguration>()))
                .ReturnsAsync(false);

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act
            var result = await mockCameraService.Object.InitializeCameraAsync(config);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    Microsoft.Extensions.Logging.LogLevel.Error,
                    It.IsAny<Microsoft.Extensions.Logging.EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error initializing camera")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task TakePhoto_ValidConfiguration_ReturnsImageBytes()
        {
            // Arrange
            var cameraConfig = new Parking_Zone.Models.CameraConfiguration
            {
                Id = 1,
                DeviceId = "CAM001",
                Name = "Entry Camera",
                Type = "IP",
                BaudRate = 9600,
                Resolution = "1920x1080",
                Format = "JPEG",
                IpAddress = "192.168.1.100",
                Port = 80,
                Username = "admin",
                Password = "password",
                IsActive = true
            };

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Setup mock behaviors
            _mockIpCameraService
                .Setup(x => x.CaptureImageAsync(It.IsAny<Parking_Zone.Models.CameraConfiguration>()))
                .ReturnsAsync(new byte[] { 0x01, 0x02, 0x03 });

            // Act
            var result = await mockCameraService.Object.TakePhoto(cameraConfig);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public async Task TakePhoto_InactiveCamera_ThrowsException()
        {
            // Arrange
            var cameraConfig = new Parking_Zone.Models.CameraConfiguration
            {
                Id = 1,
                DeviceId = "CAM001",
                Name = "Entry Camera",
                Type = "IP",
                BaudRate = 9600,
                Resolution = "1920x1080",
                Format = "JPEG",
                IpAddress = "192.168.1.100",
                Port = 80,
                Username = "admin",
                Password = "password",
                IsActive = false
            };

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => 
                mockCameraService.Object.TakePhoto(cameraConfig));
        }

        [Fact]
        public async Task CaptureImageAsync_WithValidGateId_ReturnsImagePath()
        {
            // Arrange
            string gateId = "GATE001";
            string reason = "ENTRY";
            string expectedImagePath = $"/images/vehicles/{gateId}_{reason}";

            _mockHardwareManager
                .Setup(m => m.SendCommandAsync(gateId, "CAPTURE", It.IsAny<object>()))
                .ReturnsAsync(true);

            _mockHardwareManager
                .Setup(m => m.ReadResponseAsync(gateId))
                .ReturnsAsync(expectedImagePath);

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act
            var result = await mockCameraService.Object.CaptureImageAsync(gateId, reason);

            // Assert
            Assert.Contains(expectedImagePath, result);
        }

        [Fact]
        public async Task CaptureImageAsync_WhenSendCommandFails_ThrowsException()
        {
            // Arrange
            var gateId = "GATE001";
            var reason = "ENTRY";

            _mockHardwareManager
                .Setup(m => m.SendCommandAsync(gateId, "CAPTURE", It.IsAny<object>()))
                .ReturnsAsync(false);

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<System.ApplicationException>(() => 
                mockCameraService.Object.CaptureImageAsync(gateId, reason));
            Assert.Contains($"Failed to send capture command to camera at gate {gateId}", exception.Message);
        }

        [Fact]
        public async Task CaptureImageAsync_WhenResponseIsEmpty_ThrowsException()
        {
            // Arrange
            var gateId = "GATE001";
            var reason = "ENTRY";

            _mockHardwareManager
                .Setup(m => m.SendCommandAsync(gateId, "CAPTURE", It.IsAny<object>()))
                .ReturnsAsync(true);

            _mockHardwareManager
                .Setup(m => m.ReadResponseAsync(gateId))
                .ReturnsAsync(string.Empty);

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<System.ApplicationException>(() => 
                mockCameraService.Object.CaptureImageAsync(gateId, reason));
            Assert.Contains($"No response from camera at gate {gateId}", exception.Message);
        }

        [Fact]
        public async Task IsOperationalAsync_WithValidGateId_ReturnsOperationalStatus()
        {
            // Arrange
            var gateId = "GATE001";

            _mockHardwareManager
                .Setup(m => m.IsDeviceOperationalAsync(gateId))
                .ReturnsAsync(true);

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act
            var result = await mockCameraService.Object.IsOperationalAsync(gateId);

            // Assert
            Assert.True(result);
            _mockHardwareManager.Verify(m => m.IsDeviceOperationalAsync(gateId), Times.Once);
        }

        [Fact]
        public async Task IsOperationalAsync_WhenHardwareManagerThrowsException_ReturnsFalse()
        {
            // Arrange
            var gateId = "GATE001";

            _mockHardwareManager
                .Setup(m => m.IsDeviceOperationalAsync(gateId))
                .ThrowsAsync(new System.Exception("Hardware error"));

            var mockCameraService = new Mock<Parking_Zone.Services.CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            // Act
            var result = await mockCameraService.Object.IsOperationalAsync(gateId);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    Microsoft.Extensions.Logging.LogLevel.Error,
                    It.IsAny<Microsoft.Extensions.Logging.EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Error checking camera status for gate {gateId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}