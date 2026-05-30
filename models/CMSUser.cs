using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSUser
	{
		[Key]
		public Guid UserId { get; set; }

		public Guid SiteId { get; set; }
		public string? Provider { get; set; }
		public string? NameIdentifier { get; set; }
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public int Salt { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Mobile { get; set; }
		public string Roles { get; set; }
		public string? TotpSecret { get; set; }

		public List<string> RoleList
		{
			get
			{
				return Roles.Split(",").ToList();
			}
		}
	}
}