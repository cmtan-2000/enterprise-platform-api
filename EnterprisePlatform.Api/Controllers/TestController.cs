using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EnterprisePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("exception")]
        public IActionResult ThrowException()
        {
            throw new InvalidOperationException(
                "This is a test exception.");
        }
    }
}