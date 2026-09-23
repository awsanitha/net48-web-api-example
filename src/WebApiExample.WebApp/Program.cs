using Microsoft.EntityFrameworkCore;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore;
using WebApiExample.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC + Web API controllers with JSON camelCase naming
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add Swagger / OpenAPI (replaces HelpPage area)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework DbContext (replaces EF6 UserContext constructor initialiser)
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services (replaces Unity DI config)
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UserContext>());
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// API controllers use attribute routing ([Route("api/[controller]")])
app.MapControllers();

// MVC conventional route (equivalent to old RouteConfig.RegisterRoutes)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
