using dabrelCMS.models;
using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace dabrelCMS.code
{
	internal class Site
	{
		private string _domain = string.Empty;
		private string _path = string.Empty;
		private string _webRootPath = string.Empty;

		internal string GetSite(HttpContext context)
		{
			_webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

			string html = string.Empty;
			_domain = context.Request.Host.ToString().ToLower().Trim();
			// strip off any port numbers that may be on there
			_domain = _domain.Substring(0, _domain.IndexOf(":"));
			_path = context.Request.Path.ToString().ToLower().Trim().Replace("/", string.Empty);
			if (_path.StartsWith("admin"))
				_path = "admin";

			using data.CMSDbContext dbcontext = new data.CMSDbContext();

			// type: cmsSiteUrl
			var url = dbcontext.CMSSiteUrls.Where(x => x.Url == _domain).FirstOrDefault();
			// type: cmsSite
			var site = dbcontext.CMSSites.Where(s => s.SiteId == url.SiteId).FirstOrDefault();
			// type: cmsPage
			var page = new CMSPage(); // we'll load the page in the switch statement
			System.Linq.IQueryable<CMSPage> searchresults = null;

			// get cookie and user id from jwt
			code.JwtUtils jwt = new JwtUtils();
			string? token = context.Request.Cookies["token"];
			int? userId = jwt.ValidateToken(token, CMSConfig.JwtKey, _domain);
			CMSUser? authUser = null;
			if (userId != null)
			{
				authUser = dbcontext.CMSUsers.Where(u => u.UserId == userId).FirstOrDefault();
			}

			switch (_path)
			{
				case "auth":
					{
						if (context.Request.Form["username"].ToString().Length > 0)
						{
							bool bAuth = false;
							authUser = dbcontext.CMSUsers.Where(
								u => u.Email == context.Request.Form["username"].ToString()
								&& u.SiteId == site.SiteId).FirstOrDefault();
							if (authUser == null)
								return Common.GetLoginPage(context, "/", "Please Login");
							else
							{
								int uSalt = authUser.Salt;
								string uHash = authUser.Password;
								if (uSalt > 0)
								{
									if (PWHash.IsPasswordValid(context.Request.Form["password"].ToString(), uSalt, uHash))
										bAuth = true;
								}
								else
								{
									if (uHash == context.Request.Form["password"].ToString())
										bAuth = true;
								}
								if (bAuth)
								{
									string newtoken = jwt.GenerateToken(authUser, CMSConfig.JwtKey, _domain);
									context.Response.Cookies.Append("token", newtoken, new CookieOptions
									{
										Secure = true,
										HttpOnly = true,
										SameSite = SameSiteMode.Strict
									});

									if (context.Request.Form["redirect"].ToString().Length > 0)
									{
										context.Response.StatusCode = StatusCodes.Status200OK;
										context.Response.Redirect(context.Request.Form["redirect"].ToString());
										return "";
									}
								}
								else
								{
									return Common.GetLoginPage(context, "/", "Please Login");
								}
							}
						}
						break;
					}
				case "admin":
					{
						if (authUser != null)
						{
							Admin a = new Admin();
							return a.GetPage(context, authUser);
						}
						else
							return Common.GetLoginPage(context, _path, "Please Login");
					}
				case "manager":
					{
						if (authUser != null)
						{
							return Common.GetManagerPage(context);
						}
						else
							return Common.GetLoginPage(context, _path, "Please Login");
					}
				case "search":
					{
						if (!String.IsNullOrEmpty(context.Request.Form["searchText"]))
						{
							string searchText = context.Request.Form["searchText"];
							searchresults = dbcontext.CMSPages.Where(p => p.Shortcut.Contains(searchText) 
							|| p.Title.Contains(searchText) || p.Tags.Contains(searchText) 
							|| p.Summary.Contains(searchText) && p.isOn == true);
						}
						break;
					}
				default:
					{
						page = dbcontext.CMSPages.Where(p => p.Shortcut == _path && p.isOn == true).FirstOrDefault();
						if (page == null)
							return Common.Send404(context);
						if (page.isPrivate == true && authUser == null)
							return Common.GetLoginPage(context, _path, "Please log in to view content.");

						break;
					}
			}
			//var items = dbcontext.cmsItems.Where(i => i.cmsPageId == page.cmsPageId).ToList<cmsItem>();

			if (context.Request.IsHtmx())
			{
				if (searchresults != null)
				{
					StringBuilder rs = new StringBuilder();
					rs.Append($"<section>");
					rs.Append($"<h1>Search Results</h1>");
					foreach (CMSPage result in searchresults)
					{
						string trunkatedsummary = result.Summary.Length > 250 ? result.Summary.Substring(0, 250) : result.Summary;
						rs.Append($"<div class=\"searchresult\">");
						rs.Append($"<h3><a href=\"/{result.Shortcut}\">{result.Title}</a></h3>");
						rs.Append($"<div class=\"summary\">{trunkatedsummary}</div>");
						rs.Append($"</div>");
					}
					rs.Append($"</section>");
					html = rs.ToString();
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append($"<section>");
					sb.Append($"<h1>{page.Title}</h1>");
					sb.Append($"<div class=\"body\">{page.Summary}</div>");
					sb.Append($"</section>");
					html = sb.ToString();
				}
			}
			else
			{
				string design = site.Design;
				string designPath = $"{_webRootPath}\\designs\\{design}\\{design}.htm";
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
				html = html.Replace("{{sitetitle}}", site.Title);
				html = html.Replace("{{pagetitle}}", page.Title);
				html = html.Replace("{{subtitle}}", site.SubTitle);
				html = html.Replace("{{description}}", site.MetaDescription);
				html = html.Replace("{{bodytop}}", site.BodyTop);
				if (page.HeroImage == null) { html = html.Replace("{{hero}}", ""); }
				else { html = html.Replace("{{hero}}", page.HeroImage); }
				html = html.Replace("{{content}}", page.Summary);
				html = html.Replace("{{bodybottom}}", site.BodyBottom);
				html = html.Replace("{{footer1}}", site.Footer1.Length > 0 ? $"<div>{site.Footer1}</div>" : "");
				html = html.Replace("{{footer2}}", site.Footer2.Length > 0 ? $"<div>{site.Footer2}</div>" : "");
				html = html.Replace("{{footer3}}", site.Footer3.Length > 0 ? $"<div>{site.Footer3}</div>" : "");
				html = html.Replace("{{footer4}}", site.Footer4.Length > 0 ? $"<div>{site.Footer4}</div>" : "");

				//get navigation
				List<CMSPage> nav = dbcontext.CMSPages.Where(n => n.SiteId == site.SiteId)
					.OrderBy(o => o.ParentId).ThenBy(x => x.Sort).ToList();
				html = html.Replace("{{navigation}}", GetNav(nav, 0, page.PageId));
			}
			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;
			return html;
		}

		private string GetNav(List<CMSPage> nav, int pId, int currentId)
		{
			IEnumerable<CMSPage> parent = nav.Where(y => y.ParentId == pId && y.isOn == true && y.isHidden == false);
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

				c.Append($"<a href=\"/{page.Shortcut}\" hx-get=\"/{page.Shortcut}\" hx-target=\"#pagecontent\" hx-swap=\"innerHTML\" hx-push-url=\"true\">{page.NavTitle}");
				c.Append($"</a>");
				if (gotkids)
				{
					c.Append($"<button class=\"sub\" aria-label=\"open sub nav\"><i class=\"fa-solid fa-plus\"></i></button>");
				}
				c.Append(GetNav(nav, page.PageId, currentId));
				c.Append($"</li>");
			}
			c.Append("</ul>");
			return c.ToString();
		}

		private bool HasChildren(List<CMSPage> nav, int pId)
		{
			IEnumerable<CMSPage> parent = nav.Where(y => y.ParentId == pId && y.isOn == true && y.isHidden == false);
			if (parent.Count() > 0)
				return true;
			else
				return false;
		}
	}
}