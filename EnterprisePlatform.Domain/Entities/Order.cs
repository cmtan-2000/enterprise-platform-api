namespace EnterprisePlatform.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Customer Customer { get; private set; } = null!;

    private Order()
    {
    }

    public Order(Guid customerId, decimal amount)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "CustomerId cannot be empty.",
                nameof(customerId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Order amount must be greater than zero.");
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
}