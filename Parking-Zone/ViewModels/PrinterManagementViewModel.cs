using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class PrinterManagementViewModel
    {
        public List<PrinterInfo> Printers { get; set; }
        public PrinterConfigurationModel Configuration { get; set; }
        public List<PrinterTestResult> TestResults { get; set; }

        public PrinterManagementViewModel()
        {
            Printers = new List<PrinterInfo>();
            Configuration = new PrinterConfigurationModel();
            TestResults = new List<PrinterTestResult>();
        }
    }

    public class PrinterInfo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Printer Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Default Printer")]
        public bool IsDefault { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Last Checked")]
        public DateTime LastChecked { get; set; }

        [Display(Name = "Online")]
        public bool IsOnline { get; set; }

        [Display(Name = "Assigned Gate")]
        public string AssignedGate { get; set; }

        // Printer settings
        [Display(Name = "Paper Width (mm)")]
        [Range(0, 200)]
        public int PaperWidth { get; set; }

        [Display(Name = "Characters Per Line")]
        [Range(0, 100)]
        public int CharactersPerLine { get; set; }

        [Display(Name = "Cut Paper After Print")]
        public bool CutPaperAfterPrint { get; set; }

        [Display(Name = "Open Cash Drawer After Print")]
        public bool OpenCashDrawerAfterPrint { get; set; }
    }

    public class PrinterConfigurationModel
    {
        [Display(Name = "Header Text")]
        public string HeaderText { get; set; }

        [Display(Name = "Footer Text")]
        public string FooterText { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Tax ID")]
        public string TaxId { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Show Logo")]
        public bool ShowLogo { get; set; }

        [Display(Name = "Show QR Code")]
        public bool ShowQRCode { get; set; }

        [Display(Name = "Print Duplicate")]
        public bool PrintDuplicate { get; set; }
    }

    public class PrinterTestResult
    {
        public int PrinterId { get; set; }
        public string PrinterName { get; set; }
        public DateTime TestTime { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string TestType { get; set; } // Text, Image, Barcode, etc.
        public TimeSpan Duration { get; set; }
    }
} 