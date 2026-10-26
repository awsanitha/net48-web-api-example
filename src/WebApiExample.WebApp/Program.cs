using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore;
using WebApiExample.WebApp.Services;

namespace WebApiExample.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Replaces FilterConfig.RegisterGlobalFilters (HandleErrorAttribute):
            // MVC view errors are handled by the exception handler middleware below.
            builder.Services
                .AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                });

            // The controllers validate ModelState explicitly, as they did under Web API 2.
            builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Replaces UnityConfig.RegisterComponents.
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UserContext")));
            builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UserContext>());
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            // Replaces RouteConfig.RegisterRoutes.
            app.MapControllerRoute(
                name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Replaces WebApiConfig.Register (attribute routes + the DefaultApi route).
            app.MapControllers();

            // Replaces Database.SetInitializer(new CreateDatabaseIfNotExists<UserContext>()).
            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<UserContext>().Database.EnsureCreated();
            }

            app.Run();
        }
    }
}
