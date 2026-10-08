namespace EnterprisePlatform.Api.DTOs.Orders;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    decimal Amount);