using System;
namespace branch_protector.Constants
{
	public static class BranchProtectorConstants
	{
		public static string issueBranchProtectionsTitle = "Branch Protections Set";
        public static string issueBranchProtectionsBody = "Hey @Pujolsluis the main branch is now protected. :shield:\n\n **Main - Branch Protections Enabled** \n- [x] Require a pull request before merging\n  - [x] Require at least 1 approval before merging\n  - [x] Dismiss stale pull request approvals when new commits are pushed\n  - [x] Require conversation resolution before merging";
		public static string[] repositoriesList= new string[] { "octo-guard", "octo-defender" };
	}
}

