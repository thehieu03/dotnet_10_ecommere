using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Xunit;

namespace Ordering.API.Tests.IntegrationTests;

public class OrderControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _baseUrl = "/api/v1/Order";

    public OrderControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    #region GetOrdersByUsername Tests

    [Fact]
    public async Task GetOrdersByUsername_WithValidUserName_ReturnsOk()
    {
        // Arrange
        var userName = "test_user";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/{userName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
        orders.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrdersByUsername_WithNonExistentUserName_ReturnsEmptyList()
    {
        // Arrange
        var userName = "non_existent_user";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/{userName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
        orders.Should().NotBeNull();
        orders.Should().BeEmpty();
    }

    #endregion

    #region CheckoutOrder Tests

    [Fact]
    public async Task CheckoutOrder_WithValidData_ReturnsOk()
    {
        // Arrange
        var command = new CheckoutOrderCommand
        {
            UserName = "test_user_checkout",
            TotalPrice = 99.99m,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john@example.com",
            AddressLine = "123 Main St",
            Country = "USA",
            State = "CA",
            ZipCode = "12345",
            CardName = "John Doe",
            CardNumber = "1234567890",
            Expiration = "12/25",
            Cvv = "123",
            PaymentMethod = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl, command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderId = await response.Content.ReadFromJsonAsync<int>();
        orderId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CheckoutOrder_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var command = new CheckoutOrderCommand
        {
            UserName = "", // Invalid: empty username
            TotalPrice = -1, // Invalid: negative price
            EmailAddress = "invalid-email" // Invalid email format
        };

        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl, command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckoutOrder_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var command = new CheckoutOrderCommand
        {
            UserName = "test_user",
            // Missing required fields: FirstName, LastName, EmailAddress
        };

        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl, command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region UpdateOrder Tests

    [Fact]
    public async Task UpdateOrder_WithValidData_ReturnsNoContent()
    {
        // Arrange - First create an order
        var createCommand = new CheckoutOrderCommand
        {
            UserName = "test_user_update",
            TotalPrice = 99.99m,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john@example.com",
            AddressLine = "123 Main St",
            Country = "USA",
            State = "CA",
            ZipCode = "12345",
            CardName = "John Doe",
            CardNumber = "1234567890",
            Expiration = "12/25",
            Cvv = "123",
            PaymentMethod = 1
        };
        var createResponse = await _client.PostAsJsonAsync(_baseUrl, createCommand);
        var orderId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateCommand = new UpdateOrderCommand
        {
            Id = orderId,
            UserName = "test_user_update",
            TotalPrice = 149.99m,
            FirstName = "Jane",
            LastName = "Doe",
            EmailAddress = "jane@example.com",
            AddressLine = "456 Oak Ave",
            Country = "USA",
            State = "NY",
            ZipCode = "54321",
            CardName = "Jane Doe",
            CardNumber = "0987654321",
            Expiration = "06/26",
            Cvv = "456",
            PaymentMethod = 2
        };

        // Act
        var response = await _client.PutAsJsonAsync(_baseUrl, updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateOrder_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var updateCommand = new UpdateOrderCommand
        {
            Id = 99999, // Non-existent ID
            UserName = "test_user",
            TotalPrice = 99.99m,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john@example.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync(_baseUrl, updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DeleteOrder Tests

    [Fact]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent()
    {
        // Arrange - First create an order
        var createCommand = new CheckoutOrderCommand
        {
            UserName = "test_user_delete",
            TotalPrice = 99.99m,
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john@example.com",
            AddressLine = "123 Main St",
            Country = "USA",
            State = "CA",
            ZipCode = "12345",
            CardName = "John Doe",
            CardNumber = "1234567890",
            Expiration = "12/25",
            Cvv = "123",
            PaymentMethod = 1
        };
        var createResponse = await _client.PostAsJsonAsync(_baseUrl, createCommand);
        var orderId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var orderId = 99999; // Non-existent ID

        // Act
        var response = await _client.DeleteAsync($"{_baseUrl}/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}
