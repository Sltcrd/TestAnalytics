using WinesoftPlatform.API.Analytics.Domain.Model.ValueObjects;

namespace WinesoftPlatform.API.Analytics.Domain.Services;

/// <summary>
/// Service for building analytics reports
/// </summary>
public interface IAnalyticsReportBuilder
{
    /// <summary>
    /// Generates a PDF report for the specified period and widgets
    /// </summary>
    /// <param name="period">The report period</param>
    /// <param name="widgets">List of widgets to include</param>
    /// <returns>PDF document as byte array</returns>
    Task<byte[]> GeneratePdfReportAsync(ReportPeriod period, IEnumerable<WidgetType> widgets, string language);
}