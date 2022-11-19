using System;
using Octokit;

namespace branch_protector.Interfaces
{
	public interface IRepositoryService
	{
		public GitHubClient AuthenticateGitHubApp();
        public Task<GitHubClient> AuthenticateGitHubAppInstallation(long installationId);
        public Task<BranchProtectionSettings> CreateBranchProtections(string owner, string repo, string branch, long installationId);
        public Task<Issue> CreateIssue(string owner, string repo, string title, string body, long installationId);
    }
}

