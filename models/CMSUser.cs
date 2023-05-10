using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSUser
	{
		[Key]
		public int UserId { get; set; }

		public int SiteId { get; set; }
		public string Provider { get; set; }
		public string NameIdentifier { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string EmailConfirmed { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Mobile { get; set; }
		public string Roles { get; set; }

		public List<string> RoleList
		{
			get
			{
				return Roles.Split(",").ToList();
			}
		}
	}
}