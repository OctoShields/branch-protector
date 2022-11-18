using System;
using branch_protector.Controllers;
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

        public GitHubWebhookEventProcessor(ILogger<GitHubWebhookEventProcessor> logger)
        {
            _logger = logger;
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
                    break;
                default:
                    _logger.LogDebug($"--- Received Unsupported Action {repositoryEvent.Action}");
                    break;
            }

            return base.ProcessRepositoryWebhookAsync(headers, repositoryEvent, action);
        }
    }
}

