using System.Net;
using System.Net.Http.Json;
using Basket.API.Controller;
using Basket.Application.Commands;
using Basket.Application.Responses;
using Basket.Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Basket.API.Tests.IntegrationTests;

public class BasketControllerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _baseUrl = "/Basket";

    public BasketControllerTests(WebApplicationFactory<Program> factory)
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

    #region GetBasket Tests

    [Fact]
    public async Task GetBasket_WithValidUserName_ReturnsOk()
    {
        // Arrange
        var userName = "test_user";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetBasket/{userName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var basket = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
        basket.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBasket_WithNonExistentUserName_ReturnsNull()
    {
        // Arrange
        var userName = "non_existent_user";

        // Act
        var response = await _client.GetAsync($"{_baseUrl}/GetBasket/{userName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var basket = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
        basket.Should().BeNull();
    }

    #endregion

    #region CreateBasket Tests

    [Fact]
    public async Task CreateBasket_WithValidData_ReturnsOk()
    {
        // Arrange
        var command = new CreateShoppingCartCommand
        {
            UserName = "test_user_create",
            ShoppingCartItems = new List<ShoppingCartItem>
            {
                new ShoppingCartItem
                {
                    ProductId = "product1",
                    ProductName = "Test Product",
                    Price = 99.99m,
                    Quantity = 2,
                    ImageFile = "test.jpg"
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/CreateBasket", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var basket = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
        basket.Should().NotBeNull();
        basket.UserName.Should().Be(command.UserName);
        basket.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateBasket_WithEmptyItems_ReturnsOk()
    {
        // Arrange
        var command = new CreateShoppingCartCommand
        {
            UserName = "test_user_empty",
            ShoppingCartItems = new List<ShoppingCartItem>()
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/CreateBasket", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var basket = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
        basket.Should().NotBeNull();
        basket.Items.Should().BeEmpty();
    }

    #endregion

    #region DeleteBasket Tests

    [Fact]
    public async Task DeleteBasket_WithValidUserName_ReturnsOk()
    {
        // Arrange
        var userName = "test_user_delete";

        // Act
        var response = await _client.DeleteAsync($"{_baseUrl}/DeleteBasket/{userName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteBasket_WithNonExistentUserName_ReturnsOk()
    {
        // Arrange
        var userName = "non_existent_user_delete";

        // Act
        var response = await _client.DeleteAsync($"{_baseUrl}/DeleteBasket/{userName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Checkout Tests

    [Fact]
    public async Task Checkout_WithValidBasket_ReturnsAccepted()
    {
        // Arrange - First create a basket
        var createCommand = new CreateShoppingCartCommand
        {
            UserName = "test_user_checkout",
            ShoppingCartItems = new List<ShoppingCartItem>
            {
                new ShoppingCartItem
                {
                    ProductId = "product1",
                    ProductName = "Test Product",
                    Price = 99.99m,
                    Quantity = 1,
                    ImageFile = "test.jpg"
                }
            }
        };
        await _client.PostAsJsonAsync($"{_baseUrl}/CreateBasket", createCommand);

        var checkout = new BasketCheckout
        {
            UserName = "test_user_checkout",
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
            CVV = "123",
            PaymentMethod = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/Checkout", checkout);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Checkout_WithNonExistentBasket_ReturnsBadRequest()
    {
        // Arrange
        var checkout = new BasketCheckout
        {
            UserName = "non_existent_user_checkout",
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
            CVV = "123",
            PaymentMethod = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/Checkout", checkout);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Checkout_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var checkout = new BasketCheckout
        {
            UserName = "", // Invalid: empty username
            FirstName = "",
            LastName = "",
            EmailAddress = "invalid-email" // Invalid email format
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{_baseUrl}/Checkout", checkout);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}
