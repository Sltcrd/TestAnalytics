using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using WinesoftPlatform.API.Analytics.Domain.Services;
using WinesoftPlatform.API.Shared.Domain.Repositories;
using WinesoftPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using WinesoftPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using WinesoftPlatform.API.Shared.Infrastructure.Interfaces.ASAP.Configuration;
using WinesoftPlatform.API.Orders.Domain.Repositories;
using WinesoftPlatform.API.Orders.Infrastructure.Persistence.EFC.Repositories;
using WinesoftPlatform.API.Inventory.Domain.Repositories;
using WinesoftPlatform.API.Inventory.Domain.Services;
using WinesoftPlatform.API.Inventory.Application.Internal.CommandServices;
using WinesoftPlatform.API.Inventory.Application.Internal.QueryServices;
using WinesoftPlatform.API.Inventory.Infrastructure.Persistence.Repositories;
using WinesoftPlatform.API.IAM.Infrastructure.Extensions;
using WinesoftPlatform.API.Analytics.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using WinesoftPlatform.API.Analytics.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WineSoft Platform API",
        Version = "v1",
        Description = "API for the WineSoft inventory and order management platform.",});
    options.EnableAnnotations();
});
builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContext<AppDbContext>(options => {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (connectionString is null) 
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    });
else if (builder.Environment.IsProduction())
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        var connectionStringTemplate = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionStringTemplate)) 
            throw new Exception("Database connection string template is not set in the configuration.");
        var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Database connection string is not set in the configuration.");
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ISupplyRepository, SupplyRepository>();
builder.Services.AddScoped<ISupplyCommandService, SupplyCommandService>();
builder.Services.AddScoped<ISupplyQueryService, SupplyQueryService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register IAM module: HTTP client to external login service, repository and application services.
// Expected configuration in appsettings: "IAM:AuthBaseUrl" (base URL) and "IAM:SigninPath" (signin path).
builder.Services.AddIAM(builder.Configuration);
builder.AddAnalyticsContextServices();

builder.Services.AddLocalization();
builder.Services.AddScoped<IAnalyticsReportBuilder, QuestPdfAnalyticsReportBuilder>();

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();
