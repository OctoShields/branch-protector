using System;
using branch_protector.Interfaces;
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
                _logger.LogTrace(ex.StackTrace);
            }

            return appClient;
        }

        /// <summary>
        /// Create Branch Protections for a specific branch within the repository
        /// </summary>
        public async Task<BranchProtectionSettings> CreateBranchProtections(string owner, string repo, string branch, long installationId)
        {
            GitHubClient installationClient = await AuthenticateGitHubAppInstallation(installationId);

            var branchProtection = new BranchProtectionSettingsUpdate(
                null,
                new BranchProtectionRequiredReviewsUpdate(
                    new BranchProtectionRequiredReviewsDismissalRestrictionsUpdate(false), true, false, 1),
                null, false, false, false, false, false,true);

            BranchProtectionSettings updateBranchProtections = new BranchProtectionSettings();
            Octokit.Issue newIssue = new Octokit.Issue();

            try
            {
                updateBranchProtections = await installationClient.Repository.Branch.UpdateBranchProtection(owner, repo, branch, branchProtection);
                newIssue = await CreateIssue(owner, repo, "Main Branch Protections Set", "Hey @Pujolsluis main branch is now protected", installationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogTrace(ex.StackTrace);
            }

            return updateBranchProtections;

        }

        /// <summary>
        /// Create Issue with details of Branch Protections applied to Repository
        /// </summary>
        public async Task<Octokit.Issue> CreateIssue(string owner, string repo, string title, string body, long installationId)
        {
            GitHubClient installationClient = await AuthenticateGitHubAppInstallation(installationId);

            var createIssue = new NewIssue(title);
            createIssue.Body = "Hey @Pujolsluis the main branch is now protected.\n\n**Main Branch - Protections Enabled:**\n- Require a pull request before merging\n  - Require at least 1 approval before merging\n  - Dissmiss stale pull request approvals when new commits are pushed\n- Require conversation resolution before merging";

            var newIssue = await installationClient.Issue.Create(owner, repo, createIssue);

            return newIssue;
        }

        /// <summary>
        /// Create JWT Token from GitHub App Private Key Source File
        /// </summary>
        string GenerateJWTToken()
        {
            string appId = Environment.GetEnvironmentVariable("GITHUB_APP_ID");
            int appIdValue;

            Int32.TryParse(appId, out appIdValue);

            if (Int32.TryParse(appId, out appIdValue))
            {
                Console.WriteLine(appId);
            }
            else
            {
                Console.WriteLine($"Int32.TryParse could not parse '{appId}' to an int.");
            }

            var generator = new GitHubJwt.GitHubJwtFactory(
            new GitHubJwt.FilePrivateKeySource(Environment.GetEnvironmentVariable("GITHUB_PRIVATE_KEY_SOURCE")),
            new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = appIdValue, // GitHub App ID
                ExpirationSeconds = 600, //10 mins max
            });

            return generator.CreateEncodedJwtToken();
        }

    }
}

