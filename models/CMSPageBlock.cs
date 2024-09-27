using System.ComponentModel.DataAnnotations;

namespace dabrelCMS.models
{
	public class CMSPageBlock
	{
		[Key]
		public int PageBlockID { get; set; }
		public int PageId { get; set; }
		public int BlockId { get; set; }
	}
}
