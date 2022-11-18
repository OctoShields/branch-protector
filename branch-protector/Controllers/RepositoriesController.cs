using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace branch_protector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriesController : ControllerBase
    {
        private readonly ILogger<RepositoriesController> _logger;

        public RepositoriesController(ILogger<RepositoriesController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IEnumerable<string> GetRepositories()
        {
            return new string[] { "octo-guard", "octo-defender" };
        }

        /// <summary>
        /// Receives GitHub Events related to the Organization Repositories 
        /// </summary>
        /// <response code="200">Processed GitHub Event Successfully</response>
        /// <response code="404">Could not process GitHub Event</response>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            _logger.LogDebug($"--- Received GitHub Event ---- {json}");
            return Ok();
        }
    }
}

