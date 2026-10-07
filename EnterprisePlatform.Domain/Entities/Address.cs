namespace EnterprisePlatform.Domain.Entities;

public sealed class Address
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public string City { get; private set; } = null!;

    public Customer Customer { get; private set; } = null!;
    public ICollection<Address> Addresses { get; private set; } = new List<Address>();

    private Address()
    {
    }

    public Address(
        Guid customerId,
        string city)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        City = city;
    }
}