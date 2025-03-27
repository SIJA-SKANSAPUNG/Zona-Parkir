using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Parking_Zone.Extensions;
using System.Collections.Generic;

namespace Parking_Zone.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingTransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ParkingHub> _parkingHub;
        private readonly IParkingService _parkingService;
        private readonly ILogger<ParkingTransactionController> _logger;
        private readonly IParkingTransactionService _transactionService;
        private readonly IParkingFeeService _feeService;

        public ParkingTransactionController(
            ApplicationDbContext context,
            IHubContext<ParkingHub> parkingHub,
            IParkingService parkingService,
            ILogger<ParkingTransactionController> logger,
            IParkingTransactionService transactionService,
            IParkingFeeService feeService)
        {
            _context = context;
            _parkingHub = parkingHub;
            _parkingService = parkingService;
            _logger = logger;
            _transactionService = transactionService;
            _feeService = feeService;
        }

        // POST: api/ParkingTransaction/entry
        [HttpPost("entry")]
        public async Task<IActionResult> VehicleEntry([FromBody] VehicleEntryRequest request)
        {
            try
            {
                // Cari kendaraan berdasarkan nomor plat atau buat baru jika belum ada
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.PlateNumber == request.PlateNumber);

                if (vehicle == null)
                {
                    vehicle = new Vehicle
                    {
                        PlateNumber = request.PlateNumber,
                        Type = request.VehicleType,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    _context.Vehicles.Add(vehicle);
                    await _context.SaveChangesAsync();
                }

                // Cari space parkir yang tersedia
                var parkingSpace = await _context.ParkingSpaces
                    .FirstOrDefaultAsync(ps => ps.IsActive && !ps.IsOccupied && ps.Type == request.VehicleType);

                if (parkingSpace == null)
                {
                    return BadRequest("Tidak ada ruang parkir tersedia untuk jenis kendaraan ini");
                }

                // Buat transaksi parkir baru
                var transaction = new ParkingTransaction
                {
                    VehicleId = vehicle.Id,
                    ParkingSpaceId = parkingSpace.Id,
                    EntryTime = DateTime.Now,
                    OperatorId = request.OperatorId,
                    EntryPhotoPath = request.PhotoPath,
                    CreatedAt = DateTime.Now
                };

                _context.ParkingTransactions.Add(transaction);

                // Update status ruang parkir menjadi terisi
                parkingSpace.IsOccupied = true;
                _context.ParkingSpaces.Update(parkingSpace);

                await _context.SaveChangesAsync();

                // Kirim notifikasi melalui SignalR
                await _parkingHub.Clients.All.SendAsync("ReceiveVehicleEntry", transaction);
                await _parkingHub.Clients.All.SendAsync("ReceiveSpaceUpdate", parkingSpace);

                return Ok(new
                {
                    Success = true,
                    Message = "Kendaraan berhasil masuk",
                    Data = transaction
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during vehicle entry");
                return StatusCode(500, new { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        // POST: api/ParkingTransaction/exit
        [HttpPost("exit")]
        public async Task<IActionResult> VehicleExit([FromBody] VehicleExitRequest request)
        {
            try
            {
                // Cari transaksi parkir yang aktif (belum keluar)
                var transaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingSpace)
                    .FirstOrDefaultAsync(t => t.Id == request.TransactionId && t.ExitTime == null);

                if (transaction == null)
                {
                    return BadRequest("Transaksi parkir tidak ditemukan atau kendaraan sudah keluar");
                }

                // Update data transaksi
                transaction.ExitTime = DateTime.Now;
                transaction.ExitPhotoPath = request.PhotoPath;
                transaction.Fee = request.Fee;
                transaction.IsPaid = request.IsPaid;
                transaction.UpdatedAt = DateTime.Now;

                // Update status ruang parkir menjadi tersedia kembali
                if (transaction.ParkingSpace != null)
                {
                    transaction.ParkingSpace.IsOccupied = false;
                    _context.ParkingSpaces.Update(transaction.ParkingSpace);
                }

                _context.ParkingTransactions.Update(transaction);
                await _context.SaveChangesAsync();

                // Kirim notifikasi melalui SignalR
                await _parkingHub.Clients.All.SendAsync("ReceiveVehicleExit", transaction);
                if (transaction.ParkingSpace != null)
                {
                    await _parkingHub.Clients.All.SendAsync("ReceiveSpaceUpdate", transaction.ParkingSpace);
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Kendaraan berhasil keluar",
                    Data = transaction
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during vehicle exit");
                return StatusCode(500, new { Success = false, Message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate ??= DateTime.Now.StartOfDay();
            endDate ??= DateTime.Now.EndOfDay();

            var transactions = await _transactionService.GetTransactionsAsync(startDate.Value, endDate.Value);
            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (!id.IsValidGuid())
            {
                _logger.LogWarning("Invalid transaction ID format: {Id}", id);
                return BadRequest("Invalid transaction ID format");
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(Guid.Parse(id));
            if (transaction == null)
                return NotFound();

            // Format license plate
            transaction.VehicleLicensePlate = transaction.VehicleLicensePlate.FormatLicensePlate();

            // Calculate duration
            transaction.Duration = transaction.EntryTime.CalculateParkingDuration(transaction.ExitTime);

            return View(transaction);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> UpdateFee(string id, decimal newFee)
        {
            if (!id.IsValidGuid())
            {
                _logger.LogWarning("Invalid transaction ID format: {Id}", id);
                return BadRequest("Invalid transaction ID format");
            }

            try
            {
                await _transactionService.UpdateTransactionFeeAsync(Guid.Parse(id), newFee);
                _logger.LogUserAction(User.GetUserId(), "Update Fee", $"Updated fee for transaction {id} to {newFee}");
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction fee");
                return StatusCode(500, "An error occurred while updating the fee");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Receipt(string id)
        {
            if (!id.IsValidGuid())
                return BadRequest("Invalid transaction ID format");

            var transaction = await _transactionService.GetTransactionByIdAsync(Guid.Parse(id));
            if (transaction == null)
                return NotFound();

            // Format data for receipt
            transaction.VehicleLicensePlate = transaction.VehicleLicensePlate.FormatLicensePlate();
            transaction.Duration = transaction.EntryTime.CalculateParkingDuration(transaction.ExitTime);
            transaction.EntryTimeFormatted = transaction.EntryTime.ToTimeZoneString();
            transaction.ExitTimeFormatted = transaction.ExitTime?.ToTimeZoneString();

            return View(transaction);
        }

        public class VehicleExitRequest
        {
            public int TransactionId { get; set; }
            public decimal Fee { get; set; }
            public bool IsPaid { get; set; }
            public string? PhotoPath { get; set; }
        }
    }
}