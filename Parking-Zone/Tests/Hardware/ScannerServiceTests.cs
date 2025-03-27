using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Parking_Zone.Services;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Tests.Hardware
{
    public class ScannerServiceTests
    {
        private readonly Mock<ILogger<ScannerService>> _mockLogger;
        private readonly ScannerService _service;

        public ScannerServiceTests()
        {
            _mockLogger = new Mock<ILogger<ScannerService>>();
            _service = new ScannerService(_mockLogger.Object);
        }

        [Fact]
        public async Task ScanBarcodeAsync_ReturnsValidBarcode()
        {
            // Act
            var result = await _service.ScanBarcodeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("MOCK_BARCODE_", result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Scanning barcode")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task IsReadyAsync_ReturnsTrue()
        {
            // Act
            var result = await _service.IsReadyAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsReadyAsync_WhenExceptionOccurs_ReturnsFalse()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ScannerService>>();
            var service = new ScannerService(mockLogger.Object);

            // Simulate an exception by making the logger throw when used
            mockLogger
                .Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ))
                .Throws(new Exception("Simulated error"));

            // Act
            var result = await service.IsReadyAsync();

            // Assert
            Assert.False(result);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error checking scanner status")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetDeviceInfoAsync_ReturnsDeviceInfo()
        {
            // Act
            var result = await _service.GetDeviceInfoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Mock Scanner Device v1.0", result);
        }

        [Fact]
        public async Task GetDeviceInfoAsync_WhenExceptionOccurs_ReturnsEmptyString()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ScannerService>>();
            var service = new ScannerService(mockLogger.Object);

            // Simulate an exception by making the logger throw when used
            mockLogger
                .Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ))
                .Throws(new Exception("Simulated error"));

            // Act
            var result = await service.GetDeviceInfoAsync();

            // Assert
            Assert.Equal(string.Empty, result);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error getting scanner device info")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ScanBarcodeAsync_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ScannerService>>();
            var service = new ScannerService(mockLogger.Object);

            // Simulate an exception by making the logger throw when used
            mockLogger
                .Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ))
                .Throws(new Exception("Simulated error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.ScanBarcodeAsync());
            Assert.Equal("Simulated error", exception.Message);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error scanning barcode")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
} 