using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ClosedXML.Excel;

namespace Parking_Zone.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IParkingService _parkingService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            ApplicationDbContext context,
            IParkingService parkingService,
            ILogger<ReportsController> logger)
        {
            _context = context;
            _parkingService = parkingService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new ReportsViewModel
                {
                    DailyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && t.ExitTime >= DateTime.Today)
                        .SumAsync(t => t.ParkingFee),
                    MonthlyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && 
                               t.ExitTime >= DateTime.Today.AddMonths(-1))
                        .SumAsync(t => t.ParkingFee),
                    TotalTransactions = await _context.ParkingTransactions.CountAsync(),
                    ActiveVehicles = await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit),
                    OccupancyRate = (await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit)) * 100.0 / (await _context.ParkingSpaces.CountAsync())
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating reports");
                return StatusCode(500, "Error generating reports");
            }
        }

        public async Task<IActionResult> Daily()
        {
            try
            {
                var viewModel = new ReportsViewModel
                {
                    DailyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && t.ExitTime >= DateTime.Today)
                        .SumAsync(t => t.ParkingFee),
                    MonthlyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && 
                               t.ExitTime >= DateTime.Today)
                        .SumAsync(t => t.ParkingFee),
                    TotalTransactions = await _context.ParkingTransactions
                        .CountAsync(t => t.ExitTime >= DateTime.Today),
                    ActiveVehicles = await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit && t.EntryTime >= DateTime.Today),
                    OccupancyRate = (await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit && t.EntryTime >= DateTime.Today)) * 100.0 / (await _context.ParkingSpaces.CountAsync())
                };

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating daily report");
                return StatusCode(500, "Error generating daily report");
            }
        }

        public async Task<IActionResult> Monthly(DateTime? date)
        {
            try
            {
                var reportDate = date ?? DateTime.Today;
                var startDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var viewModel = new ReportsViewModel
                {
                    DailyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && t.ExitTime >= startDate && t.ExitTime <= endDate)
                        .SumAsync(t => t.ParkingFee),
                    MonthlyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && 
                               t.ExitTime >= startDate && t.ExitTime <= endDate)
                        .SumAsync(t => t.ParkingFee),
                    TotalTransactions = await _context.ParkingTransactions
                        .CountAsync(t => t.ExitTime >= startDate && t.ExitTime <= endDate),
                    ActiveVehicles = await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit && t.EntryTime >= startDate && t.EntryTime <= endDate),
                    OccupancyRate = (await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit && t.EntryTime >= startDate && t.EntryTime <= endDate)) * 100.0 / (await _context.ParkingSpaces.CountAsync())
                };

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating monthly report");
                return StatusCode(500, "Error generating monthly report");
            }
        }

        public async Task<IActionResult> PreviewReport(DateTime? startDate, DateTime? endDate, string vehicleType)
        {
            try
            {
                var viewModel = new ReportsViewModel
                {
                    DailyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && t.ExitTime >= startDate && t.ExitTime <= endDate)
                        .SumAsync(t => t.ParkingFee),
                    MonthlyRevenue = await _context.ParkingTransactions
                        .Where(t => t.IsExit && t.ParkingFee > 0 && 
                               t.ExitTime >= startDate && t.ExitTime <= endDate)
                        .SumAsync(t => t.ParkingFee),
                    TotalTransactions = await _context.ParkingTransactions
                        .CountAsync(t => t.ExitTime >= startDate && t.ExitTime <= endDate),
                    ActiveVehicles = await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit && t.EntryTime >= startDate && t.EntryTime <= endDate),
                    OccupancyRate = (await _context.ParkingTransactions
                        .CountAsync(t => !t.IsExit && t.EntryTime >= startDate && t.EntryTime <= endDate)) * 100.0 / (await _context.ParkingSpaces.CountAsync())
                };

                return PartialView("_ReportPreview", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report preview");
                return StatusCode(500, "Error generating report preview");
            }
        }

        public async Task<IActionResult> ExportToPdf(DateTime? date)
        {
            try
            {
                var reportDate = date ?? DateTime.Today;
                var startDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var transactions = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .OrderByDescending(t => t.EntryTime)
                    .ToListAsync();

                var stats = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .GroupBy(t => t.Vehicle.VehicleType)
                    .Select(g => new VehicleTypeStats
                    {
                        VehicleType = g.Key,
                        Count = g.Count(),
                        TotalRevenue = g.Sum(t => t.Amount),
                        AverageTransaction = g.Average(t => t.Amount)
                    })
                    .OrderByDescending(g => g.TotalRevenue)
                    .ToListAsync();

                var monthlyRevenue = await _context.ParkingTransactions
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .SumAsync(t => t.Amount);

                GeneratePdfReport(stats);

                _logger.LogInformation($"Successfully generated PDF report for {reportDate:MMMM yyyy}");

                return File(System.IO.File.ReadAllBytes("VehicleTypeReport.pdf"), "application/pdf", $"parkir_report_{reportDate:yyyy_MM_dd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report");
                return StatusCode(500, "Error generating PDF report");
            }
        }

        public async Task<IActionResult> ExportToExcel(DateTime? date)
        {
            try
            {
                var reportDate = date ?? DateTime.Today;
                var startDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var transactions = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .OrderByDescending(t => t.EntryTime)
                    .ToListAsync();

                var stats = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .GroupBy(t => t.Vehicle.VehicleType)
                    .Select(g => new VehicleTypeStats
                    {
                        VehicleType = g.Key,
                        Count = g.Count(),
                        TotalRevenue = g.Sum(t => t.Amount),
                        AverageTransaction = g.Average(t => t.Amount)
                    })
                    .OrderByDescending(g => g.TotalRevenue)
                    .ToListAsync();

                var monthlyRevenue = await _context.ParkingTransactions
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .SumAsync(t => t.Amount);

                GenerateExcelReport(stats);

                _logger.LogInformation($"Successfully generated Excel report for {reportDate:MMMM yyyy}");

                return File(System.IO.File.ReadAllBytes("VehicleTypeReport.xlsx"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"parkir_report_{reportDate:yyyy_MM_dd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel report");
                return StatusCode(500, "Error generating Excel report");
            }
        }

        private void GeneratePdfReport(List<VehicleTypeStats> stats)
        {
            using (var writer = new PdfWriter("VehicleTypeReport.pdf"))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                document.Add(new Paragraph("Vehicle Type Statistics Report"));
                
                var table = new Table(4);
                table.AddHeaderCell("Vehicle Type");
                table.AddHeaderCell("Total Transactions");
                table.AddHeaderCell("Average Transaction");
                table.AddHeaderCell("Total Revenue");

                foreach (var stat in stats)
                {
                    table.AddCell(stat.VehicleType);
                    table.AddCell(stat.Count.ToString());
                    table.AddCell(stat.AverageTransaction.ToString("C"));
                    table.AddCell(stat.TotalRevenue.ToString("C"));
                }

                document.Add(table);
            }
        }

        private void GenerateExcelReport(List<VehicleTypeStats> stats)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vehicle Type Stats");
                
                worksheet.Cell(1, 1).Value = "Vehicle Type";
                worksheet.Cell(1, 2).Value = "Total Transactions";
                worksheet.Cell(1, 3).Value = "Average Transaction";
                worksheet.Cell(1, 4).Value = "Total Revenue";

                for (int i = 0; i < stats.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = stats[i].VehicleType;
                    worksheet.Cell(i + 2, 2).Value = stats[i].Count;
                    worksheet.Cell(i + 2, 3).Value = stats[i].AverageTransaction;
                    worksheet.Cell(i + 2, 4).Value = stats[i].TotalRevenue;
                }

                workbook.SaveAs("VehicleTypeReport.xlsx");
            }
        }

        public class ReportsViewModel
        {
            public decimal DailyRevenue { get; set; }
            public decimal MonthlyRevenue { get; set; }
            public int TotalTransactions { get; set; }
            public int ActiveVehicles { get; set; }
            public double OccupancyRate { get; set; }
        }
    }
}
