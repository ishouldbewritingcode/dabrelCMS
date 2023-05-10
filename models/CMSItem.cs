using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSItem
	{
		[Key]
		public int ItemId { get; set; }

		public int PageId { get; set; }
		public int Sort { get; set; }
		public string? ItemType { get; set; }
		public string? Status { get; set; }
		public string? Position { get; set; }
		public DateTime? Start { get; set; }
		public DateTime? End { get; set; }
		public string? Shortcut { get; set; }
		public string? Title1 { get; set; }
		public string? Title2 { get; set; }
		public string? Data { get; set; }
		public string? Tags { get; set; }
	}
}