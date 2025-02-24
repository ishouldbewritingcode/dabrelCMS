using Microsoft.AspNetCore.Mvc.Rendering;

namespace dabrelCMS.code
{
	public static class AdminBlock
	{
		public static string GetAddBlockForm(CMSPage page)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogaddblock.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{pageid}}", page.PageId.ToString());
			return html;
		}

		public static string GetBlockConfig(CMSBlock block)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogblock.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{blockid}}", block.BlockId.ToString());
			html = html.Replace("{{blocktype}}", block.BlockType);
			html = html.Replace("{{status}}", block.Status);
			html = html.Replace("{{title1}}", block.Title1);
			html = html.Replace("{{title2}}", block.Title2);
			html = html.Replace("{{data}}", block.Data);
			html = html.Replace("{{tags}}", block.Tags);
			return html;
		}

		public static string AddNewBlock(HttpContext context, CMSDbContext dbContext, CMSPage page)
		{

			return "";
		}
	}
}
