using WinesoftPlatform.API.Analytics.Domain.Model.Commands;

namespace WinesoftPlatform.API.Analytics.Domain.Services;

/// <summary>
/// Service for handling analytics commands
/// </summary>
public interface IAnalyticsCommandService
{
    /// <summary>
    /// Handles the generate analytics report command
    /// </summary>
    /// <param name="command">The command to generate a report</param>
    /// <returns>PDF document as byte array</returns>
    Task<byte[]> Handle(GenerateAnalyticsReportCommand command);
}