using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WinesoftPlatform.API.Analytics.Domain.Model.ValueObjects;
using WinesoftPlatform.API.Analytics.Domain.Services;
using WinesoftPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace WinesoftPlatform.API.Analytics.Infrastructure.Services;

/// <summary>
/// QuestPDF implementation of analytics report builder
/// </summary>
public class QuestPdfAnalyticsReportBuilder : IAnalyticsReportBuilder
{
    private readonly AppDbContext _context;

    public QuestPdfAnalyticsReportBuilder(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GeneratePdfReportAsync(ReportPeriod period, IEnumerable<WidgetType> widgets)
    {
        var widgetList = widgets.ToList();
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeContent(c, period, widgetList));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("WineSoft Platform").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                column.Item().Text("Analytics Report").FontSize(14).FontColor(Colors.Grey.Darken2);
            });

            row.ConstantItem(100).Height(50).Placeholder();
        });
    }

    private void ComposeContent(IContainer container, ReportPeriod period, List<WidgetType> widgets)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(10);

            column.Item().Text($"Report Period: {period.StartDate:yyyy-MM-dd} to {period.EndDate:yyyy-MM-dd}")
                .FontSize(12).SemiBold();

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            foreach (var widget in widgets)
            {
                column.Item().PaddingTop(15).Element(c => ComposeWidget(c, widget, period));
            }
        });
    }

    private void ComposeWidget(IContainer container, WidgetType widget, ReportPeriod period)
    {
        container.Column(column =>
        {
            column.Item().Text(widget.ToString()).FontSize(14).SemiBold().FontColor(Colors.Blue.Darken1);
            
            column.Item().PaddingTop(5).Element(c => ComposeWidgetContent(c, widget, period));
            
            column.Item().PaddingTop(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
        });
    }

    private void ComposeWidgetContent(IContainer container, WidgetType widget, ReportPeriod period)
    {
        switch (widget)
        {
            case WidgetType.PurchaseOrders:
                ComposePurchaseOrdersWidget(container, period);
                break;
            case WidgetType.SupplyLevels:
                ComposeSupplyLevelsWidget(container);
                break;
            case WidgetType.LowStockAlerts:
                ComposeLowStockAlertsWidget(container);
                break;
            case WidgetType.SupplyRotation:
                ComposeSupplyRotationWidget(container, period);
                break;
            case WidgetType.CostsSummary:
                ComposeCostsSummaryWidget(container, period);
                break;
        }
    }

    private void ComposePurchaseOrdersWidget(IContainer container, ReportPeriod period)
    {
        var orders = _context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedDate >= period.StartDate && o.CreatedDate <= period.EndDate)
            .OrderByDescending(o => o.CreatedDate)
            .Take(20)
            .ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(50);
                columns.RelativeColumn();
                columns.ConstantColumn(80);
                columns.ConstantColumn(100);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("ID");
                header.Cell().Element(CellStyle).Text("Status");
                header.Cell().Element(CellStyle).Text("Quantity");
                header.Cell().Element(CellStyle).Text("Date");

                static IContainer CellStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
            });

            foreach (var order in orders)
            {
                table.Cell().Element(CellStyle).Text(order.Id.ToString());
                table.Cell().Element(CellStyle).Text(order.Status);
                table.Cell().Element(CellStyle).AlignRight().Text(order.Quantity.ToString());
                table.Cell().Element(CellStyle).Text(order.CreatedDate?.DateTime.ToString("yyyy-MM-dd") ?? "N/A");

                static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }

    private void ComposeSupplyLevelsWidget(IContainer container)
    {
        var supplies = _context.Supplies
            .AsNoTracking()
            .GroupBy(s => s.SupplyName)
            .Select(g => new { Name = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(15)
            .ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.ConstantColumn(100);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Supply Name");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");

                static IContainer CellStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
            });

            foreach (var supply in supplies)
            {
                table.Cell().Element(CellStyle).Text(supply.Name);
                table.Cell().Element(CellStyle).AlignRight().Text(supply.TotalQuantity.ToString());

                static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }

    private void ComposeLowStockAlertsWidget(IContainer container)
    {
        container.PaddingVertical(5).Text("No low stock alerts configured")
            .FontSize(10).Italic().FontColor(Colors.Grey.Medium);
    }

    private void ComposeSupplyRotationWidget(IContainer container, ReportPeriod period)
    {
        var rotation = _context.Supplies
            .AsNoTracking()
            .Where(s => s.Date >= period.StartDate && s.Date <= period.EndDate)
            .GroupBy(s => s.Date.Date)
            .Select(g => new { Date = g.Key, Movements = g.Count() })
            .OrderBy(x => x.Date)
            .ToList();

        container.Column(column =>
        {
            column.Item().Text($"Total days with activity: {rotation.Count}").FontSize(10);
            column.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(120);
                    columns.ConstantColumn(100);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Date");
                    header.Cell().Element(CellStyle).AlignRight().Text("Movements");

                    static IContainer CellStyle(IContainer c) => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                foreach (var item in rotation.Take(10))
                {
                    table.Cell().Element(CellStyle).Text(item.Date.ToString("yyyy-MM-dd"));
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Movements.ToString());

                    static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            });
        });
    }

    private void ComposeCostsSummaryWidget(IContainer container, ReportPeriod period)
    {
        var totalCost = _context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedDate >= period.StartDate && o.CreatedDate <= period.EndDate)
            .Join(_context.Supplies,
                order => order.ProductId,
                supply => supply.Id,
                (order, supply) => new { order.Quantity, supply.Price }
            )
            .Sum(x => (double)x.Quantity * (double)x.Price);

        container.Column(column =>
        {
            column.Item().Text($"Total Cost: ${totalCost:N2}")
                .FontSize(16).SemiBold().FontColor(Colors.Green.Darken2);
            column.Item().PaddingTop(5).Text($"Period: {period.DaysDuration} days")
                .FontSize(10).FontColor(Colors.Grey.Darken1);
        });
    }
}