using System.ComponentModel.DataAnnotations.Schema;

namespace dabrelCMS.models
{
	public static class CMSConfig
	{
		[NotMapped]
		public static string ConStr { get; set; }

		[NotMapped]
		public static string JwtKey { get; set; }
	}
}