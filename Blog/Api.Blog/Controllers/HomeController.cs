using Api.Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Blog.Controllers;

[ApiController]
[Authorize]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get([FromServices] IConfiguration configuration)
    {
        var env = configuration.GetValue<string>("Env");
        return Ok(new ResultViewModel<dynamic>(new { environment = env }));
    }
}
