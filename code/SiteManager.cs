using dabrelCMS.models;
using Htmx;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Text.RegularExpressions;

namespace dabrelCMS.code
{
	public class SiteManager
	{
		public string GetPage(HttpContext context, CMSUser authUser)
		{
			string _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			Common.WebRootPath = _webRootPath;

			string _path = context.Request.Path.ToString().ToLower().Trim().Substring(1) + "/";
			string[] pathsegments = _path.Split("/");
			// 0 = sitemanager, 1 = action, 2 = optional id

			using CMSDbContext dbcontext = new CMSDbContext();

			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;

			if (context.Request.IsHtmx() && context.Request.HasFormContentType)
			{
				switch (pathsegments[1])
				{
					case "addsite":
						return HandleAddSite(context, dbcontext);

					case "deletesite":
						return HandleDeleteSite(context, dbcontext, pathsegments);

					case "addsiteurl":
						return HandleAddSiteUrl(context, dbcontext, pathsegments);

					case "deletesiteurl":
						return HandleDeleteSiteUrl(context, dbcontext, pathsegments);
				}
			}

			string designPath = $"{_webRootPath}\\designs\\sitemanager\\sitemanager.htm";
			string html = Common.GetFileText(designPath);
			html = html.Replace("{{sitelist}}", BuildSiteList(dbcontext));

			Regex r = new Regex(@"\{\{.*\}\}");
			html = r.Replace(html, string.Empty);
			return html;
		}

		private string HandleAddSite(HttpContext context, CMSDbContext dbcontext)
		{
			string name = context.Request.Form["sitename"].ToString().Trim();
			string url = context.Request.Form["siteurl"].ToString().Trim().ToLower();
			string design = context.Request.Form["design"].ToString().Trim();

			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
				return "<div class=\"error\">Site name and URL are required.</div>";

			if (dbcontext.CMSSiteUrls.Any(u => u.Url == url))
				return $"<div class=\"error\">URL '{url}' is already assigned to a site.</div>";

			var site = new CMSSite
			{
				SiteId = Guid.CreateVersion7(),
				Name = name,
				Design = design.Length > 0 ? design : "mountain",
				Title = name,
				SubTitle = "",
				Footer1 = "", Footer2 = "", Footer3 = "", Footer4 = "",
				MetaDescription = "", MetaImagePath = "", OnAllPages = "",
				BodyTop = "", BodyBottom = "", ImageFileName = "",
				Created = DateTime.Now,
				FaviconUrl = ""
			};
			dbcontext.CMSSites.Add(site);
			dbcontext.CMSSiteUrls.Add(new CMSSiteUrl
			{
				SiteUrlId = Guid.CreateVersion7(),
				SiteId = site.SiteId,
				Url = url,
				Primary = true
			});
			dbcontext.SaveChanges();
			dbcontext.CMSPages.Add(new CMSPage
			{
				PageId = Guid.CreateVersion7(),
				ParentId = null,
				Sort = 0,
				SiteId = site.SiteId,
				isOn = true,
				isPrivate = false,
				isHidden = false,
				Shortcut = "",
				Tags = "home",
				NavTitle = "Home",
				Title = "Home",
				Summary = "<h1>Welcome to your new site!</h1><p>This is the home page. You can edit this page and add more pages in the CMS.</p>",
				HeroImage = ""
			});
			dbcontext.SaveChanges();
			context.Response.Headers["HX-Redirect"] = "/sitemanager/";
			return "";
		}

		private string HandleDeleteSite(HttpContext context, CMSDbContext dbcontext, string[] pathsegments)
		{
			if (pathsegments.Length < 3 || !Guid.TryParse(pathsegments[2], out Guid siteId))
				return "<div class=\"error\">Invalid site ID.</div>";

			if (dbcontext.CMSPages.Any(p => p.SiteId == siteId))
				return "<div class=\"error\">Remove all pages from this site before deleting it.</div>";

			var urls = dbcontext.CMSSiteUrls.Where(u => u.SiteId == siteId).ToList();
			dbcontext.CMSSiteUrls.RemoveRange(urls);
			var site = dbcontext.CMSSites.Find(siteId);
			if (site != null)
				dbcontext.CMSSites.Remove(site);
			dbcontext.SaveChanges();
			context.Response.Headers["HX-Redirect"] = "/sitemanager/";
			return "";
		}

		private string HandleAddSiteUrl(HttpContext context, CMSDbContext dbcontext, string[] pathsegments)
		{
			if (pathsegments.Length < 3 || !Guid.TryParse(pathsegments[2], out Guid siteId))
				return "<div class=\"error\">Invalid site ID.</div>";

			string url = context.Request.Form["url"].ToString().Trim().ToLower();
			if (string.IsNullOrWhiteSpace(url))
				return "<div class=\"error\">URL is required.</div>";

			if (dbcontext.CMSSiteUrls.Any(u => u.Url == url))
				return $"<div class=\"error\">URL '{url}' is already in use.</div>";

			bool isPrimary = context.Request.Form["primary"].ToString().Length > 0;
			if (isPrimary)
			{
				var existing = dbcontext.CMSSiteUrls.Where(u => u.SiteId == siteId && u.Primary).ToList();
				foreach (var ep in existing) ep.Primary = false;
			}

			dbcontext.CMSSiteUrls.Add(new CMSSiteUrl
			{
				SiteUrlId = Guid.CreateVersion7(),
				SiteId = siteId,
				Url = url,
				Primary = isPrimary
			});
			dbcontext.SaveChanges();
			context.Response.Headers["HX-Redirect"] = "/sitemanager/";
			return "";
		}

		private string HandleDeleteSiteUrl(HttpContext context, CMSDbContext dbcontext, string[] pathsegments)
		{
			if (pathsegments.Length < 3 || !Guid.TryParse(pathsegments[2], out Guid siteUrlId))
				return "<div class=\"error\">Invalid URL ID.</div>";

			var url = dbcontext.CMSSiteUrls.Find(siteUrlId);
			if (url != null)
			{
				dbcontext.CMSSiteUrls.Remove(url);
				dbcontext.SaveChanges();
			}
			context.Response.Headers["HX-Redirect"] = "/sitemanager/";
			return "";
		}

		private string BuildSiteList(CMSDbContext dbcontext)
		{
			var sites = dbcontext.CMSSites.OrderBy(s => s.Name).ToList();
			var allUrls = dbcontext.CMSSiteUrls.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var site in sites)
			{
				var siteUrls = allUrls.Where(u => u.SiteId == site.SiteId)
					.OrderByDescending(u => u.Primary).ToList();

				sb.Append($"<div class=\"siterow\">");
				sb.Append($"<div class=\"siteheader\">");
				sb.Append($"<div><strong>{site.Name}</strong> <span class=\"designlabel\">design: {site.Design}</span></div>");
				sb.Append($"<button class=\"deletebutton\" hx-post=\"/sitemanager/deletesite/{site.SiteId}\" hx-target=\"#sitemessage\" hx-confirm=\"Delete site '{site.Name}'? This cannot be undone.\" title=\"Delete site\"><i class=\"fa-solid fa-trash\"></i></button>");
				sb.Append($"</div>");

				sb.Append($"<ul class=\"siteurls\">");
				foreach (var url in siteUrls)
				{
					sb.Append($"<li class=\"urlrow\">");
					sb.Append($"<span class=\"urltext\">{url.Url}</span>");
					if (url.Primary)
						sb.Append($" <span class=\"badge\">primary</span>");
					sb.Append($"<button class=\"deletebutton\" hx-post=\"/sitemanager/deletesiteurl/{url.SiteUrlId}\" hx-target=\"#sitemessage\" hx-confirm=\"Remove URL '{url.Url}'?\" title=\"Remove URL\"><i class=\"fa-solid fa-trash\"></i></button>");
					sb.Append($"</li>");
				}
				sb.Append($"</ul>");

				sb.Append($"<form class=\"addurlform\" hx-post=\"/sitemanager/addsiteurl/{site.SiteId}\" hx-target=\"#sitemessage\">");
				sb.Append($"<input name=\"url\" type=\"text\" placeholder=\"new-domain.com\" required />");
				sb.Append($"<label><input type=\"checkbox\" name=\"primary\" /> primary</label>");
				sb.Append($"<button type=\"submit\" title=\"Add URL\"><i class=\"fa-solid fa-plus\"></i> Add URL</button>");
				sb.Append($"</form>");

				sb.Append($"</div>");
			}

			sb.Append("<div class=\"addsiteform\">");
			sb.Append("<h3><i class=\"fa-solid fa-plus\"></i> Add New Site</h3>");
			sb.Append("<form hx-post=\"/sitemanager/addsite\" hx-target=\"#sitemessage\">");
			sb.Append("<div class=\"field\"><label>Site Name</label><input name=\"sitename\" type=\"text\" placeholder=\"My Site\" required /></div>");
			sb.Append("<div class=\"field\"><label>Primary URL / Domain</label><input name=\"siteurl\" type=\"text\" placeholder=\"domain.com\" required /></div>");
			sb.Append("<div class=\"field\"><label>Design</label><input name=\"design\" type=\"text\" placeholder=\"mountain\" /></div>");
			sb.Append("<div class=\"buttonright\"><button type=\"submit\" class=\"addsite\"><i class=\"fa-solid fa-plus\"></i> Add Site</button></div>");
			sb.Append("</form>");
			sb.Append("</div>");

			return sb.ToString();
		}
	}
}
