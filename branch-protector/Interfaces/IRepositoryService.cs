using System;
using branch_protector.Models;
using Octokit;

namespace branch_protector.Interfaces
{
	public interface IRepositoryService
	{
		public GitHubClient AuthenticateGitHubApp();
        public Task<GitHubClient> AuthenticateGitHubAppInstallation(long installationId);
        public Task<BranchProtectionSettings> CreateBranchProtections(RepositoryPT repository);
        public Task<Issue> CreateIssue(RepositoryPT repository, IssuePT issue);
    }
}

