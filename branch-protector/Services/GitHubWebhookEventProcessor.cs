using System;
using branch_protector.Controllers;
using branch_protector.Interfaces;
using branch_protector.Models;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.InstallationRepositories;
using Octokit.Webhooks.Events.Repository;

namespace branch_protector.Services
{
    /// <summary>
    /// Handles GitHub Organization Events
    /// </summary>
    public class GitHubWebhookEventProcessor : WebhookEventProcessor
    {
        private readonly ILogger<GitHubWebhookEventProcessor> _logger;
        private readonly IRepositoryService _repositoryService;

        public GitHubWebhookEventProcessor(ILogger<GitHubWebhookEventProcessor> logger, IRepositoryService repositoryService)
        {
            _logger = logger;
            _repositoryService = repositoryService;
        }

        /// <summary>
        /// Process GitHub Repository Events
        /// </summary>
        protected override Task ProcessRepositoryWebhookAsync(WebhookHeaders headers, RepositoryEvent repositoryEvent, RepositoryAction action)
        {

            _logger.LogDebug($"--- Received GitHub Repository Event ---- {repositoryEvent}");
            _logger.LogDebug($"--- GitHub Repository Event Action ---- {action}");

            // Handle GitHub repositoy event actions
            switch(repositoryEvent.Action)
            {
                case "created":
                    _logger.LogDebug($"--- Received Supported Action ---- {action}");

                    string repo = repositoryEvent.Repository != null ? repositoryEvent.Repository.Name : string.Empty;
                    string owner = repositoryEvent.Repository != null ? repositoryEvent.Repository.Owner.Login : string.Empty;
                    long installationId = repositoryEvent.Installation != null ? repositoryEvent.Installation.Id : 0;

                    // Selected branch to set Branch Protections
                    string branch = "main";

                    // Create branch protections for new repository
                    _repositoryService.CreateBranchProtections(new RepositoryPT(owner, repo, branch, installationId));
                    break;
                default:
                    _logger.LogDebug($"--- Received Unsupported Action {repositoryEvent.Action}");
                    break;
            }

            return base.ProcessRepositoryWebhookAsync(headers, repositoryEvent, action);
        }
    }
}

