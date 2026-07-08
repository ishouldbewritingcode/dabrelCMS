using dabrelCMS.models;
using Htmx;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using OtpNet;
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
		private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<Site>();

		private string _domain = string.Empty;
		private string _path = string.Empty;
		private string _webRootPath = string.Empty;

		internal string GetSite(HttpContext context)
		{
			_webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

			string html = string.Empty;
			_domain = context.Request.Host.ToString().ToLower().Trim();
			// strip off any port numbers that may be on there
			int colonIndex = _domain.IndexOf(":");
			if (colonIndex > 0)
				_domain = _domain.Substring(0, colonIndex);
			_path = context.Request.Path.ToString().ToLower().Trim().Replace("/", string.Empty);
			if (_path.StartsWith("admin"))
				_path = "admin";
			if (_path.StartsWith("sitemanager"))
				_path = "sitemanager";

			var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
			bool isHtmx = context.Request.IsHtmx();
#if DEBUG
			bool requestMightBeCacheable = false;
#else
			bool requestMightBeCacheable = context.Request.Method == "GET"
				&& _path != "admin"
				&& _path != "sitemanager"
				&& _path != "auth"
				&& _path != "search";
#endif
			string cacheKey = $"page:{_domain}:{_path}:{(isHtmx ? "htmx" : "full")}";
			if (requestMightBeCacheable && cache.TryGetValue(cacheKey, out string? cachedHtml))
			{
				_log.Information("Serving cached page {Path} on {Domain} with key {CacheKey}", _path, _domain, cacheKey);
				context.Response.Headers["Content-Type"] = "text/html";
				context.Response.StatusCode = StatusCodes.Status200OK;
				return cachedHtml!;
			}
			bool shouldCache = false;

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
			Guid? userId = jwt.ValidateToken(token, CMSConfig.JwtKey, _domain);
CMSUser? authUser = null;
			if (userId != null)
			{
				authUser = dbcontext.CMSUsers.Where(u => u.UserId == userId).FirstOrDefault();
			}

			switch (_path)
			{
				case "auth":
					{
						string step = context.Request.Form["step"].ToString();

						if (step == "totp")
						{
							// Step 2: verify TOTP code
							string? pendingToken = context.Request.Cookies["totp_pending"];
							Guid? pendingUserId = jwt.ValidatePendingTotpToken(pendingToken, CMSConfig.JwtKey, _domain);
							if (pendingUserId == null)
							{
								_log.Warning("TOTP session expired or invalid pending token on {Domain}", _domain);
								return Common.GetLoginPage(context, context.Request.Form["redirect"].ToString(), "Session expired. Please login again.");
							}

							authUser = dbcontext.CMSUsers.Where(u => u.UserId == pendingUserId).FirstOrDefault();
							if (authUser == null || string.IsNullOrEmpty(authUser.TotpSecret))
								return Common.GetLoginPage(context, "/", "Please Login");

							string submittedCode = context.Request.Form["totpcode"].ToString().Trim();
							var totp = new Totp(Base32Encoding.ToBytes(authUser.TotpSecret));
							long windowUsed;
							bool totpValid = totp.VerifyTotp(submittedCode, out windowUsed, new VerificationWindow(2, 2));

							if (!totpValid || !TotpReplayCache.TryMarkUsed(authUser.UserId, windowUsed))
							{
								_log.Warning("TOTP verification failed for user {UserId} on {Domain}", authUser.UserId, _domain);
								return Common.GetTotpPage(context, context.Request.Form["redirect"].ToString(), "Invalid or already-used code. Please try again.");
							}

							// clear pending cookie, issue session token
							_log.Information("TOTP verified for user {UserId} on {Domain}", authUser.UserId, _domain);
							context.Response.Cookies.Delete("totp_pending");
							string newtoken = jwt.GenerateToken(authUser, CMSConfig.JwtKey, _domain);
							context.Response.Cookies.Append("token", newtoken, new CookieOptions
							{
								Secure = context.Request.IsHttps,
								HttpOnly = true,
								SameSite = SameSiteMode.Strict
							});
							string totpRedirect = context.Request.Form["redirect"].ToString();
							context.Response.StatusCode = StatusCodes.Status200OK;
							context.Response.Redirect(totpRedirect.Length > 0 ? totpRedirect : "/");
							return "";
						}
						else if (context.Request.Form["username"].ToString().Length > 0)
						{
							// Step 1: verify password
							bool bAuth = false;
							authUser = dbcontext.CMSUsers.Where(
								u => u.Email == context.Request.Form["username"].ToString()
								&& u.SiteId == site.SiteId).FirstOrDefault();
							if (authUser == null)
							{
								_log.Warning("Login attempt for unknown user {Email} on {Domain}", context.Request.Form["username"].ToString(), _domain);
								return Common.GetLoginPage(context, "/", "Please Login");
							}

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

							if (!bAuth)
							{
								_log.Warning("Failed login attempt for {Email} on {Domain}", authUser.Email, _domain);
								return Common.GetLoginPage(context, "/", "Please Login");
							}

							_log.Information("Password verified for {Email} on {Domain}", authUser.Email, _domain);
							string redirect = context.Request.Form["redirect"].ToString();

							if (!string.IsNullOrEmpty(authUser.TotpSecret))
							{
								// TOTP is enabled — issue pending cookie and show TOTP page
								string pendingTok = jwt.GeneratePendingTotpToken(authUser.UserId, CMSConfig.JwtKey, _domain);
								context.Response.Cookies.Append("totp_pending", pendingTok, new CookieOptions
								{
									Secure = context.Request.IsHttps,
									HttpOnly = true,
									SameSite = SameSiteMode.Strict
								});
								return Common.GetTotpPage(context, redirect, "");
							}

							_log.Information("User {Email} logged in on {Domain}", authUser.Email, _domain);
							string newtoken = jwt.GenerateToken(authUser, CMSConfig.JwtKey, _domain);
							context.Response.Cookies.Append("token", newtoken, new CookieOptions
							{
								Secure = context.Request.IsHttps,
								HttpOnly = true,
								SameSite = SameSiteMode.Strict
							});

							context.Response.StatusCode = StatusCodes.Status200OK;
							context.Response.Redirect(redirect.Length > 0 ? redirect : "/admin");
							return "";
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
						{
							_log.Warning("Unauthenticated access attempt to admin on {Domain}", _domain);
							return Common.GetLoginPage(context, _path, "Please Login");
						}
					}
				case "sitemanager":
					{
						if (authUser != null)
						{
							// need to check to see if this user has the sitemanager role
							if (authUser.RoleList.Contains("sitemanager"))
								return new SiteManager().GetPage(context, authUser);
							else
							{
								_log.Warning("User {UserId} denied access to sitemanager on {Domain} (missing role)", authUser.UserId, _domain);
								return Common.GetLoginPage(context, _path, "Please Login");
							}
						}
						else
						{
							_log.Warning("Unauthenticated access attempt to sitemanager on {Domain}", _domain);
							return Common.GetLoginPage(context, _path, "Please Login");
						}
					}
				case "search":
					{
						if (!string.IsNullOrEmpty(context.Request.Form["searchText"]))
						{
							string searchText = context.Request.Form["searchText"].ToString();

							var pageResults = dbcontext.CMSPages.Where(p => p.isOn == true && (
								(p.Shortcut != null && p.Shortcut.Contains(searchText))
								|| (p.Title != null && p.Title.Contains(searchText))
								|| (p.Tags != null && p.Tags.Contains(searchText))
								|| (p.Summary != null && p.Summary.Contains(searchText))));

							var pageIdsFromItems = dbcontext.CMSItems
								.Where(i => (i.Title1 != null && i.Title1.Contains(searchText))
									|| (i.Title2 != null && i.Title2.Contains(searchText))
									|| (i.Data != null && i.Data.Contains(searchText)))
								.Join(dbcontext.CMSPageBlocks, i => i.BlockId, pb => pb.BlockId, (i, pb) => pb.PageId)
								.Distinct();

							searchresults = pageResults.Union(
								dbcontext.CMSPages.Where(p => p.isOn == true && pageIdsFromItems.Contains(p.PageId)));
						}
						break;
					}
				default:
					{
						page = dbcontext.CMSPages.Where(p => p.Shortcut == _path && p.isOn == true).FirstOrDefault();
						if (page == null)
						{
							_log.Warning("Page not found: {Path} on {Domain}", _path, _domain);
							return Common.Send404(context);
						}
						if (page.isPrivate == true && authUser == null)
						{
							_log.Information("Anonymous user redirected to login for private page {Path} on {Domain}", _path, _domain);
							return Common.GetLoginPage(context, _path, "Please log in to view content.");
						}
						shouldCache = requestMightBeCacheable && !page.isPrivate;
						break;
					}
			}
			//var items = dbcontext.cmsItems.Where(i => i.cmsPageId == page.cmsPageId).ToList<cmsItem>();

			if (isHtmx)
			{
				if (searchresults != null)
				{
					StringBuilder rs = new StringBuilder();
					rs.Append($"<section>");
					rs.Append($"<h1>Search Results</h1>");
					foreach (CMSPage result in searchresults)
					{
						string truncatedsummary = result.Summary.Length > 250 ? result.Summary.Substring(0, 250) : result.Summary;
						rs.Append($"<div class=\"searchresult\">");
						rs.Append($"<h3><a href=\"/{result.Shortcut}\">{result.Title}</a></h3>");
						rs.Append($"<div class=\"summary\">{truncatedsummary}</div>");
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

					// get blocks for this page
					string blocksHtml = GetPageBlocks(dbcontext, page.PageId);
					sb.Append(blocksHtml);

					html = sb.ToString();
				}
			}
			else // not HTMX
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
				html = html.Replace("{{subtitle}}", site.SubTitle.Length > 0 ? $"<div id=\"sitesubtitle\">{site.SubTitle}</div>" : "");
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

				// get blocks for this page
				string blocksHtml = GetPageBlocks(dbcontext, page.PageId);
				html = html.Replace("{{blocks}}", blocksHtml);

				//get navigation
				List<CMSPage> nav = dbcontext.CMSPages.Where(n => n.SiteId == site.SiteId)
					.OrderBy(o => o.ParentId).ThenBy(x => x.Sort).ToList();
				html = html.Replace("{{navigation}}", GetNav(nav, null, page.PageId));
			}
			if (shouldCache)
			{
				cache.Set(cacheKey, html, new MemoryCacheEntryOptions()
					.AddExpirationToken(CMSCache.GetSiteToken(_domain))
					.SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
				_log.Information("Cached page {Path} on {Domain} with key {CacheKey}", _path, _domain, cacheKey);
			}
			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;
			return html;
		}

		private string GetNav(List<CMSPage> nav, Guid? pId, Guid currentId)
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

		private bool HasChildren(List<CMSPage> nav, Guid pId)
		{
			IEnumerable<CMSPage> parent = nav.Where(y => y.ParentId == pId && y.isOn == true && y.isHidden == false);
			if (parent.Count() > 0)
				return true;
			else
				return false;
		}

		private string GetPageBlocks(data.CMSDbContext dbcontext, Guid pageId)
		{
			// Get all page blocks sorted by Sort order
			var pageBlocks = dbcontext.CMSPageBlocks
				.Where(pb => pb.PageId == pageId)
				.OrderBy(pb => pb.Sort)
				.ToList();

			if (pageBlocks.Count == 0)
				return string.Empty;

			StringBuilder blocksHtml = new StringBuilder();
			foreach (var pageBlock in pageBlocks)
			{
				var block = dbcontext.CMSBlocks
					.Where(b => b.BlockId == pageBlock.BlockId)
					.FirstOrDefault();

				if (block != null)
				{
					blocksHtml.Append($"<section><div class=\"block\" data-block-id=\"{block.BlockId}\">");
					blocksHtml.Append($"<fieldset><legend>{block.Title1}</legend>");
					if (!string.IsNullOrEmpty(block.Title2))
						blocksHtml.Append($"<h3>{block.Title2}</h3>");
					if (!string.IsNullOrEmpty(block.Data))
						blocksHtml.Append($"<div class=\"block-content\">{block.Data}</div>");

					// Get top 5 items in reverse chronological order
					var items = dbcontext.CMSItems
						.Where(i => i.BlockId == block.BlockId)
						.OrderByDescending(i => i.Start)
						.Take(5)
						.ToList();

					if (items.Count > 0)
					{
						blocksHtml.Append("<div class=\"block-items\">");
						foreach (var item in items)
						{
							if (item.Status == "published")
							{
								blocksHtml.Append($"<div class=\"item\">");
								blocksHtml.Append($"<h4>{item.Title1}</h4>");
								if (!string.IsNullOrEmpty(item.Title2))
									blocksHtml.Append($"<p class=\"item-subtitle\">{item.Title2}</p>");
								if (item.Start.HasValue)
									blocksHtml.Append($"<p class=\"item-date\">{item.Start:MMM dd, yyyy}</p>");
								if (!string.IsNullOrEmpty(item.Data))
									blocksHtml.Append($"<div class=\"item-content\">{item.Data}</div>");
								blocksHtml.Append($"</div>");
							}
						}
						blocksHtml.Append("</div>");
					}

					blocksHtml.Append($"</fieldset></div></section>");
				}
			}

			return blocksHtml.ToString();
		}
	}
}