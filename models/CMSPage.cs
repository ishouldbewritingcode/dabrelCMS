using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSPage
	{
		[Key]
		public int PageId { get; set; }

		public int ParentId { get; set; }
		public int Sort { get; set; }
		public int SiteId { get; set; }
		public bool isOn { get; set; }
		public bool isPrivate { get; set; }
		public bool isHidden { get; set; }
		public string? Shortcut { get; set; }
		public string? Tags { get; set; }
		public string? NavTitle { get; set; }
		public string? Title { get; set; }
		public string? Summary { get; set; }
		public string? HeroImage { get; set; }

		public List<CMSPageBlock>? pageBlocks { get; set; }
	}
}