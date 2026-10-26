using Microsoft.EntityFrameworkCore;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore;
using WebApiExample.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserContext>(options =>
    options.UseInMemoryDatabase("WebApiExample"));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UserContext>());
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
