namespace WinesoftPlatform.API.Analytics.Domain.Model.Commands;

/// <summary>
/// Command to generate an analytics report
/// </summary>
/// <param name="StartDate">Start date for the report period</param>
/// <param name="EndDate">End date for the report period</param>
/// <param name="Widgets">List of widget identifiers to include in the report</param>
public record GenerateAnalyticsReportCommand(
    DateTime StartDate,
    DateTime EndDate,
    IEnumerable<string> Widgets
);