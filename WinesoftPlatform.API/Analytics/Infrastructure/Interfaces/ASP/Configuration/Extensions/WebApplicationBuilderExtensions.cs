using WinesoftPlatform.API.Analytics.Application.Internal.CommandServices;
using WinesoftPlatform.API.Analytics.Application.Internal.QueryServices;
using WinesoftPlatform.API.Analytics.Domain.Services;
using WinesoftPlatform.API.Analytics.Infrastructure.Services;

namespace WinesoftPlatform.API.Analytics.Infrastructure.Interfaces.ASP.Configuration.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddAnalyticsContextServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();
        builder.Services.AddScoped<IAnalyticsCommandService, AnalyticsCommandService>();
        builder.Services.AddScoped<IAnalyticsReportBuilder, QuestPdfAnalyticsReportBuilder>();
    }
}