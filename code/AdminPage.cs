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
	}
}
