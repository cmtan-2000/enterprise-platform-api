using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnterprisePlatform.Application.Abstractions.ExternalServices;
using EnterprisePlatform.Application.Features.Customers.GetCustomerSummary;
using EnterprisePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EnterprisePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IExternalApiClient _externalApiClient;
        private readonly ICustomerApiClient _customerApiClient;


        public TestController(AppDbContext dbContext, IExternalApiClient externalApiClient, ICustomerApiClient customerApiClient)
        {
            _dbContext = dbContext;
            _externalApiClient = externalApiClient;
            _customerApiClient = customerApiClient;
        }

        [HttpGet("exception")]
        public IActionResult ThrowException()
        {
            throw new InvalidOperationException(
                "This is a test exception.");
        }

        [HttpGet("hold-connection")]
        public async Task<IActionResult> HoldConnection(CancellationToken cancellationToken)
        {
            var requestId = Guid.NewGuid();

            Console.WriteLine(
                $"[{requestId}] BEFORE OPEN {DateTime.UtcNow:HH:mm:ss.fff}");

            var connection = _dbContext.Database.GetDbConnection();

            await connection.OpenAsync(cancellationToken);

            Console.WriteLine(
                $"[{requestId}] AFTER OPEN  {DateTime.UtcNow:HH:mm:ss.fff}");

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    cancellationToken);

                Console.WriteLine(
                    $"[{requestId}] FINISH     {DateTime.UtcNow:HH:mm:ss.fff}");

                return Ok(new
                {
                    RequestId = requestId,
                    Message = "Connection released."
                });
            }
            finally
            {
                await connection.CloseAsync();

                Console.WriteLine(
                    $"[{requestId}] CONNECTION CLOSED {DateTime.UtcNow:HH:mm:ss.fff}");
            }
        }

        [HttpGet("include")]
        public async Task<IActionResult> Include(CancellationToken cancellationToken)
        {
            var customers = await _dbContext.Customers
                .AsNoTracking()
                .Include(x => x.Orders)
                .OrderBy(x => x.Id)
                .Take(10)
                .ToListAsync(cancellationToken);

            return Ok(customers);
        }

        [HttpGet("projection")]
        public async Task<IActionResult> Projection(CancellationToken cancellationToken)
        {
            var customers = await _dbContext.Customers
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Take(10)
                .Select(x => new CustomerSummaryDto(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Orders.Count,
                    x.Addresses.Count))
                .ToListAsync(cancellationToken);

            return Ok(customers);
        }

        [HttpGet("sequential")]
        public async Task<IActionResult> Sequential(Guid customerId, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var profile =
                await _externalApiClient.GetCustomerProfileAsync(
                    customerId,
                    cancellationToken);

            var payment =
                await _externalApiClient.GetPaymentSummaryAsync(
                    customerId,
                    cancellationToken);

            var notification =
                await _externalApiClient.GetNotificationStatusAsync(
                    customerId,
                    cancellationToken);

            stopwatch.Stop();

            return Ok(new
            {
                profile,
                payment,
                notification,
                elapsedMs = stopwatch.ElapsedMilliseconds
            });
        }

        [HttpGet("parallel")]
        public async Task<IActionResult> Parallel(Guid customerId, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var profileTask =
                _externalApiClient.GetCustomerProfileAsync(
                    customerId,
                    cancellationToken);

            var paymentTask =
                _externalApiClient.GetPaymentSummaryAsync(
                    customerId,
                    cancellationToken);

            var notificationTask =
                _externalApiClient.GetNotificationStatusAsync(
                    customerId,
                    cancellationToken);

            await Task.WhenAll(
                profileTask,
                paymentTask,
                notificationTask);

            stopwatch.Stop();

            return Ok(new
            {
                profile = await profileTask,
                payment = await paymentTask,
                notification = await notificationTask,
                elapsedMs = stopwatch.ElapsedMilliseconds
            });
        }

        [HttpGet("test-customer-api/{customerId:guid}")]
        public async Task<IActionResult> TestCustomerApi(Guid customerId, CancellationToken cancellationToken)
        {
            var result = await _customerApiClient.GetProfileAsync(
                customerId,
                cancellationToken);

            return Ok(result);
        }
    }
}