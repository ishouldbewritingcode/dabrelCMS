using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSSiteUrl
	{
		[Key]
		public Guid SiteUrlId { get; set; }

		public Guid SiteId { get; set; }
		public string Url { get; set; }
		public bool Primary { get; set; }
	}
}