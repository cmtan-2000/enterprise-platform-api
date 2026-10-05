using EnterprisePlatform.Application.Features.Customers.CreateCustomer;
using EnterprisePlatform.Application.Features.Customers.GetCustomerById;
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

            return Ok(result);
        }
    }
}