namespace BusinessLogicLayer.DTO;

/// <summary>
/// 新增商品的請求資料(沒有 ProductID,由伺服器產生)。
/// </summary>
public record ProductAddRequest(
    string? ProductName,
    string? Category,
    double? UnitPrice,
    int? QuantityInStock);
