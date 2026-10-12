
using EnterprisePlatform.Domain.Entities;

namespace EnterprisePlatform.UnitTests.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldCreateCustomer_WhenInputIsValid()
    {
        // Arrange
        var dateOfBirth = new DateTime(1995, 5, 10);

        // Act
        var customer = CreateValidCustomer(dateOfBirth: dateOfBirth);

        // Assert
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("John", customer.FirstName);
        Assert.Equal("Doe", customer.LastName);
        Assert.Equal("john.doe@example.com", customer.Email);
        Assert.Equal("0123456789", customer.PhoneNumber);
        Assert.Equal(dateOfBirth, customer.DateOfBirth);
        Assert.Equal(0, customer.Version);
        Assert.Equal(DateTimeKind.Utc, customer.CreatedAt.Kind);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenFirstNameIsEmpty(
        string? firstName)
    {
        Assert.Throws<ArgumentException>(
            () => CreateValidCustomer(firstName: firstName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenLastNameIsEmpty(
        string? lastName)
    {
        Assert.Throws<ArgumentException>(
            () => CreateValidCustomer(lastName: lastName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("john@")]
    public void Constructor_ShouldThrow_WhenEmailIsInvalid(
        string? email)
    {
        Assert.Throws<ArgumentException>(
            () => CreateValidCustomer(email: email));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenPhoneNumberIsEmpty(
        string? phoneNumber)
    {
        Assert.Throws<ArgumentException>(
            () => CreateValidCustomer(phoneNumber: phoneNumber));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDateOfBirthIsInFuture()
    {
        var futureDate = DateTime.UtcNow.Date.AddDays(1);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => CreateValidCustomer(dateOfBirth: futureDate));
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateCustomer_WhenInputIsValid()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var newDateOfBirth = new DateTime(1990, 3, 15);

        // Act
        customer.UpdateDetails(
            "David",
            "Smith",
            "david.smith@example.com",
            "0198765432",
            newDateOfBirth);

        // Assert
        Assert.Equal("David", customer.FirstName);
        Assert.Equal("Smith", customer.LastName);
        Assert.Equal("david.smith@example.com", customer.Email);
        Assert.Equal("0198765432", customer.PhoneNumber);
        Assert.Equal(newDateOfBirth, customer.DateOfBirth);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenEmailIsInvalid()
    {
        var customer = CreateValidCustomer();

        Assert.Throws<ArgumentException>(() =>
            customer.UpdateDetails(
                "David",
                "Smith",
                "invalid-email",
                "0198765432",
                new DateTime(1990, 3, 15)));
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenDateOfBirthIsInFuture()
    {
        var customer = CreateValidCustomer();
        var futureDate = DateTime.UtcNow.Date.AddDays(1);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            customer.UpdateDetails(
                "David",
                "Smith",
                "david.smith@example.com",
                "0198765432",
                futureDate));
    }

    private static Customer CreateValidCustomer(
        string? firstName = "John",
        string? lastName = "Doe",
        string? email = "john.doe@example.com",
        string? phoneNumber = "0123456789",
        DateTime? dateOfBirth = null)
    {
        return new Customer(
            firstName!,
            lastName!,
            email!,
            phoneNumber!,
            dateOfBirth ?? new DateTime(1995, 5, 10));
    }
}
