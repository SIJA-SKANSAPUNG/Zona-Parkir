using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.Data;

namespace Parking_Zone.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Core services
            services.AddScoped<IParkingFeeService, ParkingFeeService>();
            services.AddScoped<IParkingGateService, ParkingGateService>();
            services.AddScoped<IParkingTransactionService, ParkingTransactionService>();
            services.AddScoped<IUserService, UserService>();

            // Hardware services
            services.AddSingleton<IPCameraService, PCameraService>();
            services.AddSingleton<IPrinterService, PrinterService>();
            services.AddSingleton<IScannerService, ScannerService>();
            services.AddSingleton<ITicketService, TicketService>();

            // Notification services
            services.AddScoped<IParkingNotificationService, ParkingNotificationService>();

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5000",
                                          "https://localhost:5001")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                    options.Cookie.Name = "ParkingZone.Auth";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(12);
                    options.SlidingExpiration = true;
                });

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("RequireOperatorRole", policy =>
                    policy.RequireRole("Operator"));

                options.AddPolicy("RequireSupervisorRole", policy =>
                    policy.RequireRole("Supervisor"));
            });

            return services;
        }

        public static IServiceCollection AddCustomHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddCheck<ParkingGateHealthCheck>("ParkingGates")
                .AddCheck<PrinterHealthCheck>("Printers")
                .AddCheck<CameraHealthCheck>("Cameras");

            return services;
        }

        public static IServiceCollection AddCustomMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024;
                options.CompactionPercentage = 0.2;
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            });

            return services;
        }

        public static IServiceCollection AddCustomSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
                options.StreamBufferCapacity = 10;
            });

            return services;
        }
    }
}