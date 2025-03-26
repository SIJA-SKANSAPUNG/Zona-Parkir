using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Parking_Zone.Tests
{
    public class ParkingTransactionServiceTests
    {
        private readonly Mock<ILogger<ParkingTransactionService>> _loggerMock;
        private readonly Mock<IParkingFeeService> _feeServiceMock;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public ParkingTransactionServiceTests()
        {
            _loggerMock = new Mock<ILogger<ParkingTransactionService>>();
            _feeServiceMock = new Mock<IParkingFeeService>();
            
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext CreateContext()
        {
            var context = new ApplicationDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CreateTransactionAsync_ValidInput_CreatesTransaction()
        {
            // Arrange
            using var context = CreateContext();
            var service = new ParkingTransactionService(context, _feeServiceMock.Object, _loggerMock.Object);

            var vehicle = new Vehicle { Id = Guid.NewGuid(), PlateNumber = "TEST123" };
            var parkingZone = new ParkingZone { Id = Guid.NewGuid(), Name = "Test Zone" };

            context.Vehicles.Add(vehicle);
            context.ParkingZones.Add(parkingZone);
            await context.SaveChangesAsync();

            // Act
            var transaction = await service.CreateTransactionAsync(vehicle.Id, parkingZone.Id);

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal(vehicle.Id, transaction.VehicleId);
            Assert.Equal(parkingZone.Id, transaction.ParkingZoneId);
            Assert.Equal(TransactionStatus.Active, transaction.Status);
        }

        [Fact]
        public async Task CreateTransactionAsync_InvalidVehicle_ThrowsKeyNotFoundException()
        {
            // Arrange
            using var context = CreateContext();
            var service = new ParkingTransactionService(context, _feeServiceMock.Object, _loggerMock.Object);

            var parkingZone = new ParkingZone { Id = Guid.NewGuid(), Name = "Test Zone" };
            context.ParkingZones.Add(parkingZone);
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                service.CreateTransactionAsync(Guid.NewGuid(), parkingZone.Id));
        }

        [Fact]
        public async Task CompleteTransactionAsync_ValidTransaction_CompletesSuccessfully()
        {
            // Arrange
            using var context = CreateContext();
            var service = new ParkingTransactionService(context, _feeServiceMock.Object, _loggerMock.Object);

            var vehicle = new Vehicle { Id = Guid.NewGuid(), PlateNumber = "TEST123" };
            var parkingZone = new ParkingZone { Id = Guid.NewGuid(), Name = "Test Zone" };
            var transaction = new ParkingTransaction 
            { 
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                ParkingZoneId = parkingZone.Id,
                Status = TransactionStatus.Active,
                StartTime = DateTime.UtcNow.AddHours(-1)
            };

            context.Vehicles.Add(vehicle);
            context.ParkingZones.Add(parkingZone);
            context.ParkingTransactions.Add(transaction);
            await context.SaveChangesAsync();

            // Act
            var completedTransaction = await service.CompleteTransactionAsync(transaction.Id, 50.00m);

            // Assert
            Assert.NotNull(completedTransaction);
            Assert.Equal(TransactionStatus.Completed, completedTransaction.Status);
            Assert.Equal(50.00m, completedTransaction.Amount);
            Assert.NotNull(completedTransaction.EndTime);
        }

        [Fact]
        public async Task CalculateParkingFeeAsync_ActiveTransaction_CalculatesCorrectly()
        {
            // Arrange
            using var context = CreateContext();
            var service = new ParkingTransactionService(context, _feeServiceMock.Object, _loggerMock.Object);

            var vehicle = new Vehicle 
            { 
                Id = Guid.NewGuid(), 
                PlateNumber = "TEST123",
                VehicleType = "Car"
            };
            var parkingZone = new ParkingZone { Id = Guid.NewGuid(), Name = "Test Zone" };
            var transaction = new ParkingTransaction 
            { 
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                Vehicle = vehicle,
                ParkingZoneId = parkingZone.Id,
                Status = TransactionStatus.Active,
                StartTime = DateTime.UtcNow.AddHours(-2)
            };

            context.Vehicles.Add(vehicle);
            context.ParkingZones.Add(parkingZone);
            context.ParkingTransactions.Add(transaction);
            await context.SaveChangesAsync();

            _feeServiceMock.Setup(x => x.CalculateFee(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>()))
                .ReturnsAsync(25.00m);

            // Act
            var fee = await service.CalculateParkingFeeAsync(transaction.Id);

            // Assert
            Assert.Equal(25.00m, fee);
            _feeServiceMock.Verify(x => x.CalculateFee(
                It.Is<string>(t => t == "Car"),
                It.IsAny<TimeSpan>()), 
                Times.Once);
        }

        [Fact]
        public async Task GetActiveTransactionsAsync_ReturnsOnlyActiveTransactions()
        {
            // Arrange
            using var context = CreateContext();
            var service = new ParkingTransactionService(context, _feeServiceMock.Object, _loggerMock.Object);

            var vehicle = new Vehicle { Id = Guid.NewGuid(), PlateNumber = "TEST123" };
            var parkingZone = new ParkingZone { Id = Guid.NewGuid(), Name = "Test Zone" };
            
            var activeTransaction = new ParkingTransaction 
            { 
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                ParkingZoneId = parkingZone.Id,
                Status = TransactionStatus.Active,
                StartTime = DateTime.UtcNow.AddHours(-1)
            };

            var completedTransaction = new ParkingTransaction 
            { 
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                ParkingZoneId = parkingZone.Id,
                Status = TransactionStatus.Completed,
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = DateTime.UtcNow.AddHours(-1)
            };

            context.Vehicles.Add(vehicle);
            context.ParkingZones.Add(parkingZone);
            context.ParkingTransactions.AddRange(activeTransaction, completedTransaction);
            await context.SaveChangesAsync();

            // Act
            var activeTransactions = await service.GetActiveTransactionsAsync();

            // Assert
            Assert.Single(activeTransactions);
            Assert.Equal(TransactionStatus.Active, activeTransactions.First().Status);
        }
    }
}
