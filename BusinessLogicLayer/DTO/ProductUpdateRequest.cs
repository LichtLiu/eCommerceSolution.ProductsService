namespace BusinessLogicLayer.DTO;

/// <summary>
/// 修改商品的請求資料(要帶 ProductID 指出修改哪一筆)。
/// </summary>
public record ProductUpdateRequest(
    Guid ProductID,
    string? ProductName,
    string? Category,
    double? UnitPrice,
    int? QuantityInStock);
