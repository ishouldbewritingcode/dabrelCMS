using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSSite
	{
		[Key]
		public Guid SiteId { get; set; }

		public string? Name { get; set; }
		public string? Design { get; set; }
		public string? Title { get; set; }
		public string? SubTitle { get; set; }
		public string? Footer1 { get; set; }
		public string? Footer2 { get; set; }
		public string? Footer3 { get; set; }
		public string? Footer4 { get; set; }
		public string? MetaDescription { get; set; }
		public string? MetaImagePath { get; set; }
		public string? OnAllPages { get; set; }
		public string? BodyTop { get; set; }
		public string? BodyBottom { get; set; }
		public string? ImageFileName { get; set; }
		public DateTime Created { get; set; }
		public string? FaviconUrl { get; set; }

		public List<CMSSiteUrl>? cmsSiteUrls { get; set; }
		public List<CMSPage>? cmsPages { get; set; }
	}
}