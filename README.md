# eCommerce Products Microservice

A Products microservice built with ASP.NET Core (.NET 10), Minimal APIs, EF Core and MySQL.
It manages product data (create, read, update, delete, search) and is layered as follows:

| Project | Responsibility |
|---|---|
| `DataAccessLayer` | `Product` entity, `ApplicationDbContext`, `IProductsRepository` / `ProductsRepository`, EF Core migrations, DI registration |
| `BusinessLogicLayer` | DTOs, AutoMapper profile, `ProductsService`, FluentValidation validators, DI registration |
| `APILayer` | Minimal API endpoints, exception handling middleware, Swagger, CORS, `Program.cs` |

Dependency direction: `APILayer → BusinessLogicLayer → DataAccessLayer`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL 8 (a Docker command is provided below)
- EF Core CLI tool: `dotnet tool install --global dotnet-ef`

## How to run

1. **Start MySQL** (skip if you already have one):

   ```bash
   docker run -d --name products-mysql \
     -e MYSQL_ROOT_PASSWORD=mysql123 \
     -e MYSQL_DATABASE=ecommerce_products \
     -p 3306:3306 mysql:8
   ```

2. **Check the connection string** in `APILayer/appsettings.json` and adjust it to your MySQL:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Port=3306;Database=ecommerce_products;User=root;Password=mysql123"
   }
   ```

3. **Create the database schema** (applies the `InitialCreate` migration):

   ```bash
   dotnet ef database update --project DataAccessLayer --startup-project APILayer
   ```

4. **Run the API:**

   ```bash
   dotnet run --project APILayer --launch-profile http
   ```

   The API listens on `http://localhost:5161`. Swagger UI: `http://localhost:5161/swagger`.

## API endpoints

| Method | Route | Description | Success | Errors |
|---|---|---|---|---|
| GET | `/api/products` | Get all products | 200 | |
| GET | `/api/products/search/product-id/{productID}` | Get a product by ID | 200 | 404 |
| GET | `/api/products/products/search/{searchString}` | "Contains" search on product name or category | 200 | |
| POST | `/api/products` | Add a product | 201 | 400 |
| PUT | `/api/products` | Update a product (body includes `productID`) | 200 | 400, 404 |
| DELETE | `/api/products/{productID}` | Delete a product | 200 | 404 |

Example request body for `POST /api/products`:

```json
{
  "productName": "Keyboard",
  "category": "Electronics",
  "unitPrice": 49.9,
  "quantityInStock": 10
}
```

## Validation

`ProductAddRequest` and `ProductUpdateRequest` are validated with FluentValidation before reaching the service.
Invalid requests return `400 Bad Request` with the errors grouped by field:

- `productName`, `category`: required, max 100 characters
- `unitPrice`, `quantityInStock`: required, must be `>= 0` (0 is allowed)
- `productID` (update only): required

## Error handling

`ExceptionHandlingMiddleware` wraps the whole pipeline. Any unhandled exception is logged
(with stack trace) and the client receives a generic `500` response:

```json
{ "error": "An unexpected error occurred. Please try again later." }
```

## CORS

Requests from the Angular frontend `http://localhost:4200` are allowed (configured in `Program.cs`).

## Notes

- The MySQL provider is the official `MySql.EntityFrameworkCore` package, because the community
  `Pomelo.EntityFrameworkCore.MySql` provider has no stable release for EF Core 10 yet.
  The provider method is therefore `UseMySQL` (all-caps "SQL").
- The route `/api/products/products/search/{searchString}` follows the assignment text literally.
- The connection string in `appsettings.json` is for local development only. Use your own credentials
  (e.g. `dotnet user-secrets`) anywhere else.
