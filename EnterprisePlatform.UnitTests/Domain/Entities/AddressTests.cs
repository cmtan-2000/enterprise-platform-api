using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.UnitTests.Domain.Entities;

public class AddressTests
{
    [Fact]
    public void Constructor_ShouldCreateAddress_WhenInputIsValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var city = "Kuala Lumpur";

        // Act
        var address = new Address(customerId, city);

        // Assert
        Assert.NotEqual(Guid.Empty, address.Id);
        Assert.Equal(customerId, address.CustomerId);
        Assert.Equal(city, address.City);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds_WhenCreatingMultipleAddresses()
    {
        // Arrange & Act
        var customerId = Guid.NewGuid();

        var address1 = new Address(customerId, "Kuala Lumpur");
        var address2 = new Address(customerId, "Johor Bahru");

        // Assert
        Assert.NotEqual(address1.Id, address2.Id);
    }


    [Fact]
    public void Constructor_ShouldAllowEmptyCity()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var address = new Address(customerId, string.Empty);

        // Assert
        Assert.Equal(string.Empty, address.City);
    }

    [Fact]
    public void Constructor_ShouldAllowEmptyCustomerId()
    {
        // Arrange
        var city = "Kuala Lumpur";

        // Act
        var address = new Address(Guid.Empty, city);

        // Assert
        Assert.Equal(Guid.Empty, address.CustomerId);
        Assert.Equal(city, address.City);
    }

}