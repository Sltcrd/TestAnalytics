using WinesoftPlatform.API.Analytics.Domain.Model.Queries;
using WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;

namespace WinesoftPlatform.API.Analytics.Domain.Services;

public interface IAnalyticsQueryService
{
    Task<IEnumerable<PurchaseOrderResource>> Handle(GetPurchaseOrdersLast7DaysQuery query);
    
    Task<IEnumerable<SupplyLevelResource>> HandleGetSupplyLevels();
    
    Task<IEnumerable<LowStockAlertResource>> HandleGetLowStockAlerts();
    
    Task<IEnumerable<SupplyRotationResource>> HandleGetSupplyRotation(GetAnalyticsMetricsQuery query);
    
    Task<CostsSummaryResource> HandleGetCostsSummary(GetAnalyticsMetricsQuery query);
}