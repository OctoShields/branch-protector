using System;
using branch_protector.Constants;
using branch_protector.Interfaces;
using branch_protector.Models;
using Octokit;
using Octokit.Webhooks.Models;

namespace branch_protector.Services
{
	public class GitHubRepositoryService : IRepositoryService
	{
        ILogger<GitHubRepositoryService> _logger;

		public GitHubRepositoryService(ILogger<GitHubRepositoryService> logger)
		{
            _logger = logger;
        }

        /// <summary>
        /// Authenticate as Github App
        /// </summary>
        public GitHubClient AuthenticateGitHubApp()
        {
            // Generate JWT Token
            var jwtToken = GenerateJWTToken();

            // Return authenticated GitHub Client
            string? appName = Environment.GetEnvironmentVariable("GITHUB_APP_NAME");
            var appClient = new GitHubClient(new ProductHeaderValue(appName))
            {
                Credentials = new Credentials(jwtToken, AuthenticationType.Bearer)
            };

            return appClient;
        }

        /// <summary>
        /// Authenticate as Github App Installation
        /// </summary>
        public async Task<GitHubClient> AuthenticateGitHubAppInstallation(long installationId)
        {
            var appClient = AuthenticateGitHubApp();

            try
            {
                var response = await appClient.GitHubApps.CreateInstallationToken(installationId);

                string? appName = Environment.GetEnvironmentVariable("GITHUB_APP_NAME");
                var installationClient = new GitHubClient(new ProductHeaderValue($"{appName}-{installationId}"))
                {
                    Credentials = new Credentials(response.Token, AuthenticationType.Bearer)
                };

                return installationClient;
            } catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogTrace(ex.StackTrace);
            }

            return appClient;
        }

        /// <summary>
        /// Create Branch Protections for a specific branch within the repository
        /// </summary>
        public async Task<BranchProtectionSettings> CreateBranchProtections(RepositoryPT repository)
        {
            // Authenticate GitHub App Installation
            GitHubClient installationClient = await AuthenticateGitHubAppInstallation(repository.InstallationId);

            // Create Branch Protections Settings
            var branchProtectionSettings = new BranchProtectionSettingsUpdate(
                null,
                new BranchProtectionRequiredReviewsUpdate(
                    new BranchProtectionRequiredReviewsDismissalRestrictionsUpdate(false), true, false, 1),
                null, false, false, false, false, false,true);

            BranchProtectionSettings updateBranchProtectionsResponse = new BranchProtectionSettings();
            Octokit.Issue newIssue = new Octokit.Issue();

            try
            {
                // Request to Update Branch Protections Settings in Repository
                updateBranchProtectionsResponse =
                    await installationClient
                    .Repository
                    .Branch
                    .UpdateBranchProtection(repository.Owner,repository.Name,
                                            repository.Branch, branchProtectionSettings);

                // Create Issue to notify user of successful Branch update
                newIssue = await CreateIssue(repository, new IssuePT(BranchProtectorConstants.issueBranchProtectionsTitle,
                                                                     BranchProtectorConstants.issueBranchProtectionsBody));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogTrace(ex.StackTrace);
            }

            return updateBranchProtectionsResponse;
        }

        /// <summary>
        /// Create Issue with details of Branch Protections applied to Repository
        /// </summary>
        public async Task<Octokit.Issue> CreateIssue(RepositoryPT repository, IssuePT issue)
        {
            // Authenticate GitHub App Installation
            GitHubClient installationClient = await AuthenticateGitHubAppInstallation(repository.InstallationId);

            // Prepare new Issue data
            var newIssue = new NewIssue(issue.Title);
            newIssue.Body = issue.Body;

            Octokit.Issue issueResponse = new Octokit.Issue();

            try
            {
                // Request to Create new GitHub issue
                issueResponse = await installationClient.Issue.Create(repository.Owner, repository.Name, newIssue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogTrace(ex.StackTrace);
            }

            return issueResponse;
        }

        /// <summary>
        /// Create JWT Token from GitHub App Private Key Source File
        /// </summary>
        string GenerateJWTToken()
        {
            // Initialize AppID from .env
            string appId = Environment.GetEnvironmentVariable("GITHUB_APP_ID");
            int appIdValue;

            Int32.TryParse(appId, out appIdValue);
            if (Int32.TryParse(appId, out appIdValue))
            {
                _logger.LogDebug($"Parsed {appId} successfully");
            }
            else
            {
                _logger.LogError($"Int32.TryParse could not parse '{appId}' to an int.");
                throw new ArgumentException("App ID could not be parsed, please verify .env file");
            }

            // Initialize GitHub Private Key Source Path from .env
            string privateKeySourcePath = Environment.GetEnvironmentVariable("GITHUB_PRIVATE_KEY_SOURCE");

            if (string.IsNullOrEmpty(privateKeySourcePath))
                throw new ArgumentException("App ID could not be parsed, please verify .env file");

            // Initialize JWT Generator
            var generator = new GitHubJwt.GitHubJwtFactory(
                new GitHubJwt.FilePrivateKeySource(privateKeySourcePath),
                new GitHubJwt.GitHubJwtFactoryOptions
                {
                    AppIntegrationId = appIdValue, // GitHub App ID
                    ExpirationSeconds = 600, //10 mins max
                });

            return generator.CreateEncodedJwtToken();
        }

    }
}

