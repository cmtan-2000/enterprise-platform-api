using EnterprisePlatform.Application.Features.Customers.CreateCustomer;
using EnterprisePlatform.Application.Features.Customers.GetCustomerById;
using EnterprisePlatform.Application.Features.Customers.UpdateCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnterprisePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ISender _sender;

        public CustomersController(ISender sender)
        {
            _sender = sender;
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
    }
}