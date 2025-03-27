using Microsoft.AspNetCore.Mvc.Rendering;

namespace dabrelCMS.code
{
	public static class AdminPage
	{
		public static string GetPageForm(CMSPage page)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\pageform.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{pageid}}", page.PageId.ToString());
			html = html.Replace("{{parentid}}", page.ParentId.ToString());
			html = html.Replace("{{pageshortcut}}", page.Shortcut);
			html = html.Replace("{{pagesort}}", page.Sort.ToString());
			html = html.Replace("{{pageison}}", page.isOn.ToString());
			html = html.Replace("{{pageisprivate}}", page.isPrivate.ToString());
			html = html.Replace("{{pageishidden}}", page.isHidden.ToString());
			html = html.Replace("{{pagetags}}", page.Tags);
			html = html.Replace("{{pagetitle}}", page.Title);
			html = html.Replace("{{navtitle}}", page.NavTitle);
			html = html.Replace("{{content}}", page.Summary);
			html = html.Replace("{{pagehero}}", page.HeroImage);
			return html;
		}
		public static string GetPageConfig(CMSPage page)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogpage.htm";
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

	}
}
