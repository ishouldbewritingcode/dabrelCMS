using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSFile
	{
		[Key]
		public int FileId { get; set; }

		public int SiteId { get; set; }
		public string Filename { get; set; }
		public string? Status { get; set; }
		public string? Text { get; set; }
		public string? Tags { get; set; }
	}
}