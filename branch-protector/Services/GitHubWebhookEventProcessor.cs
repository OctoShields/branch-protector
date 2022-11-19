using System;
using branch_protector.Controllers;
using branch_protector.Interfaces;
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

            switch(repositoryEvent.Action)
            {
                case "created":
                    _logger.LogDebug($"--- Received Supported Action ---- {action}");
                    string repo = repositoryEvent.Repository != null ? repositoryEvent.Repository.Name : string.Empty;
                    string owner = repositoryEvent.Repository != null ? repositoryEvent.Repository.Owner.Login : string.Empty;
                    long installationId = repositoryEvent.Installation != null ? repositoryEvent.Installation.Id : 0;

                    _repositoryService.CreateBranchProtections(owner, repo, "main", installationId);
                    break;
                default:
                    _logger.LogDebug($"--- Received Unsupported Action {repositoryEvent.Action}");
                    break;
            }

            return base.ProcessRepositoryWebhookAsync(headers, repositoryEvent, action);
        }
    }
}

