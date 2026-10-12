
using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.UnitTests.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldGenerateUniqueId()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var order = new Order(customerId, 100m);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
    }

    [Fact]
    public void Constructor_ShouldSetCustomerId()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var order = new Order(customerId, 100m);

        // Assert
        Assert.Equal(customerId, order.CustomerId);
    }

    [Fact]
    public void Constructor_ShouldSetAmount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const decimal amount = 199.99m;

        // Act
        var order = new Order(customerId, amount);

        // Assert
        Assert.Equal(amount, order.Amount);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtInUtc()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var order = new Order(customerId, 100m);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.Equal(DateTimeKind.Utc, order.CreatedAt.Kind);
        Assert.InRange(
            order.CreatedAt,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Constructor_ShouldGenerateDifferentIdsForDifferentOrders()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var firstOrder = new Order(customerId, 100m);
        var secondOrder = new Order(customerId, 100m);

        // Assert
        Assert.NotEqual(firstOrder.Id, secondOrder.Id);
    }


    [Fact]
    public void Constructor_ShouldThrow_WhenCustomerIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new Order(Guid.Empty, 100m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_ShouldThrow_WhenAmountIsNotPositive(
        decimal amount)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Order(Guid.NewGuid(), amount));
    }

    [Fact]
    public void Constructor_ShouldCreateOrder_WhenInputIsValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const decimal amount = 100m;

        // Act
        var order = new Order(customerId, amount);

        // Assert
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(amount, order.Amount);
        Assert.NotEqual(Guid.Empty, order.Id);
    }

}
