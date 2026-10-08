using System.Collections.Concurrent;
using EnterprisePlatform.Api.DTOs.Orders;
using EnterprisePlatform.Application.Abstractions.ExternalServices;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Application.Features.Customers.CreateCustomer;
using EnterprisePlatform.Application.Features.Customers.GetCustomerById;
using EnterprisePlatform.Application.Features.Customers.GetCustomers;
using EnterprisePlatform.Application.Features.Customers.UpdateCustomer;
using EnterprisePlatform.Application.Features.Orders.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnterprisePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICustomerQueries _customerQueries;
        private static readonly ConcurrentDictionary<Guid, int> Attempts = new();

        public CustomersController(ISender sender, ICustomerQueries customerQueries)
        {
            _sender = sender;
            _customerQueries = customerQueries;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return Created($"/api/customers/{result.Id}", result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetCustomerByIdResult>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetCustomerByIdQuery(id),
                cancellationToken);

            Response.Headers.ETag = $"\"{result.Version}\"";

            return Ok(new
            {
                result.Id,
                result.FirstName,
                result.LastName,
                result.Email,
                result.PhoneNumber,
                result.DateOfBirth
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request, [FromHeader(Name = "If-Match")] string? ifMatch, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ifMatch))
            {
                return StatusCode(
                    StatusCodes.Status428PreconditionRequired);
            }

            var rawValue = ifMatch.ToString().Trim('"');

            if (!long.TryParse(rawValue, out var expectedVersion))
            {
                return BadRequest("Invalid If-Match header.");
            }

            var newVersion = await _sender.Send(
                new UpdateCustomerCommand(
                    id,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber,
                    request.DateOfBirth,
                    expectedVersion),
                cancellationToken);

            Response.Headers.ETag = $"\"{newVersion}\"";

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<GetCustomersResult>> GetCustomers([FromQuery] GetCustomersQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                query,
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("test-n-plus-one")]
        public async Task<IActionResult> TestNPlusOne(CancellationToken cancellationToken)
        {
            await _customerQueries.TestNPlusOneAsync(
                cancellationToken);

            return Ok();
        }

        [HttpGet("external-simulation/{customerId:guid}")]
        public async Task<IActionResult> ExternalSimulation(Guid customerId, CancellationToken cancellationToken)
        {
            var attempt = Attempts.AddOrUpdate(customerId, 1, (_, current) => current + 1);

            Console.WriteLine($"External API attempt: {attempt}");

            if (attempt <= 2)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            Attempts.TryRemove(customerId, out _);

            return Ok(new CustomerProfileDto(
                customerId,
                "John",
                "Doe",
                "john@example.com"));
        }

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromHeader(Name = "Idempotency-Key")] string? idempotencyKey, CreateOrderRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return BadRequest(new
                {
                    code = "idempotency_key.required",
                    message = "Idempotency-Key is required."
                });
            }

            var orderId = await _sender.Send(
                new CreateOrderCommand(
                    request.CustomerId,
                    request.Amount,
                    idempotencyKey),
                cancellationToken);

            return Ok(new
            {
                orderId
            });
        }
    }
}