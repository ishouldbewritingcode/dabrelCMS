using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSBlock
	{
		[Key]
		public Guid BlockId { get; set; }
		public Guid SiteId {  get; set; }
		public string? BlockType { get; set; }
		public string? Status { get; set; }
		public string? Title1 { get; set; }
		public string? Title2 { get; set; }
		public string? Data { get; set; }
		public string? Tags { get; set; }

		public List<CMSItem>? cmsItems { get; set; }
	}
}
