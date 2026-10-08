using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Exceptions;
using EnterprisePlatform.Domain.Entities;
using MediatR;

namespace EnterprisePlatform.Application.Features.Orders.CreateOrder
{
    public sealed class CreateOrderHandler
    : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IIdempotencyRepository idempotencyRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Check idempotency
            var existing = await _idempotencyRepository.GetByKeyAsync(
                request.IdempotencyKey,
                cancellationToken);

            if (existing is not null)
            {
                return existing.OrderId;
            }

            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(
                    async ct =>
                    {
                        var order = new Order(
                            request.CustomerId,
                            request.Amount);

                        await _orderRepository.AddAsync(
                            order,
                            ct);

                        var idempotencyRecord =
                            new IdempotencyRecord(
                                request.IdempotencyKey,
                                order.Id);

                        await _idempotencyRepository.AddAsync(
                            idempotencyRecord,
                            ct);

                        return order.Id;
                    },
                    cancellationToken);
            }
            catch (IdempotencyConflictException)
            {
                existing = await _idempotencyRepository.GetByKeyAsync(request.IdempotencyKey, cancellationToken);

                if (existing is not null)
                {
                    return existing.OrderId;
                }

                throw;
            }
        }
    }
}