using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dabrelCMS.models
{
	public class CMSFile
	{
		[Key]
		public Guid FileId { get; set; }

		public Guid SiteId { get; set; }
		public string Filename { get; set; }
		public string? Status { get; set; }
		public string? Text { get; set; }
		public string? Tags { get; set; }

		[NotMapped]
		public string? Temp { get; set; }
	}
}