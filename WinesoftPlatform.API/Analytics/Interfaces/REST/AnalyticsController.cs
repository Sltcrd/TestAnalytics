using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WinesoftPlatform.API.Analytics.Domain.Model.Queries;
using WinesoftPlatform.API.Analytics.Domain.Services;
using WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;
using WinesoftPlatform.API.Analytics.Interfaces.REST.Transform;

namespace WinesoftPlatform.API.Analytics.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[SwaggerTag("Analytics endpoints for metrics and reports")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsQueryService _analyticsQueryService;
    private readonly IAnalyticsCommandService _analyticsCommandService;

    public AnalyticsController(IAnalyticsQueryService analyticsQueryService,
        IAnalyticsCommandService analyticsCommandService)
    {
        _analyticsQueryService = analyticsQueryService;
        _analyticsCommandService = analyticsCommandService;
    }

    [HttpGet("purchase-orders/last-7-days")]
    [SwaggerOperation(
        Summary = "Get purchase orders from last 7 days",
        Description = "Retrieves all purchase orders created in the last 7 days",
        OperationId = "GetPurchaseOrdersLast7Days")]
    [SwaggerResponse(200, "Purchase orders retrieved successfully")]
    public async Task<IActionResult> GetPurchaseOrdersLast7Days()
    {
        var query = new GetPurchaseOrdersLast7DaysQuery();
        var orders = await _analyticsQueryService.Handle(query);
        return Ok(orders);
    }

    [HttpGet("supply-levels")]
    [SwaggerOperation(
        Summary = "Get current supply levels",
        Description = "Retrieves current inventory levels for all supplies",
        OperationId = "GetSupplyLevels")]
    [SwaggerResponse(200, "Supply levels retrieved successfully")]
    public async Task<IActionResult> GetSupplyLevels()
    {
        var levels = await _analyticsQueryService.HandleGetSupplyLevels();
        return Ok(levels);
    }

    [HttpGet("low-stock-alerts")]
    [SwaggerOperation(
        Summary = "Get low stock alerts",
        Description = "Retrieves alerts for supplies below minimum stock threshold",
        OperationId = "GetLowStockAlerts")]
    [SwaggerResponse(200, "Low stock alerts retrieved successfully")]
    public async Task<IActionResult> GetLowStockAlerts()
    {
        var alerts = await _analyticsQueryService.HandleGetLowStockAlerts();
        return Ok(alerts);
    }

    [HttpGet("supply-rotation")]
    [SwaggerOperation(
        Summary = "Get supply rotation data",
        Description = "Retrieves daily supply rotation metrics for the specified date range",
        OperationId = "GetSupplyRotation")]
    [SwaggerResponse(200, "Supply rotation data retrieved successfully")]
    public async Task<IActionResult> GetSupplyRotation([FromQuery] GetAnalyticsMetricsQuery query)
    {
        var data = await _analyticsQueryService.HandleGetSupplyRotation(query);
        return Ok(data);
    }

    [HttpGet("costs-summary")]
    [SwaggerOperation(
        Summary = "Get costs summary",
        Description = "Retrieves total costs summary for the specified date range",
        OperationId = "GetCostsSummary")]
    [SwaggerResponse(200, "Costs summary retrieved successfully")]
    public async Task<IActionResult> GetCostsSummary([FromQuery] GetAnalyticsMetricsQuery query)
    {
        var data = await _analyticsQueryService.HandleGetCostsSummary(query);
        return Ok(data);
    }

    [HttpPost("reports")]
    [SwaggerOperation(
        Summary = "Generate analytics report",
        Description = "Generates a PDF report for the specified period and widgets",
        OperationId = "GenerateAnalyticsReport")]
    [SwaggerResponse(200, "Report generated successfully", typeof(FileContentResult))]
    [SwaggerResponse(400, "Invalid request parameters")]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateReportResource resource)
    {
        var command = GenerateAnalyticsReportCommandFromResourceAssembler.ToCommandFromResource(resource);

        try
        {
            var pdfBytes = await _analyticsCommandService.Handle(command);

            return File(pdfBytes, "application/pdf", $"analytics-report-{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
