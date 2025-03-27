using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Parking_Zone.Hubs;

namespace Parking_Zone.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    
                    var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = 500,
                            Message = "An internal server error occurred.",
                            DetailedMessage = app.ApplicationServices.GetService<IHostEnvironment>()?.IsDevelopment() == true 
                                ? ex.Message 
                                : null
                        });
                    }
                });
            });

            return app;
        }

        public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
        {
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            return app;
        }

        public static IApplicationBuilder UseCustomHsts(this IApplicationBuilder app, IHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            return app;
        }

        public static IApplicationBuilder UseCustomHttpsRedirection(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            return app;
        }

        public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            return app;
        }

        public static IApplicationBuilder UseCustomRouting(this IApplicationBuilder app)
        {
            app.UseRouting();
            return app;
        }

        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public static IApplicationBuilder UseCustomEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
                endpoints.MapHub<ParkingHub>("/parkingHub");
            });

            return app;
        }
    }
} 