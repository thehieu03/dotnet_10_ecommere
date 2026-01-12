# Testing Guide - Ecommerce Microservices APIs

## 📋 Tổng Quan

Tài liệu này hướng dẫn cách chạy và viết tests cho tất cả các API trong hệ thống E-commerce Microservices.

## 🧪 Test Projects

### 1. Catalog.API.Tests

**Location:** `Tests/Catalog.API.Tests/`

**Test Coverage:**

- ✅ GetProductById - Test lấy sản phẩm theo ID
- ✅ GetProductByProductName - Test tìm sản phẩm theo tên
- ✅ GetAllProducts - Test lấy danh sách sản phẩm với pagination và filters
- ✅ GetAllBrands - Test lấy danh sách thương hiệu
- ✅ GetAllTypes - Test lấy danh sách loại sản phẩm
- ✅ GetAllProductByBrandName - Test lấy sản phẩm theo thương hiệu
- ✅ CreateProduct - Test tạo sản phẩm mới
- ✅ UpdateProduct - Test cập nhật sản phẩm
- ✅ DeleteProduct - Test xóa sản phẩm

### 2. Basket.API.Tests

**Location:** `Tests/Basket.API.Tests/`

**Test Coverage:**

- ✅ GetBasket - Test lấy giỏ hàng theo username
- ✅ CreateBasket - Test tạo giỏ hàng mới
- ✅ DeleteBasket - Test xóa giỏ hàng
- ✅ Checkout - Test checkout giỏ hàng

### 3. Ordering.API.Tests

**Location:** `Tests/Ordering.API.Tests/`

**Test Coverage:**

- ✅ GetOrdersByUsername - Test lấy đơn hàng theo username
- ✅ CheckoutOrder - Test tạo đơn hàng mới
- ✅ UpdateOrder - Test cập nhật đơn hàng
- ✅ DeleteOrder - Test xóa đơn hàng

### 4. Discount.API.Tests

**Location:** `Tests/Discount.API.Tests/`

**Test Coverage:**

- ✅ gRPC endpoints - Test gRPC service
- ✅ Health checks - Test service health

## 🚀 Chạy Tests

### Chạy tất cả tests:

```powershell
# Windows PowerShell
.\Tests\run-all-tests.ps1

# Hoặc
dotnet test Tests/
```

### Chạy tests cho một project cụ thể:

```bash
# Catalog API Tests
dotnet test Tests/Catalog.API.Tests/Catalog.API.Tests.csproj

# Basket API Tests
dotnet test Tests/Basket.API.Tests/Basket.API.Tests.csproj

# Ordering API Tests
dotnet test Tests/Ordering.API.Tests/Ordering.API.Tests.csproj

# Discount API Tests
dotnet test Tests/Discount.API.Tests/Discount.API.Tests.csproj
```

### Chạy với coverage report:

```bash
dotnet test Tests/ --collect:"XPlat Code Coverage"
```

### Chạy với verbose output:

```bash
dotnet test Tests/ --verbosity detailed
```

## 📝 Viết Test Mới

### Template cho Integration Test:

```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace YourService.API.Tests.IntegrationTests;

public class YourControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _baseUrl = "/api/v1/YourEndpoint";

    public YourControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Setup test data
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Cleanup test data
        await Task.CompletedTask;
    }

    [Fact]
    public async Task YourMethod_WithValidData_ReturnsOk()
    {
        // Arrange
        var request = new YourRequest { /* ... */ };

        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<YourResponse>();
        result.Should().NotBeNull();
    }
}
```

## 🔧 Test Configuration

### WebApplicationFactory Setup

Mỗi test project sử dụng `WebApplicationFactory<Program>` để tạo test server:

```csharp
var factory = new WebApplicationFactory<Program>();
var client = factory.CreateClient();
```

### Mocking Dependencies

Sử dụng Moq để mock external dependencies:

```csharp
var mockRepository = new Mock<IRepository>();
mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
    .ReturnsAsync(new Entity());
```

### Testcontainers (Optional)

Để test với real databases, sử dụng Testcontainers:

```csharp
var mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:latest")
    .Build();

await mongoContainer.StartAsync();
```

## 📊 Test Results

### XUnit Test Results

Tests sẽ tạo file `.trx` trong thư mục `TestResults/`

### Coverage Reports

Coverage reports được tạo trong format:

- OpenCover: `coverage.opencover.xml`
- Cobertura: `coverage.cobertura.xml`

## 🎯 Best Practices

1. **Test Naming:** Sử dụng format `MethodName_Scenario_ExpectedResult`
2. **AAA Pattern:** Arrange-Act-Assert trong mỗi test
3. **Isolation:** Mỗi test độc lập, không phụ thuộc vào test khác
4. **Cleanup:** Cleanup test data trong `DisposeAsync()`
5. **Assertions:** Sử dụng FluentAssertions cho readable assertions
6. **Test Data:** Sử dụng test data builders hoặc factories

## 🔍 Debugging Tests

### Visual Studio

1. Set breakpoint trong test
2. Right-click test → Debug Test

### VS Code

1. Install C# extension
2. Set breakpoint
3. Run test với debugger

### Command Line

```bash
dotnet test Tests/ --logger "console;verbosity=detailed"
```

## 📚 Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Testcontainers Documentation](https://dotnet.testcontainers.org/)

## 🐛 Troubleshooting

### Tests không chạy được

1. Kiểm tra .NET SDK version: `dotnet --version`
2. Restore packages: `dotnet restore`
3. Build solution: `dotnet build`

### Database connection errors

- Đảm bảo test databases đang chạy
- Kiểm tra connection strings trong test configuration

### Port conflicts

- Đổi ports trong test configuration nếu cần
- Sử dụng random ports cho test servers

## ✅ Checklist khi viết test mới

- [ ] Test name mô tả rõ ràng behavior
- [ ] Test covers cả success và failure cases
- [ ] Test data được setup và cleanup đúng cách
- [ ] Assertions sử dụng FluentAssertions
- [ ] Test không phụ thuộc vào test khác
- [ ] Test có thể chạy độc lập
- [ ] Error messages rõ ràng khi test fail
