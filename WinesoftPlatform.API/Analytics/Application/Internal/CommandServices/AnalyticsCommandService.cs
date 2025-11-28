using WinesoftPlatform.API.Analytics.Domain.Model.Commands;
using WinesoftPlatform.API.Analytics.Domain.Model.ValueObjects;
using WinesoftPlatform.API.Analytics.Domain.Services;

namespace WinesoftPlatform.API.Analytics.Application.Internal.CommandServices;

/// <summary>
/// Implementation of analytics command service
/// </summary>
public class AnalyticsCommandService : IAnalyticsCommandService
{
    private readonly IAnalyticsReportBuilder _reportBuilder;

    public AnalyticsCommandService(IAnalyticsReportBuilder reportBuilder)
    {
        _reportBuilder = reportBuilder;
    }

    public async Task<byte[]> Handle(GenerateAnalyticsReportCommand command)
    {
        var period = new ReportPeriod(command.StartDate, command.EndDate);
        period.Validate();

        var widgets = command.Widgets
            .Select(w => Enum.Parse<WidgetType>(w, ignoreCase: true))
            .ToList();

        return await _reportBuilder.GeneratePdfReportAsync(period, widgets);
    }
}