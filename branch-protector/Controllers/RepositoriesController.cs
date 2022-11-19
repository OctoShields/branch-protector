using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using branch_protector.Constants;
using Microsoft.AspNetCore.Mvc;
using Octokit.Webhooks;

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

        /* TODO: Add extra functionality to web service for listing 
         * repositories, pull request, and other useful information systems 
         * can leverage across organization
        */
        [HttpGet()]
        public IEnumerable<string> GetRepositories()
        {
            return BranchProtectorConstants.repositoriesList;
        }
    }
}

