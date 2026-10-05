using EnterprisePlatform.Api.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EnterprisePlatform.Api.Controllers;

[ApiController]
[Route("api/info")]
public class InfoController : ControllerBase
{
    private readonly ApplicationSettings _settings;

    public InfoController(IOptions<ApplicationSettings> options)
    {
        _settings = options.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            application = _settings.ApplicationName,
            environment = _settings.Environment
        });
    }
}