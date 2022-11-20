using System;
namespace branch_protector.Models
{
	public class RepositoryPT
    {
		public string Owner { get; set; }
		public string Name { get; set; }
		public string Branch { get; set; }
        public string Sender { get; set; }
        public long InstallationId { get; set; }

		public RepositoryPT(string owner, string name, string branch,
			string sender, long installationId)
		{
			Owner = owner;
			Name = name;
			Branch = branch;
			Sender = sender;
			InstallationId = installationId;
		}
	}
}

