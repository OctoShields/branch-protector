using System;
namespace branch_protector.Models
{
	public class IssuePT
    {
		public string Title { get; set; }
		public string Body { get; set; }

		public IssuePT(string title, string body)
		{
			Title = title;
			Body = body;
		}
	}
}

