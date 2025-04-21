using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSPageBlock
	{
		[Key]
		public int PageBlockID { get; set; }
		public int Sort { get; set; } = 0;
		public int PageId { get; set; }
		public int BlockId { get; set; }
		public string? Position { get; set; }
		public string? AltTitle { get; set; }
		public string? AltSubtitle { get; set; }
	}
}
