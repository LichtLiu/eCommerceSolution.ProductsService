namespace BusinessLogicLayer.DTO;

/// <summary>
/// 回傳給 client 的商品資料。
/// </summary>
public record ProductResponse(
    Guid ProductID,
    string? ProductName,
    string? Category,
    double? UnitPrice,
    int? QuantityInStock);
