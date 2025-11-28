using WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;

namespace WinesoftPlatform.API.Analytics.Domain.Model.Aggregates;

public class AnalyticsReportData
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public CostsSummaryResource? CostSummary { get; }
    public IEnumerable<RecentOrderResource> Orders { get; }
    public IEnumerable<SupplyRotationResource> SupplyRotation { get; }
    public IEnumerable<SupplyLevelResource> SupplyLevels { get; }
    public IEnumerable<LowStockAlertResource> LowStockAlerts { get; }

    public AnalyticsReportData(
        DateTime startDate,
        DateTime endDate,
        CostsSummaryResource? costSummary,
        IEnumerable<RecentOrderResource> orders,
        IEnumerable<SupplyRotationResource> supplyRotation,
        IEnumerable<SupplyLevelResource> supplyLevels,
        IEnumerable<LowStockAlertResource> lowStockAlerts)
    {
        StartDate = startDate;
        EndDate = endDate;
        CostSummary = costSummary;
        Orders = orders ?? Enumerable.Empty<RecentOrderResource>();
        SupplyRotation = supplyRotation ?? Enumerable.Empty<SupplyRotationResource>();
        SupplyLevels = supplyLevels ?? Enumerable.Empty<SupplyLevelResource>();
        LowStockAlerts = lowStockAlerts ?? Enumerable.Empty<LowStockAlertResource>();
    }
}