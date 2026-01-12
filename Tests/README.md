# API Tests Documentation

## 📋 Tổng Quan

Thư mục này chứa các test projects cho tất cả các API trong hệ thống E-commerce Microservices.

## 🧪 Test Projects

### 1. **Catalog.API.Tests**

Test cho Catalog Service API endpoints.

**Test Coverage:**

- ✅ GetProductById
- ✅ GetProductByProductName
- ✅ GetAllProducts (với pagination, filters)
- ✅ GetAllBrands
- ✅ GetAllTypes
- ✅ GetAllProductByBrandName
- ✅ CreateProduct
- ✅ UpdateProduct
- ✅ DeleteProduct

### 2. **Basket.API.Tests**

Test cho Basket Service API endpoints.

**Test Coverage:**

- ✅ GetBasket
- ✅ CreateBasket
- ✅ DeleteBasket
- ✅ Checkout

### 3. **Ordering.API.Tests**

Test cho Ordering Service API endpoints.

**Test Coverage:**

- ✅ GetOrdersByUsername
- ✅ CheckoutOrder
- ✅ UpdateOrder
- ✅ DeleteOrder

### 4. **Discount.API.Tests**

Test cho Discount Service (gRPC và REST).

**Test Coverage:**

- ✅ gRPC endpoints
- ✅ Health checks

## 🚀 Chạy Tests

### Chạy tất cả tests:

```bash
dotnet test
```

### Chạy tests cho một project cụ thể:

```bash
dotnet test Tests/Catalog.API.Tests/Catalog.API.Tests.csproj
dotnet test Tests/Basket.API.Tests/Basket.API.Tests.csproj
dotnet test Tests/Ordering.API.Tests/Ordering.API.Tests.csproj
dotnet test Tests/Discount.API.Tests/Discount.API.Tests.csproj
```

### Chạy với coverage:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📝 Test Structure

Mỗi test project có cấu trúc:

```
Tests/
├── {Service}.API.Tests/
│   ├── IntegrationTests/
│   │   └── {Controller}Tests.cs
│   └── {Service}.API.Tests.csproj
```

## 🔧 Dependencies

- **xUnit** - Testing framework
- **FluentAssertions** - Assertions library
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing
- **Moq** - Mocking framework
- **Testcontainers** - Container-based testing (MongoDB, Redis, PostgreSQL, SQL Server)

## 📊 Test Coverage Goals

- **Unit Tests:** > 80%
- **Integration Tests:** > 70%
- **API Endpoints:** 100%

## 🎯 Best Practices

1. **Isolation:** Mỗi test độc lập, không phụ thuộc vào test khác
2. **Cleanup:** Cleanup test data sau mỗi test
3. **Naming:** Test names mô tả rõ ràng behavior được test
4. **AAA Pattern:** Arrange-Act-Assert pattern
5. **Mocking:** Mock external dependencies (databases, message queues)

## 🔍 Running Tests in CI/CD

Tests sẽ tự động chạy trong CI/CD pipeline với:

- Docker containers cho databases
- Testcontainers cho integration tests
- Coverage reports

## 📚 Additional Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
