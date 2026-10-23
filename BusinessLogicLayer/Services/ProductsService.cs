using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;

namespace BusinessLogicLayer.Services;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IMapper _mapper;

    public ProductsService(IProductsRepository productsRepository, IMapper mapper)
    {
        _productsRepository = productsRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        IEnumerable<Product> products = await _productsRepository.GetProducts();

        return _mapper.Map<IEnumerable<ProductResponse?>>(products).ToList();
    }

    public async Task<List<ProductResponse?>> GetProductsByCondition(string searchString)
    {
        // Contains 會被 EF 翻成 SQL LIKE '%...%',同時比對商品名稱與分類
        IEnumerable<Product> products = await _productsRepository.GetProductsByCondition(
            p => (p.ProductName != null && p.ProductName.Contains(searchString))
              || (p.Category != null && p.Category.Contains(searchString)));

        return _mapper.Map<IEnumerable<ProductResponse?>>(products).ToList();
    }

    public async Task<ProductResponse?> GetProductByProductID(Guid productID)
    {
        Product? product = await _productsRepository.GetProductByCondition(p => p.ProductID == productID);

        if (product == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        // Request -> Entity,主鍵由 Repository 產生
        Product product = _mapper.Map<Product>(productAddRequest);

        Product? addedProduct = await _productsRepository.AddProduct(product);

        if (addedProduct == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(addedProduct);
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        Product product = _mapper.Map<Product>(productUpdateRequest);

        Product? updatedProduct = await _productsRepository.UpdateProduct(product);

        if (updatedProduct == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(updatedProduct);
    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        // 先確認存在,方便之後在這裡加商業規則(例如庫存不為 0 不可刪)
        Product? existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productID);

        if (existingProduct == null)
        {
            return false;
        }

        return await _productsRepository.DeleteProduct(productID);
    }
}
