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
using Microsoft.OpenApi.Models;

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

// Register CameraService
builder.Services.AddScoped<ICameraService, CameraService>();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Zona-Parkir API", 
        Version = "v1",
        Description = "API for Zona-Parkir parking management system"
    });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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
    // Enable Swagger UI in development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zona-Parkir API V1");
        c.RoutePrefix = "swagger";
    });
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