using dabrelCMS.models;
using Htmx;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace dabrelCMS.code
{
	public class Admin
	{
		public string GetPage(HttpContext context)
		{
			string _domain = string.Empty;
			string _path = string.Empty;
			string _webRootPath = string.Empty;
			string html = string.Empty;

			_webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			_domain = context.Request.Host.ToString().ToLower().Trim();
			// strip off any port numbers that may be involved
			_domain = _domain.Substring(0, _domain.IndexOf(":"));
			_path = context.Request.Path.ToString().ToLower().Trim().Substring(1) + "/";
			string[] pathsegments = _path.Split("/");
			// 0 should always be admin
			// 1 should be shortcut
			// 2 would be pageid if editing

			using CMSDbContext dbcontext = new CMSDbContext();

			// type: cmsSiteUrl
			var url = dbcontext.CMSSiteUrls.Where(x => x.Url == _domain).FirstOrDefault();

			// type: cmsSite
			var site = dbcontext.CMSSites.Where(s => s.SiteId == url.SiteId).FirstOrDefault();

			string design = "admin";
			string designPath = $"{_webRootPath}\\designs\\{design}\\{design}.htm";

			// type: cmsPage
			var page = new CMSPage(); // we'll load the page in the switch statement
			page = dbcontext.CMSPages.Where(p => p.Shortcut == pathsegments[1]).FirstOrDefault();

			if (context.Request.IsHtmx())
			{
				StringBuilder sb = new StringBuilder();
				if (context.Request.HasFormContentType)
				{
					switch (pathsegments[1])
					{
						case "siteform":
							if (context.Request.Form["design"] != string.Empty)
							{
								site.Title = context.Request.Form["sitetitle"].ToString();
								site.SubTitle = context.Request.Form["subtitle"].ToString();
								site.Design = context.Request.Form["design"].ToString();
								site.FaviconUrl = context.Request.Form["faviconurl"].ToString();
								site.MetaDescription = context.Request.Form["metadescription"].ToString();
								site.MetaImagePath = context.Request.Form["metaimagepath"].ToString();
								site.BodyTop = context.Request.Form["bodytop"].ToString();
								site.BodyBottom = context.Request.Form["bodybottom"].ToString();
								site.OnAllPages = context.Request.Form["onallpages"].ToString();
								site.Footer1 = context.Request.Form["footer1"].ToString();
								site.Footer2 = context.Request.Form["footer2"].ToString();
								site.Footer3 = context.Request.Form["footer3"].ToString();
								site.Footer4 = context.Request.Form["footer4"].ToString();
								dbcontext.SaveChanges();
								context.Response.Headers["HX-Redirect"] = $"/admin/";
								return "<h2>Success<h2>";
							}
							else
								return "<h2>You need a design</h2>";

						case "getpageform":
							page = dbcontext.CMSPages.Where(p => p.PageId == int.Parse(pathsegments[2])).FirstOrDefault();
							string pageformPath = $"{_webRootPath}\\designs\\{design}\\pageform.htm";
							html = Common.GetFileText(pageformPath);
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
							break;

						case "getaddpageform":
							string addpageformPath = $"{_webRootPath}\\designs\\{design}\\addpageform.htm";
							html = Common.GetFileText(addpageformPath);
							if (pathsegments.Length > 2)
								if (pathsegments[2] != string.Empty)
								{
									int p = 0;
									int.TryParse(pathsegments[2], out p);
									html = html.Replace("{{parentid}}", p.ToString());
								}
							break;

						case "addpageform":
							page = new CMSPage();
							dbcontext.CMSPages.Add(page);
							page.Title = context.Request.Form["pagetitle"].ToString();
							page.SiteId = site.SiteId;
							page.NavTitle = page.Title;
							page.Shortcut = HttpUtility.UrlEncode(page.Title.ToLower().Replace(" ", String.Empty));
							page.ParentId = int.Parse(context.Request.Form["parentid"].ToString());
							page.Sort = 1;
							page.isOn = true;
							page.isPrivate = false;
							page.isHidden = false;
							page.Tags = page.Title.ToLower();
							page.HeroImage = string.Empty;
							page.Summary = context.Request.Form["content"].ToString();
							dbcontext.SaveChanges();
							context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
							//context.Response.Redirect($"/admin/{page.Shortcut}");
							break;

						case "pageform":
							// save page here
							int id = 0;
							string sid = context.Request.Form["pageid"].ToString().Trim();
							bool isEdit = int.TryParse(sid, out id);
							if (isEdit)
								page = dbcontext.CMSPages.Where(p => p.PageId == id).FirstOrDefault();
							else
							{
								page = new CMSPage();
								dbcontext.CMSPages.Add(page);
								// page.PageId = 0;
							}
							page.Title = context.Request.Form["pagetitle"].ToString();
							page.NavTitle = context.Request.Form["navtitle"].ToString();
							page.Shortcut = context.Request.Form["pageshortcut"].ToString();
							page.ParentId = int.Parse(context.Request.Form["parentid"].ToString());
							page.Sort = int.Parse(context.Request.Form["pagesort"].ToString());
							page.isOn = bool.Parse(context.Request.Form["pageison"].ToString());
							page.isPrivate = bool.Parse(context.Request.Form["pageisprivate"].ToString());
							page.isHidden = bool.Parse(context.Request.Form["pageishidden"].ToString());
							page.Tags = context.Request.Form["pagetags"].ToString();
							page.HeroImage = context.Request.Form["pagehero"].ToString();
							page.Summary = context.Request.Form["content"].ToString();
							dbcontext.SaveChanges();
							context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
							break;

						case "deletepage":
							if (pathsegments.Length > 2)
							{
								if (pathsegments[2] != string.Empty)
								{
									int delId = 0;
									int.TryParse(pathsegments[2], out delId);
									page = dbcontext.CMSPages.Where(p => p.PageId == delId).FirstOrDefault();
									// don't let them delete the home page.
									if (page != null && page.Shortcut != string.Empty)
									{
										int redirectid = page.ParentId;
										CMSPage redirect;
										if (redirectid == 0)
											redirect = dbcontext.CMSPages.Where(p => p.Shortcut == string.Empty).FirstOrDefault();
										else
											redirect = dbcontext.CMSPages.Where(p => p.PageId == redirectid).FirstOrDefault();
										dbcontext.CMSPages.Remove(page);
										dbcontext.SaveChanges();
										context.Response.Headers["HX-Redirect"] = $"/admin/{redirect.Shortcut}";
									}
								}
							}
							break;

						case "getadduserform":
							string adduserformPath = $"{_webRootPath}\\designs\\{design}\\adduserform.htm";
							html = Common.GetFileText(adduserformPath);
							break;
					}
				}
				else
				{
					sb.Append($"<section>");
					//sb.Append($"<div class=\"buttonright\" hx-post=\"/admin/getpageform/{page.PageId}\" hx-target=\"#pagecontent\"><i class=\"fa-regular fa-pen-to-square\"></i></div>");
					sb.Append($"<div class=\"buttonright\">");
					sb.Append($"<span id=\"admineditpage\" hx-post=\"/admin/getpageform/{page.PageId}\" hx-target=\"#pagecontent\"><i class=\"fa-regular fa-pen-to-square\"></i></span>");
					sb.Append($"<span id=\"adminaddpage\" hx-post=\"/admin/getaddpageform/{page.PageId}\" hx-target=\"#pagecontent\"><i class=\"fa-regular fa-plus\"></i></span>");
					sb.Append($"<span id=\"admindeletepage\" hx-post=\"/admin/deletepage/{page.PageId}\" hx-confirm=\"Are you absolutely sure you wish to delete this page?\" hx-target=\"#pagecontent\"><i class=\"fa-regular fa-trash\"></i></span>");
					sb.Append($"</div>");
					sb.Append($"<h1 id=\"pagetitle\">{page.Title}</h1>");
					sb.Append($"<div id=\"content\" class=\"body\">{page.Summary}</div>");
					sb.Append($"</section>");
					html = sb.ToString();
				}
			}
			else
			{
				if (design == null || design.Length == 0)
					return Common.Send404(context);
				// load design template
				html = Common.GetFileText(designPath);

				// do content replacements
				if (site.ImageFileName != null && site.ImageFileName != string.Empty)
				{
					html = html.Replace("{{titleimage}}", $"<img class=\"titleimage\" src=\"{site.ImageFileName}\" alt=\"{site.Title}\" />");
					html = html.Replace("{{headersitetitle}}", string.Empty);
				}
				else
				{
					html = html.Replace("{{titleimage}}", string.Empty);
					html = html.Replace("{{headersitetitle}}", $"<span class=\"titletext\">{site.Title}</span>");
				}
				html = html.Replace("{{sitename}}", site.Name);
				html = html.Replace("{{design}}", site.Design);
				html = html.Replace("{{faviconurl}}", site.FaviconUrl);
				html = html.Replace("{{sitetitle}}", site.Title);
				html = html.Replace("{{subtitle}}", site.SubTitle);
				html = html.Replace("{{pageid}}", page.PageId.ToString());
				html = html.Replace("{{shortcut}}", page.Shortcut);
				html = html.Replace("{{pagetitle}}", page.Title);
				html = html.Replace("{{description}}", site.MetaDescription);
				html = html.Replace("{{metaimagepath}}", site.MetaImagePath);
				html = html.Replace("{{onallpages}}", site.OnAllPages);
				html = html.Replace("{{created}}", site.Created.ToString());
				html = html.Replace("{{bodytop}}", site.BodyTop);
				if (page.HeroImage == null) { html = html.Replace("{{hero}}", ""); }
				else { html = html.Replace("{{hero}}", page.HeroImage); }
				html = html.Replace("{{content}}", page.Summary);
				html = html.Replace("{{bodybottom}}", site.BodyBottom);
				html = html.Replace("{{footer1}}", site.Footer1);
				html = html.Replace("{{footer2}}", site.Footer2);
				html = html.Replace("{{footer3}}", site.Footer3);
				html = html.Replace("{{footer4}}", site.Footer4);

				//get navigation
				List<CMSPage> nav = dbcontext.CMSPages.Where(n => n.SiteId == site.SiteId)
					.OrderBy(o => o.ParentId).ThenBy(x => x.Sort).ToList();
				html = html.Replace("{{navigation}}", GetNav(nav, 0, page.PageId));
			}
			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;

			Regex r = new Regex(@"\{\{.*\}\}");
			html = r.Replace(html, String.Empty);
			return html;
		}

		private string GetNav(List<CMSPage> nav, int pId, int currentId)
		{
			IEnumerable<CMSPage> parent = nav.Where(y => y.ParentId == pId);
			StringBuilder c = new StringBuilder();
			c.Append("<ul>");
			foreach (CMSPage page in parent)
			{
				bool gotkids = HasChildren(nav, page.PageId);
				bool current = page.PageId == currentId;
				if (gotkids && current)
				{ c.Append($"<li id=\"{page.PageId}\" class=\"haschildren path\">"); }
				else if (gotkids)
				{ c.Append($"<li id=\"{page.PageId}\" class=\"haschildren\">"); }
				else if (current)
				{ c.Append($"<li id=\"{page.PageId}\" class=\"path\">"); }
				else
				{ c.Append($"<li id=\"{page.PageId}\">"); }

				c.Append($"<a href=\"/admin/{page.Shortcut}\" hx-get=\"/admin/{page.Shortcut}\" hx-target=\"#pagecontent\" hx-swap=\"innerHTML\" hx-push-url=\"true\">{page.NavTitle}");
				c.Append($"</a>");
				if (gotkids)
				{
					c.Append($"<span class=\"sub\"><i class=\"fa-solid fa-plus\"></i></span>");
				}
				c.Append(GetNav(nav, page.PageId, currentId));
				c.Append($"</li>");
			}
			c.Append("</ul>");
			return c.ToString();
		}

		private bool HasChildren(List<CMSPage> nav, int pId)
		{
			IEnumerable<CMSPage> parent = nav.Where(y => y.ParentId == pId);
			if (parent.Count() > 0)
				return true;
			else
				return false;
		}
	}
}