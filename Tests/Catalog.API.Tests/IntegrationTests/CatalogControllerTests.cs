using System.Net;
using System.Net.Http.Json;
using Catalog.API.Controllers;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Catalog.API.Tests.IntegrationTests;

public class CatalogControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _baseUrl = "/api/v1/Catalog";

    public CatalogControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Setup test data if needed
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Cleanup test data if needed
        await Task.CompletedTask;
    }

    #region GetProductById Tests

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsOk()
    {
        // Arrange
        var productId = "507f1f77bcf86cd799439011"; // Valid MongoDB ObjectId format

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetProductById/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = "invalid-id";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetProductById/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProductById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439999";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetProductById/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GetProductByProductName Tests

    [Fact]
    public async Task GetProductByProductName_WithValidName_ReturnsOk()
    {
        // Arrange
        var productName = "Laptop";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetProductByProductName/{productName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductByProductName_WithNonExistentName_ReturnsEmptyList()
    {
        // Arrange
        var productName = "NonExistentProduct12345";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetProductByProductName/{productName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products.Should().BeEmpty();
    }

    #endregion

    #region GetAllProducts Tests

    [Fact]
    public async Task GetAllProducts_WithoutParameters_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProducts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Pagination<ProductResponse>>();
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllProducts_WithPagination_ReturnsOk()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProducts?PageIndex={pageIndex}&PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Pagination<ProductResponse>>();
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(pageIndex);
        result.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public async Task GetAllProducts_WithBrandFilter_ReturnsFilteredResults()
    {
        // Arrange
        var brandName = "Dell";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProducts?Brand={brandName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Pagination<ProductResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllProducts_WithTypeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var typeName = "Electronics";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProducts?Type={typeName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Pagination<ProductResponse>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllProducts_WithSearch_ReturnsFilteredResults()
    {
        // Arrange
        var search = "laptop";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProducts?Search={search}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Pagination<ProductResponse>>();
        result.Should().NotBeNull();
    }

    #endregion

    #region GetAllBrands Tests

    [Fact]
    public async Task GetAllBrands_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllBrands");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var brands = await response.Content.ReadFromJsonAsync<List<BrandResponse>>();
        brands.Should().NotBeNull();
    }

    #endregion

    #region GetAllTypes Tests

    [Fact]
    public async Task GetAllTypes_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllTypes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var types = await response.Content.ReadFromJsonAsync<List<TypesResponse>>();
        types.Should().NotBeNull();
    }

    #endregion

    #region GetAllProductByBrandName Tests

    [Fact]
    public async Task GetAllProductByBrandName_WithValidBrand_ReturnsOk()
    {
        // Arrange
        var brandName = "Dell";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProductByBrandName/{brandName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllProductByBrandName_WithNonExistentBrand_ReturnsNotFound()
    {
        // Arrange
        var brandName = "NonExistentBrand12345";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetAllProductByBrandName/{brandName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region CreateProduct Tests

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsOk()
    {
        // Arrange
        var brand = new ProductBrand { Name = "Test Brand" };
        var type = new ProductType { Name = "Test Type" };
        var command = new CreateProductCommand(
            name: "Test Product",
            summary: "Test Summary",
            description: "Test Description",
            imageFile: "test.jpg",
            brands: brand,
            types: type,
            price: 99.99m
        );

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/CreateProduct", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        product.Should().NotBeNull();
        product.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var brand = new ProductBrand { Name = "Test Brand" };
        var type = new ProductType { Name = "Test Type" };
        var command = new CreateProductCommand(
            name: "", // Invalid: empty name
            summary: "",
            description: "",
            imageFile: "",
            brands: brand,
            types: type,
            price: -1 // Invalid: negative price
        );

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/CreateProduct", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region UpdateProduct Tests

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsOk()
    {
        // Arrange
        var brand = new ProductBrand { Name = "Updated Brand" };
        var type = new ProductType { Name = "Updated Type" };
        var command = new UpdateProductCommand(
            id: "507f1f77bcf86cd799439011",
            name: "Updated Product Name",
            summary: "Updated Summary",
            description: "Updated Description",
            imageFile: "updated.jpg",
            brands: brand,
            types: type,
            price: 149.99m
        );

        // Act
        var response = await _client.PutAsJsonAsync($"{_baseUrl}/UpdateProduct", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProduct_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var brand = new ProductBrand { Name = "Test Brand" };
        var type = new ProductType { Name = "Test Type" };
        var command = new UpdateProductCommand(
            id: "507f1f77bcf86cd799439999",
            name: "Updated Product Name",
            summary: "Updated Summary",
            description: "Updated Description",
            imageFile: "updated.jpg",
            brands: brand,
            types: type,
            price: 149.99m
        );

        // Act
        var response = await _client.PutAsJsonAsync($"{_baseUrl}/UpdateProduct", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeFalse();
    }

    #endregion

    #region DeleteProduct Tests

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsOk()
    {
        // Arrange
        var productId = "507f1f77bcf86cd799439011";

        // Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteProduct_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var productId = "507f1f77bcf86cd799439999";

        // Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        result.Should().BeFalse();
    }

    #endregion
}
