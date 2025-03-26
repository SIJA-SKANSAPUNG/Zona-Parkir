# 🚗 Parking Zone

Open source ASP.NET Core 8 MVC project for parking management system. Designed to provide an efficient and user-friendly parking reservation experience.

## 📁 Project Structure

```
Parking-Zone/
├── Parking-Zone/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Migrations/
│   ├── Models/
│   │   ├── FeeConfiguration.cs
│   │   ├── ParkingGate.cs
│   │   ├── ParkingTransaction.cs
│   │   ├── ParkingZone.cs
│   │   └── Vehicle.cs
│   ├── Services/
│   │   ├── IParkingFeeService.cs
│   │   ├── IVehicleService.cs
│   │   ├── ParkingFeeService.cs
│   │   └── VehicleService.cs
│   └── Views/
├── Parking-Zone.Tests/
│   ├── ParkingFeeServiceTests.cs
│   └── VehicleServiceTests.cs
└── Tests/
```

## ✅ Progress Checklist

### Models
- [x] Vehicle Model
- [x] ParkingGate Model
- [x] ParkingTransaction Model
- [x] FeeConfiguration Model
- [x] ParkingZone Model
- [ ] User Model
- [ ] Reservation Model

### Services
- [x] Vehicle Service
- [x] Parking Fee Service
- [x] Parking Gate Service
- [x] Transaction Service
- [ ] Reservation Service
- [ ] User Service

### Testing
- [x] Vehicle Service Tests
- [x] Parking Fee Service Tests
- [x] Parking Gate Service Tests
- [x] Transaction Service Tests
- [ ] Reservation Service Tests
- [ ] Integration Tests

### Features
- [ ] User Authentication
- [ ] User Authorization
- [ ] Parking Spot Reservation
- [x] Real-time Parking Availability
- [ ] Payment Integration
- [ ] Reporting System
- [x] Gate Management: Control and monitor parking gates via API
- [x] Transaction Management: Handle parking transactions with fee calculation
- [x] Real-time Monitoring: SignalR-based live updates for parking events

## 🌟 Features

- **Booking System:** Reserve parking spots ahead of time
- **User Management:** Simple user authentication and authorization
- **Entity Framework Core:** Robust data management with SQL Server backend

## 🛠 Technologies Used

- **Framework:** ASP.NET Core 8
- **Architecture:** Model-View-Controller (MVC)
- **Database:** SQL Server with Entity Framework Core
- **Frontend:** JavaScript, jQuery, Bootstrap
- **Real-time Communication:** SignalR
- **Testing:** XUnit for unit testing

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or later

### Installation

1. Clone the repository
2. Configure database connection string in `appsettings.json`
3. Run database migrations:
   ```bash
   dotnet ef database update
   ```
4. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

## 🤝 Contributing

Contributions are welcome! Please read our contributing guidelines before getting started.

## 📄 License

This project is open-source. Please check the LICENSE file for details.