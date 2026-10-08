using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public TestController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("exception")]
        public IActionResult ThrowException()
        {
            throw new InvalidOperationException(
                "This is a test exception.");
        }

        [HttpGet("hold-connection")]
        public async Task<IActionResult> HoldConnection(
    CancellationToken cancellationToken)
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
    }
}