using Devices.Application.Interfaces;
using Devices.Application.Services;
using Devices.Application.Validators;
using Devices.Domain.Interfaces;
using Devices.Infrastructure;
using Devices.Infrastructure.Extensions.DependencyInjection;
using Devices.Infrastructure.Repositories;
using DeviceWebApi.Middlewares;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException(nameof(connectionString), "Connection String must be defined in appsettings.json or environment variables");
}

builder.Services.AddEFDbContext(connectionString);

//Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateDeviceCommandValidator>();

// Repositories
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

// Application Services
builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Devices API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Verificar se o banco existe e aplicar migrations
    if (!dbContext.Database.CanConnect())
    {
        dbContext.Database.EnsureCreated();
    }
    else
    {
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();