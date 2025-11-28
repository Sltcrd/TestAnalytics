using System.ComponentModel.DataAnnotations;

namespace WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;

/// <summary>
/// Resource for generating an analytics report
/// </summary>
/// <param name="StartDate">Start date for the report period</param>
/// <param name="EndDate">End date for the report period</param>
/// <param name="Widgets">List of widget names to include</param>
public record GenerateReportResource(
    [Required] DateTime StartDate,
    [Required] DateTime EndDate,
    [Required] IEnumerable<string> Widgets
);