using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels;
using Parking_Zone.Hubs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Parking_Zone.Extensions;

using ModelVehicleEntry = Parking_Zone.Models.VehicleEntry;
using ViewModelVehicleEntry = Parking_Zone.ViewModels.VehicleEntry;

namespace Parking_Zone.Controllers
{
    [Authorize(Roles = "Admin,Operator")]
    public class EntryGateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EntryGateController> _logger;
        private readonly ConnectionStatusService _connectionStatusService;
        private readonly ICameraService _cameraService;
        private readonly StorageService _storageService;
        private readonly ITicketService _ticketService;
        private readonly IPrinterService _printerService;
        private readonly IHubContext<ParkingHub> _parkingHubContext;

        public EntryGateController(
            ApplicationDbContext context,
            ILogger<EntryGateController> logger,
            ConnectionStatusService connectionStatusService,
            ICameraService cameraService,
            StorageService storageService,
            ITicketService ticketService,
            IPrinterService printerService,
            IHubContext<ParkingHub> parkingHubContext)
        {
            _context = context;
            _logger = logger;
            _connectionStatusService = connectionStatusService;
            _cameraService = cameraService;
            _storageService = storageService;
            _ticketService = ticketService;
            _printerService = printerService;
            _parkingHubContext = parkingHubContext;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new GatesViewModel
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
                            .Select(t => new ViewModelVehicleEntry
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

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEntryGate(EntryGate gate)
        {
            if (ModelState.IsValid)
            {
                gate.Id = Guid.NewGuid();
                gate.IsActive = true;
                gate.LastActivity = DateTime.Now;
                
                _context.EntryGates.Add(gate);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"New entry gate added: {gate.Name}");
                
                return RedirectToAction(nameof(Index));
            }
            
            return View(gate);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEntryGate(string id, bool isActive)
        {
            var gate = await _context.EntryGates.FindAsync(id);
            if (gate == null)
            {
                return NotFound();
            }
            
            gate.IsActive = isActive;
            gate.LastActivity = DateTime.Now;
            
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Entry gate {id} status updated: Active = {isActive}");
            
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetGateStatus(string id)
        {
            var gate = await _context.EntryGates.FindAsync(id);
            if (gate == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                id = gate.Id,
                name = gate.Name,
                isOnline = gate.IsOnline,
                isOpen = gate.IsOpen,
                lastActivity = gate.LastActivity
            });
        }

        [HttpPost("capture")]
        public async Task<IActionResult> CaptureEntry([FromBody] EntryRequest request)
        {
            try 
            {
                // Validate vehicle type
                var vehicleType = await _context.VehicleTypes
                    .FirstOrDefaultAsync(vt => vt.Id == request.VehicleTypeId);
                
                if (vehicleType == null)
                {
                    return BadRequest(new { message = "Invalid vehicle type" });
                }

                // Capture vehicle photo if not provided
                byte[] imageBytes = request.ImagePath != null 
                    ? Convert.FromBase64String(request.ImagePath)
                    : await _cameraService.TakePhoto();
                
                // Generate unique filename
                string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{request.GateId}.jpg";
                
                // Save photo
                string imagePath = await _storageService.SaveImage(fileName, imageBytes);
                
                // Create vehicle entry
                var vehicleEntry = new VehicleEntry
                {
                    VehicleNumber = request.VehicleNumber,
                    LicensePlate = request.LicensePlate,
                    VehicleTypeId = request.VehicleTypeId,
                    EntryPhotoPath = imagePath,
                    EntryTime = DateTime.UtcNow,
                    GateId = request.GateId,
                    OperatorId = User.GetOperatorGuid(),
                    ParkingZoneId = 1 // Default parking zone, adjust if needed
                };

                _context.VehicleEntries.Add(vehicleEntry);
                await _context.SaveChangesAsync();

                // Generate ticket
                var ticket = await _ticketService.GenerateTicketAsync(
                    request.VehicleNumber, 
                    request.VehicleTypeId, 
                    imagePath, 
                    parkingGateId: request.GateId, 
                    operatorId: User.GetOperatorGuid()
                );

                return Ok(new { 
                    ticketNumber = ticket.TicketNumber,
                    imagePath = imagePath,
                    vehicleEntryId = vehicleEntry.Id
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error processing entry");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("assign-space")]
        public async Task<IActionResult> AssignSpace([FromBody] EntryRequest request)
        {
            try
            {
                // Find available parking space
                var availableSpace = await _context.ParkingSpaces
                    .Where(s => !s.IsOccupied && s.SpaceType == request.VehicleTypeId)
                    .OrderBy(s => s.SpaceNumber)
                    .FirstOrDefaultAsync();

                if (availableSpace == null)
                {
                    return BadRequest(new { message = "No parking space available" });
                }

                // Create parking transaction
                var transaction = new ParkingTransaction
                {
                    VehicleNumber = request.VehicleNumber,
                    VehicleLicensePlate = request.LicensePlate,
                    VehicleTypeId = request.VehicleTypeId,
                    EntryTime = DateTime.UtcNow,
                    EntryGateId = request.GateId,
                    EntryPhotoPath = request.ImagePath,
                    ParkingSpaceId = availableSpace.Id,
                    OperatorId = User.GetOperatorGuid(),
                    Status = "Active"
                };

                _context.ParkingTransactions.Add(transaction);
                
                // Update space status
                availableSpace.IsOccupied = true;
                
                await _context.SaveChangesAsync();

                return Ok(new { 
                    transactionId = transaction.Id,
                    parkingSpaceId = availableSpace.Id 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning parking space");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessVehicleEntry(VehicleEntryRequest request)
        {
            try
            {
                // Cari kendaraan berdasarkan nomor plat atau buat baru jika belum ada
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.LicensePlate == request.PlateNumber);

                if (vehicle == null)
                {
                    vehicle = new Vehicle
                    {
                        LicensePlate = request.PlateNumber,
                        Type = (int)request.VehicleType,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    _context.Vehicles.Add(vehicle);
                    await _context.SaveChangesAsync();
                }

                // Cari space parkir yang tersedia
                var parkingSpace = await _context.ParkingSpaces
                    .FirstOrDefaultAsync(ps => ps.IsActive && !ps.IsOccupied && ps.Type == (int)request.VehicleType);

                if (parkingSpace == null)
                {
                    TempData["ErrorMessage"] = "Tidak ada ruang parkir tersedia untuk jenis kendaraan ini";
                    return RedirectToAction(nameof(Operator), new { gateId = request.GateId });
                }

                // Buat entry vehicle baru
                var vehicleEntry = new ModelVehicleEntry
                {
                    LicensePlate = request.PlateNumber,
                    VehicleType = (int)request.VehicleType,
                    EntryTime = DateTime.Now,
                    EntryPhotoPath = request.PhotoPath,
                    VehicleId = vehicle.Id,
                    ParkingSpaceId = parkingSpace.Id,
                    OperatorId = User.GetOperatorGuid(),
                    Notes = request.Notes
                };

                _context.VehicleEntries.Add(vehicleEntry);

                // Buat transaksi parkir baru
                var transaction = new ParkingTransaction
                {
                    VehicleId = vehicle.Id,
                    ParkingSpaceId = parkingSpace.Id,
                    EntryTime = DateTime.Now,
                    OperatorId = User.GetOperatorGuid(),
                    EntryPhotoPath = request.PhotoPath,
                    CreatedAt = DateTime.Now
                };

                _context.ParkingTransactions.Add(transaction);

                // Update status ruang parkir menjadi terisi
                parkingSpace.IsOccupied = true;
                _context.ParkingSpaces.Update(parkingSpace);

                await _context.SaveChangesAsync();

                // Kirim notifikasi melalui SignalR
                await _parkingHubContext.Clients.All.SendAsync("ReceiveVehicleEntry", transaction);
                await _parkingHubContext.Clients.All.SendAsync("ReceiveSpaceUpdate", parkingSpace);
                
                // Cetak tiket jika diminta
                if (request.PrintTicket)
                {
                    await _parkingHubContext.Clients.All.SendAsync("PrintTicket", new
                    {
                        plateNumber = request.PlateNumber,
                        vehicleType = (int)request.VehicleType,
                        entryTime = transaction.EntryTime,
                        transactionId = transaction.Id
                    });
                }
                
                // Buka palang
                await _parkingHubContext.Clients.All.SendAsync("OpenEntryGate", request.GateId);

                TempData["SuccessMessage"] = "Kendaraan berhasil masuk area parkir";
                return RedirectToAction(nameof(Operator), new { gateId = request.GateId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Terjadi kesalahan: {ex.Message}";
                return RedirectToAction(nameof(Operator), new { gateId = request.GateId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessButtonPress(string gateId)
        {
            try
            {
                // Kirim notifikasi ke semua klien bahwa tombol masuk ditekan
                await _parkingHubContext.Clients.All.SendAsync("EntryButtonPressed", gateId);
                
                // Kirim perintah ke kamera untuk mengambil gambar
                await _parkingHubContext.Clients.All.SendAsync("TriggerCamera", gateId);
                
                return Ok(new { success = true, message = "Push button processed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PrintTicket(int transactionId)
        {
            try
            {
                var transaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transaction == null)
                {
                    return NotFound(new { success = false, message = "Transaksi tidak ditemukan" });
                }

                // Kirim perintah untuk mencetak tiket
                await _parkingHubContext.Clients.All.SendAsync("PrintTicket", new
                {
                    plateNumber = transaction.Vehicle?.LicensePlate,
                    vehicleType = (int)transaction.Vehicle?.Type,
                    entryTime = transaction.EntryTime,
                    transactionId = transaction.Id
                });

                return Ok(new { success = true, message = "Print command sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Operator(string gateId = "GATE-01")
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve recent vehicle entries
            var recentEntries = await _context.VehicleEntries
                .OrderByDescending(ve => ve.EntryTime)
                .Take(10)
                .ToListAsync();

            var viewModel = new EntryGateViewModel
            {
                GateId = gateId,
                OperatorName = $"{currentUser.FirstName} {currentUser.LastName}",
                Status = "Ready",
                IsCameraActive = false,
                IsPrinterActive = true,
                IsOfflineMode = false,
                LastSync = DateTime.Now,
                RecentEntries = recentEntries
            };

            return View(viewModel);
        }
    }
}