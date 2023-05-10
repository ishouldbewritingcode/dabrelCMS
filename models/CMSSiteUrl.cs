using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSSiteUrl
	{
		[Key]
		public int SiteUrlId { get; set; }

		public int SiteId { get; set; }
		public string Url { get; set; }
		public bool Primary { get; set; }
	}
}