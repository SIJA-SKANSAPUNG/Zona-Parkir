using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Parking_Zone.Data;
using Parking_Zone.Middleware;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parking_Zone.Extensions;
using Parking_Zone.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using our custom extensions
builder.Services
    .AddCustomDbContext(builder.Configuration)
    .AddCustomIdentity()
    .AddCustomServices()
    .AddCustomCors()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomAuthorization()
    .AddCustomHealthChecks(builder.Configuration)
    .AddCustomMemoryCache()
    .AddCustomSignalR();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
    };
});

// Add MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline using our custom extensions
if (!app.Environment.IsDevelopment())
{
    app.UseCustomExceptionHandler();
    app.UseCustomHsts(app.Environment);
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseCustomHttpsRedirection()
   .UseCustomStaticFiles()
   .UseCustomRouting()
   .UseCustomCors()
   .UseCustomAuthentication()
   .UseAuthentication()
   .UseAuthorization();

app.UseCustomEndpoints();

app.Run();