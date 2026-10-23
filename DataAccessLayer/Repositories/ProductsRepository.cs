using System.Linq.Expressions;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _dbContext.Products.Where(conditionExpression).ToListAsync();
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<Product?> AddProduct(Product product)
    {
        // 主鍵由伺服器產生,不信任 client 傳來的值
        product.ProductID = Guid.NewGuid();

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        Product? existingProduct = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.ProductID == product.ProductID);

        if (existingProduct == null)
        {
            return null;
        }

        // 只更新可修改的欄位,主鍵不動
        existingProduct.ProductName = product.ProductName;
        existingProduct.Category = product.Category;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.QuantityInStock = product.QuantityInStock;

        await _dbContext.SaveChangesAsync();

        return existingProduct;
    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        Product? existingProduct = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.ProductID == productID);

        if (existingProduct == null)
        {
            return false;
        }

        _dbContext.Products.Remove(existingProduct);
        int affectedRows = await _dbContext.SaveChangesAsync();

        return affectedRows > 0;
    }
}
