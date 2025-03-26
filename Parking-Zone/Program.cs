using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Parking_Zone.Data;
using Parking_Zone.Middleware;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add DbContextFactory
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseNpgsql(connectionString));

// Add SignalR services
builder.Services.AddSignalR();

builder.Services.AddScoped<IParkingZoneRepository, ParkingZoneRepository>();
builder.Services.AddScoped<IParkingZoneService, ParkingZoneService>();
builder.Services.AddScoped<IParkingSlotRepository, ParkingSlotRepository>();
builder.Services.AddScoped<IParkingSlotService, ParkingSlotService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// Register new services
builder.Services.AddScoped<IParkingFeeService, ParkingFeeService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IParkingGateService, ParkingGateService>();
builder.Services.AddScoped<IParkingTransactionService, ParkingTransactionService>();
builder.Services.AddScoped<IParkingNotificationService, ParkingNotificationService>();

// Register hardware integration services
builder.Services.AddScoped<IIPCameraService, IPCameraService>();
builder.Services.AddScoped<IPrinterService, PrinterService>();
builder.Services.AddScoped<IScannerService, ScannerService>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiting();

app.UseAuthorization();

// Add SignalR hub mapping
app.MapHub<ParkingHub>("/parkingHub");

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=ParkingZone}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();