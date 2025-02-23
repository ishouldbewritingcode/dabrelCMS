using Microsoft.AspNetCore.Mvc.Rendering;

namespace dabrelCMS.code
{
	public static class AdminPage
	{
		public static string GetPageConfig(CMSPage page)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\admin\\dialogpage.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{pageid}}", page.PageId.ToString());
			html = html.Replace("{{parentid}}", page.ParentId.ToString());
			html = html.Replace("{{pageshortcut}}", page.Shortcut);
			html = html.Replace("{{pagesort}}", page.Sort.ToString());
			html = html.Replace("{{pageison}}", (page.isOn ? "checked='true'" : ""));
			html = html.Replace("{{pageisprivate}}", (page.isPrivate ? "checked='true'" : ""));
			html = html.Replace("{{pageishidden}}", (page.isHidden ? "checked='true'" : ""));
			html = html.Replace("{{pagetags}}", page.Tags);
			html = html.Replace("{{pagetitle}}", page.Title);
			html = html.Replace("{{navtitle}}", page.NavTitle);
			html = html.Replace("{{pagehero}}", page.HeroImage);
			return html;
		}

		public static string GetAddBlockForm(CMSPage page)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\admin\\dialogaddblock.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{pageid}}", page.PageId.ToString());
			return html;
		}

		public static string GetBlockConfig(CMSBlock block)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\admin\\dialogblock.htm";
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
