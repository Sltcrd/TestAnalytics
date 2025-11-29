using WinesoftPlatform.API.Analytics.Domain.Model.Commands;
using WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;

namespace WinesoftPlatform.API.Analytics.Interfaces.REST.Transform;

/// <summary>
/// Assembler to convert GenerateReportResource to GenerateAnalyticsReportCommand
/// </summary>
public static class GenerateAnalyticsReportCommandFromResourceAssembler
{
    public static GenerateAnalyticsReportCommand ToCommandFromResource(GenerateReportResource resource)
    {
        return new GenerateAnalyticsReportCommand(
            resource.StartDate,
            resource.EndDate,
            resource.Widgets,
            resource.Language
        );
    }
}