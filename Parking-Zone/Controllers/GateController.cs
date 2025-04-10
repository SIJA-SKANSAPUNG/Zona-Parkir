using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Drawing;
using Parking_Zone.Extensions;

namespace Parking_Zone.Controllers
{
    [Authorize]
    public class GateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<GateController> _logger;
        private readonly IPrinterService _printerService;
        private readonly IParkingGateService _gateService;

        public GateController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<GateController> logger,
            IPrinterService printerService,
            IParkingGateService gateService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _printerService = printerService;
            _gateService = gateService;
        }

        // GET: Gate/Entry
        public IActionResult Entry()
        {
            return View();
        }

        // POST: Gate/ProcessEntry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessEntry([FromForm] VehicleEntryModel model, [FromForm] string base64Image)
        {
            try
            {
                _logger.LogInformation($"Processing vehicle entry for {model.VehicleNumber}, Type: {model.VehicleType}");

                // Check if vehicle is already parked
                var existingVehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.VehicleNumber == model.VehicleNumber && v.IsParked);

                if (existingVehicle != null)
                {
                    return Json(new { success = false, message = "Kendaraan sudah terparkir" });
                }

                // Find available parking space
                var parkingSpace = await FindOptimalParkingSpace(model.VehicleType);
                if (parkingSpace == null)
                {
                    return Json(new { success = false, message = "Tidak ada slot parkir yang tersedia untuk jenis kendaraan ini" });
                }

                // Save image if provided
                string? photoPath = null;
                if (!string.IsNullOrEmpty(base64Image))
                {
                    photoPath = await SaveBase64Image(base64Image, "entry-photos");
                }

                // Get current shift
                var currentShift = await GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    return Json(new { success = false, message = "Tidak ada shift aktif saat ini" });
                }

                // Create new vehicle record
                var vehicle = new Vehicle
                {
                    VehicleNumber = model.VehicleNumber,
                    VehicleType = model.VehicleType,
                    DriverName = model.DriverName ?? string.Empty,
                    PhoneNumber = model.PhoneNumber ?? string.Empty,
                    EntryTime = DateTime.Now,
                    IsParked = true,
                    EntryPhotoPath = photoPath ?? string.Empty,
                    ParkingSpaceId = parkingSpace.Id,
                    ShiftId = currentShift.Id
                };

                // Generate barcode
                string barcodeData = GenerateBarcodeData(vehicle);
                string barcodeImagePath = await GenerateAndSaveQRCode(barcodeData);
                vehicle.BarcodeImagePath = barcodeImagePath;

                // Update parking space
                parkingSpace.IsOccupied = true;
                parkingSpace.LastOccupiedTime = DateTime.Now;

                // Create parking ticket
                var ticket = new ParkingTicket
                {
                    TicketNumber = GenerateTicketNumber(),
                    BarcodeData = barcodeData,
                    BarcodeImagePath = barcodeImagePath,
                    IssueTime = DateTime.Now,
                    Vehicle = vehicle,
                    OperatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    ShiftId = currentShift.Id
                };

                // Create parking transaction
                var transaction = new ParkingTransaction
                {
                    Vehicle = vehicle,
                    ParkingSpace = parkingSpace,
                    EntryTime = DateTime.Now,
                    TransactionNumber = GenerateTransactionNumber(),
                    Status = "Active"
                };

                await _context.Vehicles.AddAsync(vehicle);
                await _context.ParkingTickets.AddAsync(ticket);
                await _context.ParkingTransactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                // Print ticket
                bool printSuccess = await _printerService.PrintTicket(ticket);
                if (!printSuccess)
                {
                    _logger.LogWarning("Failed to print ticket {TicketNumber}", ticket.TicketNumber);
                }

                // Open gate
                await OpenGateAsync();

                return Json(new { 
                    success = true, 
                    message = "Kendaraan berhasil diproses" + (!printSuccess ? " (Gagal mencetak tiket)" : ""),
                    ticketNumber = ticket.TicketNumber,
                    entryTime = vehicle.EntryTime,
                    barcodeImageUrl = $"/images/barcodes/{Path.GetFileName(barcodeImagePath)}",
                    printStatus = printSuccess
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle entry");
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        // GET: Gate/Exit
        public IActionResult Exit()
        {
            return View();
        }

        // POST: Gate/ProcessExit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessExit([FromForm] string ticketNumber, [FromForm] string base64Image)
        {
            try
            {
                var ticket = await _context.ParkingTickets
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

                if (ticket == null)
                {
                    return NotFound(new { error = "Tiket tidak ditemukan" });
                }

                if (ticket.IsUsed)
                {
                    return BadRequest(new { error = "Tiket sudah digunakan" });
                }

                // Save exit photo
                string? exitPhotoPath = null;
                if (!string.IsNullOrEmpty(base64Image))
                {
                    exitPhotoPath = await SaveBase64Image(base64Image, "exit_photos");
                }

                // Update vehicle status
                var vehicle = ticket.Vehicle;
                if (vehicle == null)
                {
                    return BadRequest(new { error = "Vehicle data not found" });
                }

                vehicle.IsParked = false;
                vehicle.ExitTime = DateTime.Now;
                vehicle.ExitPhotoPath = exitPhotoPath ?? string.Empty;

                // Mark ticket as used
                ticket.IsUsed = true;
                ticket.ScanTime = DateTime.Now;

                // Get current shift
                var currentShift = await GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    return BadRequest(new { error = "Tidak ada shift aktif saat ini" });
                }

                // Create journal entry
                var journal = new Journal
                {
                    Action = "Check Out",
                    Description = $"Vehicle {vehicle.VehicleNumber} checked out",
                    Timestamp = DateTime.UtcNow,
                    OperatorId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                _context.Journals.Add(journal);
                await _context.SaveChangesAsync();

                // Find transaction
                var transaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.TransactionNumber == ticket.TicketNumber);

                if (transaction == null)
                {
                    return NotFound(new { error = "Transaksi tidak ditemukan" });
                }

                // Calculate parking duration and fee
                var duration = DateTime.Now - transaction.EntryTime;
                var parkingFee = await CalculateParkingFee(duration, vehicle.VehicleType);

                // Update transaction
                transaction.ExitTime = DateTime.Now;
                transaction.Duration = duration;
                transaction.ParkingFee = parkingFee;
                transaction.Status = "Completed";
                transaction.ExitOperatorId = User.GetOperatorId();

                // Update ticket
                ticket.IsUsed = true;

                // Update vehicle
                vehicle.IsParked = false;
                vehicle.ExitTime = DateTime.Now;

                // Update parking space
                var parkingSpace = await _context.ParkingSpaces
                    .FirstOrDefaultAsync(ps => ps.Id == transaction.ParkingSpaceId);
                
                if (parkingSpace != null)
                {
                    parkingSpace.IsOccupied = false;
                    parkingSpace.LastVacatedTime = DateTime.Now;
                }

                // Save changes
                await _context.SaveChangesAsync();

                // Print receipt
                bool receiptPrinted = await _printerService.PrintExitReceipt(new ExitReceiptViewModel
                {
                    LicensePlate = transaction.VehicleLicensePlate,
                    VehicleType = transaction.VehicleType,
                    EntryTime = transaction.EntryTime,
                    ExitTime = transaction.ExitTime ?? DateTime.Now,
                    Duration = duration,
                    ParkingFee = parkingFee,
                    TicketNumber = ticket.TicketNumber
                });

                // Open gate
                await _gateService.OpenGateAsync(Guid.Parse(Request.Form["gateId"]));

                return Json(new
                {
                    success = true,
                    message = "Kendaraan berhasil keluar" + (!receiptPrinted ? " (Gagal mencetak kwitansi)" : ""),
                    licensePlate = transaction.VehicleLicensePlate,
                    parkingFee = parkingFee,
                    receiptPrinted = receiptPrinted
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Terjadi kesalahan: {ex.Message}" });
            }
        }

        private async Task<string> SaveBase64Image(string base64Image, string folder)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image) || !base64Image.Contains(","))
                {
                    _logger.LogWarning("Invalid base64 image format");
                    return string.Empty;
                }

                var base64Data = base64Image.Split(',')[1];
                if (string.IsNullOrEmpty(base64Data))
                {
                    _logger.LogWarning("Empty base64 image data");
                    return string.Empty;
                }

                var bytes = Convert.FromBase64String(base64Data);
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.jpg";
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                
                return $"/uploads/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save base64 image");
                return string.Empty;
            }
        }

        private string GenerateBarcodeData(Vehicle vehicle)
        {
            return $"{vehicle.VehicleNumber}|{vehicle.EntryTime:yyyyMMddHHmmss}";
        }

        private string GenerateTicketNumber()
        {
            return $"TKT{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private string GenerateTransactionNumber()
        {
            return $"TRN-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        }

        private async Task<string> GenerateAndSaveQRCode(string data)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new BitmapByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);
                
                var fileName = $"qr_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.png";
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "barcodes", fileName);
                
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await System.IO.File.WriteAllBytesAsync(filePath, qrCodeBytes);
                
                return $"/images/barcodes/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                return string.Empty;
            }
        }

        private string GenerateJournalNumber()
        {
            return $"JRN-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        }

        private async Task<Shift?> GetCurrentShiftAsync()
        {
            try
            {
                var now = DateTime.Now;
                // First get all active shifts
                var activeShifts = await _context.Shifts
                    .Where(s => s.IsActive)
                    .ToListAsync();
                
                // Then check time in memory
                return activeShifts.FirstOrDefault(s => s.IsTimeInShift(now));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current shift");
                return null;
            }
        }

        private async Task<ParkingSpace?> FindOptimalParkingSpace(string vehicleType)
        {
            try
            {
                // Get all unoccupied parking spaces for the vehicle type
                var availableSpaces = await _context.ParkingSpaces
                    .Where(p => !p.IsOccupied && p.SpaceType == vehicleType && p.IsActive)
                    .OrderBy(p => p.LastOccupiedTime)
                    .FirstOrDefaultAsync();

                if (availableSpaces == null)
                {
                    _logger.LogWarning($"No available parking space found for vehicle type: {vehicleType}");
                    return null;
                }

                return availableSpaces;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error finding optimal parking space for vehicle type: {vehicleType}");
                return null;
            }
        }

        private async Task OpenGateAsync()
        {
            try
            {
                // TODO: Implement actual gate control logic
                await Task.Delay(1000); // Simulate gate operation
                _logger.LogInformation("Gate opened successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening gate");
                throw;
            }
        }

        private async Task<decimal> CalculateParkingFee(TimeSpan duration, string vehicleType)
        {
            // Implement parking fee calculation logic here
            // For demonstration purposes, a simple calculation is used
            var hours = duration.TotalHours;
            var feePerHour = vehicleType == "Car" ? 5.00m : 10.00m;
            return (decimal)hours * feePerHour;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var gates = new GatesViewModel
            {
                EntryGates = await _context.EntryGates
                    .Select(g => new GateOperationalViewModel
                    {
                        GateId = g.Id.ToString(),
                        OperatorName = g.Name,
                        Status = g.Status,
                        IsCameraActive = g.IsActive,
                        LastSync = g.LastActivity ?? DateTime.Now,
                        RecentEntries = g.Transactions
                            .Where(t => t.EntryTime.Date == DateTime.Today)
                            .Select(t => new VehicleEntry
                            {
                                LicensePlate = t.LicensePlate,
                                VehicleType = t.VehicleType,
                                EntryTime = t.EntryTime,
                                TicketNumber = t.TicketNumber,
                                GateId = g.Id.ToString(),
                                OperatorId = t.OperatorId.ToString()
                            })
                            .ToList()
                    })
                    .ToListAsync(),

                ExitGates = await _context.ExitGates
                    .Select(g => new GateExitOperationalViewModel
                    {
                        GateId = g.Id.ToString(),
                        OperatorName = g.Name,
                        Status = g.Status,
                        IsCameraActive = g.IsActive,
                        LastSync = g.LastActivity ?? DateTime.Now,
                        RecentExits = g.Transactions
                            .Where(t => t.ExitTime.HasValue && t.ExitTime.Value.Date == DateTime.Today)
                            .Select(t => new VehicleExit
                            {
                                LicensePlate = t.LicensePlate,
                                VehicleType = t.VehicleType,
                                EntryTime = t.EntryTime,
                                ExitTime = t.ExitTime.Value,
                                Fee = t.ParkingFee ?? 0,
                                TicketNumber = t.TicketNumber,
                                GateId = g.Id.ToString(),
                                OperatorId = t.OperatorId.ToString()
                            })
                            .ToList()
                    })
                    .ToListAsync()
            };

            return View(gates);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var gate = await _gateService.GetGateByIdAsync(id);
            if (gate == null)
                return NotFound();

            return View(gate);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OpenGate(Guid id)
        {
            var gate = await _gateService.GetGateByIdAsync(id);
            if (gate == null)
                return NotFound();

            try
            {
                await _gateService.OpenGateAsync(id);
                _logger.LogInformation($"Gate {id} opened successfully");
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error opening gate {id}");
                return StatusCode(500, "An error occurred while opening the gate");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CloseGate(Guid id)
        {
            var gate = await _gateService.GetGateByIdAsync(id);
            if (gate == null)
                return NotFound();

            try
            {
                await _gateService.CloseGateAsync(id);
                _logger.LogInformation($"Gate {id} closed successfully");
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error closing gate {id}");
                return StatusCode(500, "An error occurred while closing the gate");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Status(string id)
        {
            if (!HttpContext.IsAjaxRequest())
                return BadRequest();

            var gate = await _gateService.GetGateByIdAsync(Guid.Parse(id));
            if (gate == null)
                return NotFound();

            var status = new
            {
                gate.Id,
                gate.Name,
                gate.Status,
                gate.LastUpdated,
                UserIp = HttpContext.GetIpAddress(),
                RequestTime = DateTime.Now.ToTimeZoneString()
            };

            return Json(status);
        }

        // LOG: Gate Operation
        [HttpPost]
        public async Task<IActionResult> LogOperation([FromBody] GateOperationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var operation = new GateOperation
            {
                OperationType = model.OperationType,
                Status = model.Status,
                Details = model.Details,
                Timestamp = DateTime.Now,
                OperatorId = model.OperatorId,
                OperatorName = model.OperatorName
            };

            if (model.GateType == "Entry" && model.GateId.HasValue)
            {
                operation.EntryGateId = model.GateId;
            }
            else if (model.GateType == "Exit" && model.GateId.HasValue)
            {
                operation.ExitGateId = model.GateId;
            }
            
            if (!string.IsNullOrEmpty(model.TransactionId))
            {
                if (Guid.TryParse(model.TransactionId, out Guid transactionId))
                {
                    operation.ParkingTransactionId = transactionId;
                }
            }

            _context.GateOperations.Add(operation);
            
            // Log to Journal
            var journal = new Journal
            {
                EventType = "GateOperation",
                Description = $"{model.GateType} Gate Operation: {model.OperationType}",
                EntityId = model.GateId,
                EntityType = $"{model.GateType}Gate",
                Details = model.Details,
                UserName = model.OperatorName,
                Timestamp = DateTime.Now
            };
            
            _context.Journal.Add(journal);
            
            await _context.SaveChangesAsync();
            
            return Ok(operation.Id);
        }
    }
}