using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NC.Localization.Json.Sample.Dtos;

namespace NC.Localization.Json.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IStringLocalizer<TestController> _stringLocalizer;
        public TestController(IStringLocalizer<TestController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet("{name}")]
        public IActionResult Index(string name)
        {
            var message = string.Format(_stringLocalizer["welcome"], name);
            return Ok(message);
        }

        [HttpPost]
        public IActionResult Post(PostTestDto dto)
        {
            var welcomeMessage = string.Format(_stringLocalizer["welcome"], dto.Name);
            return Ok(welcomeMessage);
        }
    }
}
