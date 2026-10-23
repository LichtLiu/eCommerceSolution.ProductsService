using System.Linq.Expressions;
using DataAccessLayer.Entities;

namespace DataAccessLayer.RepositoryContracts;

/// <summary>
/// 定義 Products 資料表的存取合約(CRUD)。
/// </summary>
public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProducts();
    /// <summary>依條件取得多筆商品(給「搜尋」用)</summary>
    Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);

    /// <summary>依條件取得單筆商品,找不到回傳 null</summary>
    Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);

    /// <summary>新增商品,回傳新增後的商品;失敗回傳 null</summary>
    Task<Product?> AddProduct(Product product);

    /// <summary>更新商品,回傳更新後的商品;找不到回傳 null</summary>
    Task<Product?> UpdateProduct(Product product);

    /// <summary>依 ID 刪除商品,成功回傳 true</summary>
    Task<bool> DeleteProduct(Guid productID);
}
