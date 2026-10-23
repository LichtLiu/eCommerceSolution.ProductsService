using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.ServiceContracts;

/// <summary>
/// 商品的商業邏輯合約。回傳 DTO,不暴露 Entity。
/// </summary>
public interface IProductsService
{
    /// <summary>取得所有商品</summary>
    Task<List<ProductResponse?>> GetProducts();

    /// <summary>搜尋商品名稱或分類(包含比對)</summary>
    Task<List<ProductResponse?>> GetProductsByCondition(string searchString);

    /// <summary>依 ID 取得單一商品,找不到回傳 null</summary>
    Task<ProductResponse?> GetProductByProductID(Guid productID);

    /// <summary>新增商品,失敗回傳 null</summary>
    Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);

    /// <summary>修改商品,找不到回傳 null</summary>
    Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);

    /// <summary>刪除商品,成功回傳 true</summary>
    Task<bool> DeleteProduct(Guid productID);
}
