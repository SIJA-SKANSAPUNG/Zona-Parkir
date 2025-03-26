# 🚗 Parking Zone

Open source ASP.NET Core 8 MVC project for parking management system. Designed to provide an efficient and user-friendly parking reservation experience.

## 📁 Project Structure

```
├── Controllers/
│   ├── Api/
│   │   ├── AuthApiController.cs
│   │   ├── GatesApiController.cs
│   │   ├── TransactionsApiController.cs
│   │   └── VehiclesApiController.cs
│   ├── RatesController.cs
│   ├── TicketController.cs
│   ├── SettingsController.cs
│   ├── SiteSettingsController.cs
│   ├── CameraController.cs
│   ├── OperatorsController.cs
│   ├── ShiftsController.cs
│   └── ...
├── Extensions/
│   ├── ApplicationBuilderExtensions.cs
│   ├── ClaimsPrincipalExtensions.cs
│   ├── ConfigurationExtensions.cs
│   ├── DateTimeExtensions.cs
│   ├── ExceptionExtensions.cs
│   ├── FileExtensions.cs
│   ├── HttpContextExtensions.cs
│   ├── JsonExtensions.cs
│   ├── LoggerExtensions.cs
│   ├── QueryableExtensions.cs
│   ├── SecurityExtensions.cs
│   ├── ServiceCollectionExtensions.cs
│   ├── StringExtensions.cs
│   └── ValidationExtensions.cs
├── Middleware/
│   ├── RateLimitingMiddleware.cs
│   └── RateLimitingMiddlewareExtensions.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── FeeConfiguration.cs
│   ├── ParkingGate.cs
│   ├── ParkingTransaction.cs
│   ├── ParkingZone.cs
│   └── Vehicle.cs
├── Services/
│   ├── IParkingFeeService.cs
│   ├── IParkingGateService.cs
│   ├── IParkingNotificationService.cs
│   ├── IParkingTransactionService.cs
│   ├── IUserService.cs
│   ├── ParkingFeeService.cs
│   ├── ParkingGateService.cs
│   ├── ParkingNotificationService.cs
│   ├── ParkingTransactionService.cs
│   ├── IPCameraService.cs
│   ├── PrinterService.cs
│   ├── ScannerService.cs
│   ├── TicketService.cs
│   └── WebSocketServer.cs
├── ViewModels/
│   ├── AuthViewModels.cs
│   ├── DashboardViewModels.cs
│   ├── GateViewModels.cs
│   ├── HistoryViewModel.cs
│   ├── PrinterManagementViewModel.cs
│   ├── ReportsViewModel.cs
│   ├── SystemStatusViewModel.cs
│   ├── VehicleHistoryPageViewModel.cs
│   └── ParkingRateViewModel.cs
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml
│   ├── Dashboard/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _LoginPartial.cshtml
│       └── _ValidationScriptsPartial.cshtml
└── Hubs/
    └── ParkingHub.cs
```

### Features
- [x] User Authentication
  - [x] Identity with Login view
  - [x] JWT Authentication for API
  - [x] Role-based Authorization
- [x] Enhanced Real-time Monitoring (via SignalR)
  - [x] Gate Events and Status Updates
  - [x] Vehicle Detection and Plate Recognition
  - [x] Barrier Control and Status
  - [x] Entry/Exit Management
  - [x] Real-time Occupancy Updates
  - [x] Transaction Notifications
- [x] Vehicle Entry/Exit Management
- [x] Parking Fee Calculation
- [x] Gate Control System
- [x] Rate Limiting (API abuse prevention)
- [x] Enhanced Database Management
  - [x] DbContextFactory for flexible context creation
  - [x] Improved entity configurations
  - [x] Extended hardware settings models
  - [x] Detailed vehicle entry/exit tracking
- [x] Admin Dashboard
  - [x] Real-time Statistics
  - [x] Gate Status Overview
  - [x] Recent Transactions
  - [x] System Health
- [x] Reporting System
  - [x] Transaction History
  - [x] Vehicle History
  - [x] Revenue Reports
  - [x] Occupancy Analytics
  - [x] Custom Date Range Reports
- [x] Settings Management
  - [x] Site Configuration
  - [x] Gate Settings
  - [x] Rate Configuration
  - [x] Hardware Integration
  - [x] User Management
- [x] API Integration
  - [x] JWT Authentication
  - [x] Vehicle Management
  - [x] Gate Control
  - [x] Transaction Processing
  - [x] Hardware Integration
- [x] Operator Management
  - [x] Shift Management
  - [x] Operator Tracking
  - [x] Activity Logs