namespace WinesoftPlatform.API.Analytics.Interfaces.REST.Resources;

/// <summary>
/// Resource representing a purchase order for analytics
/// </summary>
/// <param name="OrderId">The unique identifier of the order</param>
/// <param name="Status">Current status of the order</param>
/// <param name="Date">Date when the order was created</param>
/// <param name="ProductId">Identifier of the product ordered</param>
/// <param name="Quantity">Quantity ordered</param>
public record PurchaseOrderResource(
    int OrderId,
    string Status,
    DateTime Date,
    int ProductId,
    int Quantity
);