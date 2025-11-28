using Microsoft.EntityFrameworkCore;
using WinesoftPlatform.API.Analytics.Domain.Model.Queries;
using WinesoftPlatform.API.Analytics.Domain.Services;
using WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;
using WinesoftPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace WinesoftPlatform.API.Analytics.Application.Internal.QueryServices;

public class AnalyticsQueryService : IAnalyticsQueryService
{
    private readonly AppDbContext _context;

    public AnalyticsQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PurchaseOrderResource>> Handle(GetPurchaseOrdersLast7DaysQuery query)
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedDate >= sevenDaysAgo)
            .OrderByDescending(o => o.CreatedDate)
            .Select(o => new PurchaseOrderResource(
                o.Id,
                o.Status,
                o.CreatedDate.Value.DateTime,
                o.ProductId,
                o.Quantity
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplyLevelResource>> HandleGetSupplyLevels()
    {
        return await _context.Supplies
            .AsNoTracking()
            .GroupBy(s => s.SupplyName)
            .Select(g => new SupplyLevelResource(
                g.Key,
                g.Sum(s => s.Quantity)
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<LowStockAlertResource>> HandleGetLowStockAlerts()
    {
        {
            // Definimos un umbral fijo para pruebas ej: 10 unidades
            int defaultThreshold = 10;

            return await _context.Supplies
                .AsNoTracking()
                .Where(s => s.Quantity < defaultThreshold) // Filtra productos con menos de 10 unidades
                .Select(s => new LowStockAlertResource(
                    s.SupplyName, 
                    s.Quantity, 
                    defaultThreshold
                ))
                .ToListAsync();
        }
    }

    public async Task<IEnumerable<SupplyRotationResource>> HandleGetSupplyRotation(GetAnalyticsMetricsQuery query)
    {
        var endDate = query.EndDate ?? DateTime.UtcNow;
        var startDate = query.StartDate ?? endDate.AddDays(-7);

        var rawSupplies = await _context.Supplies
            .AsNoTracking()
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .ToListAsync();
        
        // PASO 2: Agrupar y proyectar en Memoria (C#)
        var result = rawSupplies
            .GroupBy(s => s.Date.Date)
            .Select(g => new SupplyRotationResource(
                g.Key,
                g.Count()
            ))
            .OrderBy(r => r.Day)
            .ToList();

        return result;
    }

    public async Task<CostsSummaryResource> HandleGetCostsSummary(GetAnalyticsMetricsQuery query)
    {
        var endDate = query.EndDate ?? DateTime.UtcNow;
        var startDate = query.StartDate ?? endDate.AddDays(-30);

        var totalCost = await _context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
            .Join(_context.Supplies,
                order => order.ProductId,
                supply => supply.Id,
                (order, supply) => new { order.Quantity, supply.Price }
            )
            .SumAsync(x => (double)x.Quantity * (double)x.Price);

        return new CostsSummaryResource(totalCost, startDate, endDate);
    }
}