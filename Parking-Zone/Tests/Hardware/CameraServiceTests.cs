using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Parking_Zone.Services;
using Parking_Zone.Hardware;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Tests.Hardware
{
    public class CameraServiceTests
    {
        private readonly Mock<ILogger<CameraService>> _mockLogger;
        private readonly Mock<IHardwareManager> _mockHardwareManager;
        private readonly Mock<IIPCameraService> _mockIpCameraService;

        public CameraServiceTests()
        {
            _mockLogger = new Mock<ILogger<CameraService>>();
            _mockHardwareManager = new Mock<IHardwareManager>();
            _mockIpCameraService = new Mock<IIPCameraService>();
        }

        [Fact]
        public async Task InitializeCameraAsync_WithValidConfig_ReturnsTrue()
        {
            // Arrange
            var config = new Services.Models.CameraConfiguration
            {
                CameraId = "CAM001",
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
                .Setup(m => m.InitializeDeviceAsync(It.IsAny<Services.Models.CameraConfiguration>()))
                .ReturnsAsync(true);

            _mockHardwareManager
                .Setup(m => m.ReadResponseAsync())
                .ReturnsAsync("Success");

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.InitializeCameraAsync(It.IsAny<Services.Models.CameraConfiguration>()))
                .CallBase();

            // Act
            var result = await mockCameraService.Object.InitializeCameraAsync(config);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task InitializeCameraAsync_WhenHardwareManagerFails_ReturnsFalse()
        {
            // Arrange
            var config = new Services.Models.CameraConfiguration
            {
                CameraId = "CAM001",
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
                .Setup(m => m.InitializeDeviceAsync(It.IsAny<Services.Models.CameraConfiguration>()))
                .ReturnsAsync(false);

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.InitializeCameraAsync(It.IsAny<Services.Models.CameraConfiguration>()))
                .CallBase();

            // Act
            var result = await mockCameraService.Object.InitializeCameraAsync(config);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task TakePhotoAsync_ActiveCamera_ReturnsPhotoPath()
        {
            // Arrange
            var config = new Services.Models.CameraConfiguration
            {
                CameraId = "CAM001",
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

            var expectedPhotoPath = "/path/to/photo.jpg";

            _mockIpCameraService
                .Setup(m => m.CaptureImageAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync("data:image/jpeg;base64,base64encodedimage");

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.TakePhotoAsync(It.IsAny<Services.Models.CameraConfiguration>()))
                .CallBase();

            // Act
            var result = await mockCameraService.Object.TakePhotoAsync(config);

            // Assert
            Assert.Contains("/images/cameras/", result);
        }

        [Fact]
        public async Task TakePhotoAsync_InactiveCamera_ThrowsException()
        {
            // Arrange
            var config = new Services.Models.CameraConfiguration
            {
                CameraId = "CAM001",
                Name = "Entry Gate Camera",
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

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.TakePhotoAsync(It.IsAny<Services.Models.CameraConfiguration>()))
                .CallBase();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                mockCameraService.Object.TakePhotoAsync(config));
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
                .Setup(m => m.ReadResponseAsync())
                .ReturnsAsync(expectedImagePath);

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.CaptureImageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .CallBase();

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

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.CaptureImageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .CallBase();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => 
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
                .Setup(m => m.ReadResponseAsync())
                .ReturnsAsync(string.Empty);

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.CaptureImageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .CallBase();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => 
                mockCameraService.Object.CaptureImageAsync(gateId, reason));
            Assert.Contains($"No response from camera at gate {gateId}", exception.Message);
        }

        [Fact]
        public async Task IsOperationalAsync_WithValidGateId_ReturnsOperationalStatus()
        {
            // Arrange
            var gateId = "GATE001";

            _mockHardwareManager
                .Setup(m => m.IsDeviceOperationalAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.IsOperationalAsync(It.IsAny<string>()))
                .CallBase();

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
                .Setup(m => m.IsDeviceOperationalAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Hardware error"));

            var mockCameraService = new Mock<CameraService>(
                _mockLogger.Object, 
                _mockHardwareManager.Object, 
                _mockIpCameraService.Object
            );

            mockCameraService.Setup(m => m.IsOperationalAsync(It.IsAny<string>()))
                .CallBase();

            // Act
            var result = await mockCameraService.Object.IsOperationalAsync(gateId);

            // Assert
            Assert.False(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Error checking camera status for gate {gateId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}