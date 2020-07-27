using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CalculatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        // GET api/calculator
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Hallo from Calculator service";
        }

        // GET api/calculator/square/5
        [HttpGet("square/{n}")]
        public ActionResult<int> GetSquare(int n)
        {
            _logger.LogInformation($"Returning the square of {n}");
            return n*n;
        }
    }
}
