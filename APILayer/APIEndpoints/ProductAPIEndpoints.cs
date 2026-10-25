using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;

namespace APILayer.APIEndpoints;

public static class ProductAPIEndpoints
{
    /// <summary>
    /// 註冊所有 /api/products 的 Minimal API 端點。
    /// </summary>
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        // 群組:所有端點共用 /api/products 前綴
        RouteGroupBuilder productsGroup = app.MapGroup("/api/products");

        // GET /api/products
        productsGroup.MapGet("/", async (IProductsService productsService) =>
        {
            List<ProductResponse?> products = await productsService.GetProducts();
            return Results.Ok(products);
        });

        // GET /api/products/search/product-id/{productID}
        productsGroup.MapGet("/search/product-id/{productID:guid}", async (IProductsService productsService, Guid productID) =>
        {
            ProductResponse? product = await productsService.GetProductByProductID(productID);

            return product == null ? Results.NotFound() : Results.Ok(product);
        });

        // GET /api/products/search/{searchString}
        productsGroup.MapGet("/search/{searchString}", async (IProductsService productsService, string searchString) =>
        {
            List<ProductResponse?> products = await productsService.GetProductsByCondition(searchString);
            return Results.Ok(products);
        });

        // POST /api/products
        productsGroup.MapPost("/", async (
            IProductsService productsService,
            IValidator<ProductAddRequest> validator,
            ProductAddRequest productAddRequest) =>
        {
            // 先驗證,不合法就回 400,不進 Service
            ValidationResult validationResult = await validator.ValidateAsync(productAddRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(ToErrorDictionary(validationResult));
            }

            ProductResponse? addedProduct = await productsService.AddProduct(productAddRequest);

            if (addedProduct == null)
            {
                return Results.Problem("Error in adding product");
            }

            // 201 Created,並用 Location header 告訴 client 新資源的網址
            return Results.Created($"/api/products/search/product-id/{addedProduct.ProductID}", addedProduct);
        });

        // PUT /api/products
        productsGroup.MapPut("/", async (
            IProductsService productsService,
            IValidator<ProductUpdateRequest> validator,
            ProductUpdateRequest productUpdateRequest) =>
        {
            ValidationResult validationResult = await validator.ValidateAsync(productUpdateRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(ToErrorDictionary(validationResult));
            }

            ProductResponse? updatedProduct = await productsService.UpdateProduct(productUpdateRequest);

            return updatedProduct == null ? Results.NotFound() : Results.Ok(updatedProduct);
        });

        // DELETE /api/products/{productID}
        productsGroup.MapDelete("/{productID:guid}", async (IProductsService productsService, Guid productID) =>
        {
            bool isDeleted = await productsService.DeleteProduct(productID);

            return isDeleted ? Results.Ok(true) : Results.NotFound();
        });

        return app;
    }

    /// <summary>
    /// 把 FluentValidation 的結果整理成 { 欄位名: [錯誤訊息...] },給 ValidationProblem 用。
    /// </summary>
    private static Dictionary<string, string[]> ToErrorDictionary(ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
