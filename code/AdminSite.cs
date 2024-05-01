namespace dabrelCMS.code
{
	public static class AdminSite
	{
		public static string GetSiteForm(CMSSite site)
		{
			string siteformPath = $"{Common.WebRootPath}\\designs\\admin\\dialogsite.htm";
			string html = Common.GetFileText(siteformPath);
			html = html.Replace("{{sitename}}", site.Name.ToString());
			html = html.Replace("{{created}}", site.Created.ToString());
			html = html.Replace("{{design}}", site.Design.ToString());
			html = html.Replace("{{faviconurl}}", site.FaviconUrl.ToString());
			html = html.Replace("{{sitetitle}}", site.Title.ToString());
			html = html.Replace("{{subtitle}}", site.SubTitle.ToString());
			html = html.Replace("{{description}}", site.MetaDescription.ToString());
			html = html.Replace("{{metaimagepath}}", site.MetaImagePath.ToString());
			html = html.Replace("{{onallpages}}", site.OnAllPages.ToString());
			html = html.Replace("{{bodytop}}", site.BodyTop.ToString());
			html = html.Replace("{{bodybottom}}", site.BodyBottom.ToString());
			html = html.Replace("{{footer1}}", site.Footer1.ToString());
			html = html.Replace("{{footer2}}", site.Footer2.ToString());
			html = html.Replace("{{footer3}}", site.Footer3.ToString());
			html = html.Replace("{{footer4}}", site.Footer4.ToString());
			return html;
		}

	}
}
