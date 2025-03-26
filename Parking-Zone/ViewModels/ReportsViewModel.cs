using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels
{
    public class ReportsViewModel
    {
        public ReportFilterModel Filter { get; set; }
        public List<ReportData> Data { get; set; }
        public ReportSummaryModel Summary { get; set; }
        public List<ChartData> Charts { get; set; }

        public ReportsViewModel()
        {
            Filter = new ReportFilterModel();
            Data = new List<ReportData>();
            Summary = new ReportSummaryModel();
            Charts = new List<ChartData>();
        }
    }

    public class ReportFilterModel
    {
        [Required]
        [Display(Name = "Report Type")]
        public ReportType Type { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Parking Zone")]
        public int? ParkingZoneId { get; set; }

        [Display(Name = "Gate")]
        public int? GateId { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Display(Name = "Operator")]
        public int? OperatorId { get; set; }

        [Display(Name = "Group By")]
        public string GroupBy { get; set; } // Daily, Weekly, Monthly, etc.

        [Display(Name = "Include Charts")]
        public bool IncludeCharts { get; set; } = true;

        [Display(Name = "Export Format")]
        public string ExportFormat { get; set; } // PDF, Excel, CSV
    }

    public class ReportData
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public int TransactionCount { get; set; }
        public decimal Revenue { get; set; }
        public int VehicleCount { get; set; }
        public decimal AverageStayDuration { get; set; }
        public decimal OccupancyRate { get; set; }
        public Dictionary<string, decimal> AdditionalData { get; set; }

        public ReportData()
        {
            AdditionalData = new Dictionary<string, decimal>();
        }
    }

    public class ReportSummaryModel
    {
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UniqueVehicles { get; set; }
        public decimal AverageRevenue { get; set; }
        public decimal PeakOccupancy { get; set; }
        public DateTime? PeakTime { get; set; }
        public Dictionary<string, decimal> RevenueByCategory { get; set; }
        public Dictionary<string, int> TransactionsByType { get; set; }

        public ReportSummaryModel()
        {
            RevenueByCategory = new Dictionary<string, decimal>();
            TransactionsByType = new Dictionary<string, int>();
        }
    }

    public class ChartData
    {
        public string ChartType { get; set; } // Line, Bar, Pie, etc.
        public string Title { get; set; }
        public List<string> Labels { get; set; }
        public List<ChartSeries> Series { get; set; }
        public ChartOptions Options { get; set; }

        public ChartData()
        {
            Labels = new List<string>();
            Series = new List<ChartSeries>();
            Options = new ChartOptions();
        }
    }

    public class ChartSeries
    {
        public string Name { get; set; }
        public List<decimal> Data { get; set; }
        public string Color { get; set; }

        public ChartSeries()
        {
            Data = new List<decimal>();
        }
    }

    public class ChartOptions
    {
        public bool ShowLegend { get; set; } = true;
        public bool ShowTooltips { get; set; } = true;
        public bool Responsive { get; set; } = true;
        public string YAxisLabel { get; set; }
        public string XAxisLabel { get; set; }
    }

    public enum ReportType
    {
        Revenue,
        Occupancy,
        VehicleTypes,
        OperatorPerformance,
        GateUtilization,
        PeakHours,
        Custom
    }
} 