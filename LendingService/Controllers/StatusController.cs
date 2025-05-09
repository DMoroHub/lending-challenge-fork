using Microsoft.AspNetCore.Mvc;

namespace LendingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok();
    }
}
