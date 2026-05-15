using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSPageBlock
	{
		[Key]
		public Guid PageBlockID { get; set; }
		public int Sort { get; set; } = 0;
		public Guid PageId { get; set; }
		public Guid BlockId { get; set; }
		public string? Position { get; set; }
		public string? AltTitle { get; set; }
		public string? AltSubtitle { get; set; }
	}
}
