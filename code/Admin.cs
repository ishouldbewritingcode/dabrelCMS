using dabrelCMS.models;
using Htmx;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace dabrelCMS.code
{
	public class Admin
	{
		private StringBuilder sbUploadsFolder = new StringBuilder();

		public string GetPage(HttpContext context, CMSUser authUser)
		{
			string _domain = string.Empty;
			string _path = string.Empty;
			string _webRootPath = string.Empty;
			string html = string.Empty;

			_webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			Common.WebRootPath = _webRootPath;
			string design = "admin";
			Common.AdminDesign = design;
			_domain = context.Request.Host.ToString().ToLower().Trim();
			// strip off any port numbers that may be involved
			int colonIndex = _domain.IndexOf(":");
			if (colonIndex > 0)
				_domain = _domain.Substring(0, colonIndex);

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
						case "addblockform":
							string z = AdminBlock.AddNewBlock(context, dbcontext);
							if (page != null)
								context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";

							break;
						case "savesiteform":
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
							break;

						case "getpageform":
							page = dbcontext.CMSPages.Where(p => p.PageId == Guid.Parse(pathsegments[2])).FirstOrDefault();
							html = AdminPage.GetPageForm(page);
							break;

						case "getaddpageform":
							string addpageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\addpageform.htm";
							html = Common.GetFileText(addpageformPath);
							if (pathsegments.Length > 2)
								if (pathsegments[2] != string.Empty)
								{
									Guid p = Guid.Empty;
									Guid.TryParse(pathsegments[2], out p);
									html = html.Replace("{{parentid}}", p.ToString());
								}
							break;

						case "addpageform":
							page = new CMSPage();
							page.PageId = Guid.CreateVersion7();
							dbcontext.CMSPages.Add(page);
							page.Title = context.Request.Form["pagetitle"].ToString();
							page.SiteId = site.SiteId;
							page.NavTitle = page.Title;
							page.Shortcut = HttpUtility.UrlEncode(page.Title.ToLower().Replace(" ", String.Empty));
							string addParentStr = context.Request.Form["parentid"].ToString().Trim();
							page.ParentId = Guid.TryParse(addParentStr, out Guid addParentId) ? addParentId : null;
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
							Guid id = Guid.Empty;
							string sid = context.Request.Form["pageid"].ToString().Trim();
							bool isEdit = Guid.TryParse(sid, out id);
							if (isEdit)
								page = dbcontext.CMSPages.Where(p => p.PageId == id).FirstOrDefault();
							else
							{
								page = new CMSPage();
								page.PageId = Guid.CreateVersion7();
								dbcontext.CMSPages.Add(page);
							}
							page.Title = context.Request.Form["pagetitle"].ToString();
							page.NavTitle = context.Request.Form["navtitle"].ToString();
							page.Shortcut = context.Request.Form["pageshortcut"].ToString();
							string pageformParentStr = context.Request.Form["parentid"].ToString().Trim();
							page.ParentId = Guid.TryParse(pageformParentStr, out Guid pageformParentId) ? pageformParentId : null;
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

						case "pagecontentform":
							// save page content here
							id = Guid.Empty;
							sid = context.Request.Form["pageid"].ToString().Trim();
							isEdit = Guid.TryParse(sid, out id);
							if (isEdit)
								page = dbcontext.CMSPages.Where(p => p.PageId == id).FirstOrDefault();
							else
							{
								page = new CMSPage();
								page.PageId = Guid.CreateVersion7();
								dbcontext.CMSPages.Add(page);
							}
							page.Title = context.Request.Form["pagetitle"].ToString();
							page.Summary = context.Request.Form["content"].ToString();
							dbcontext.SaveChanges();
							context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
							break;

						case "pageconfigform":
							// save page config here
							id = Guid.Empty;
							sid = context.Request.Form["pageid"].ToString().Trim();
							isEdit = Guid.TryParse(sid, out id);
							if (isEdit)
								page = dbcontext.CMSPages.Where(p => p.PageId == id).FirstOrDefault();
							else
							{
								page = new CMSPage();
								page.PageId = Guid.CreateVersion7();
								dbcontext.CMSPages.Add(page);
							}
							page.Title = context.Request.Form["pagetitle"].ToString();
							page.NavTitle = context.Request.Form["navtitle"].ToString();
							page.Shortcut = context.Request.Form["pageshortcut"].ToString();
							string configParentStr = context.Request.Form["parentid"].ToString().Trim();
							page.ParentId = Guid.TryParse(configParentStr, out Guid configParentId) ? configParentId : null;
							page.Sort = int.Parse(context.Request.Form["pagesort"].ToString());
							page.isOn = (context.Request.Form["pageison"].ToString().Length > 0 ? true : false);
							page.isPrivate = (context.Request.Form["pageisprivate"].ToString().Length > 0 ? true : false);
							page.isHidden = (context.Request.Form["pageishidden"].ToString().Length > 0 ? true : false);
							page.Tags = context.Request.Form["pagetags"].ToString();
							page.HeroImage = context.Request.Form["pagehero"].ToString();
							dbcontext.SaveChanges();
							context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
							break;

						case "deletepage":
							if (pathsegments.Length > 2)
							{
								if (pathsegments[2] != string.Empty)
								{
									Guid delId = Guid.Empty;
									Guid.TryParse(pathsegments[2], out delId);
									page = dbcontext.CMSPages.Where(p => p.PageId == delId).FirstOrDefault();
									// don't let them delete the home page.
									if (page != null && page.Shortcut != string.Empty)
									{
										Guid? redirectid = page.ParentId;
										CMSPage redirect;
										if (redirectid == null)
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

						case "saveuserform":
							return AdminUser.SaveUserForm(dbcontext, context, authUser.UserId);
							// save user here - this is only for changing the currently logged in user.
							//CMSUser tempUser = dbcontext.CMSUsers.Where(u => u.UserId == authUser.UserId).FirstOrDefault();
							//tempUser.Email = context.Request.Form["email"].ToString();
							//tempUser.FirstName = context.Request.Form["firstname"].ToString();
							//tempUser.LastName = context.Request.Form["lastname"].ToString();
							//tempUser.Mobile = context.Request.Form["mobile"].ToString();

							//if (context.Request.Form["password"].ToString().Length > 0)
							//{
							//	if (context.Request.Form["password"].ToString() == context.Request.Form["vpassword"].ToString())
							//	{
							//		int salt = GetRandomSalt();
							//		tempUser.Salt = salt;
							//		tempUser.Password = PWHash.ComputePasswordHash(context.Request.Form["password"].ToString(), salt);
							//	}
							//}

							//dbcontext.SaveChanges();
							//context.Response.Headers["Content-Type"] = "text/html";
							//context.Response.StatusCode = StatusCodes.Status200OK;
							//return "<div>User saved successfully.</div>";
							//context.Response.Headers["HX-Redirect"] = $"/admin";
							break;

						case "additemform":
							AdminItem.AddNewItem(context, dbcontext);
							Guid blockIdForRedirect = Guid.Parse(context.Request.Form["blockid"].ToString());
							CMSBlock blockForRedirect = dbcontext.CMSBlocks.FirstOrDefault(b => b.BlockId == blockIdForRedirect);
							if (blockForRedirect != null)
							{
								CMSPageBlock pageBlockForRedirect = dbcontext.CMSPageBlocks.FirstOrDefault(pb => pb.BlockId == blockIdForRedirect);
								if (pageBlockForRedirect != null)
								{
									page = dbcontext.CMSPages.FirstOrDefault(p => p.PageId == pageBlockForRedirect.PageId);
									if (page != null)
										context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
								}
							}
							return "Item added";
							break;

						case "saveitemform":
							AdminItem.SaveItem(context, dbcontext);
							Guid blockIdForSave = Guid.Parse(context.Request.Form["blockid"].ToString());
							CMSBlock blockForSave = dbcontext.CMSBlocks.FirstOrDefault(b => b.BlockId == blockIdForSave);
							if (blockForSave != null)
							{
								CMSPageBlock pageBlockForSave = dbcontext.CMSPageBlocks.FirstOrDefault(pb => pb.BlockId == blockIdForSave);
								if (pageBlockForSave != null)
								{
									page = dbcontext.CMSPages.FirstOrDefault(p => p.PageId == pageBlockForSave.PageId);
									if (page != null)
										context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
								}
							}
							return "Item saved";
							break;

						case "deleteitem":
							if (pathsegments.Length > 2)
							{
								Guid itemId = Guid.Empty;
								Guid.TryParse(pathsegments[2], out itemId);
								CMSItem itemToDelete = dbcontext.CMSItems.FirstOrDefault(i => i.ItemId == itemId);
								if (itemToDelete != null)
								{
									Guid blockIdForDelete = itemToDelete.BlockId;
									AdminItem.DeleteItem(itemId, dbcontext);
									CMSBlock blockForDelete = dbcontext.CMSBlocks.FirstOrDefault(b => b.BlockId == blockIdForDelete);
									if (blockForDelete != null)
									{
										CMSPageBlock pageBlockForDelete = dbcontext.CMSPageBlocks.FirstOrDefault(pb => pb.BlockId == blockIdForDelete);
										if (pageBlockForDelete != null)
										{
											page = dbcontext.CMSPages.FirstOrDefault(p => p.PageId == pageBlockForDelete.PageId);
											if (page != null)
												context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";
										}
									}
								}
							}
							break;

						case "addfolder":
							if (context.Request.Form["siteId"].ToString().Length > 0)
							{
								if (context.Request.Form["newfolder"].ToString().Length > 0)
								{
									if (context.Request.Form["currentdirectory"].ToString().Length > 0)
									{
										string savePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
										savePath = Path.Combine(savePath, site.SiteId.ToString());
										string newPath = Path.Combine(savePath, context.Request.Form["currentdirectory"].ToString());
										newPath = Path.Combine(newPath, context.Request.Form["newfolder"].ToString());
										if (!Directory.Exists(newPath))
											Directory.CreateDirectory(newPath);
									}
								}
							}
							context.Response.Headers["Content-Type"] = "text/html";
							return "<div>New folder saved successfully.</div>";
							context.Response.StatusCode = StatusCodes.Status200OK;
							break;

						case "deletefile":
							context.Response.Headers["Content-Type"] = "text/html";
							context.Response.StatusCode = StatusCodes.Status200OK;
							return HandleDeleteFile(context, site);

						case "deletedirectory":
							context.Response.Headers["Content-Type"] = "text/html";
							context.Response.StatusCode = StatusCodes.Status200OK;
							return HandleDeleteDirectory(context, site);
					}
				}
				else
				{
					// get request
					switch (pathsegments[1])
					{
						case "getsiteform":
							html = AdminSite.GetSiteForm(site);
							break;

						case "getuserform":
							html = AdminUser.GetUserForm(authUser);
							break;

						case "getfilesform":
							html = AdminFiles.GetFilesForm();
							break;

						case "getuploads":
							string savePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
							savePath = Path.Combine(savePath, site.SiteId.ToString());

							// Get directory parameter from query string
							string? dirParam = context.Request.Query["dir"];
							if (!string.IsNullOrEmpty(dirParam))
							{
								savePath = Path.Combine(savePath, dirParam);
							}

							if (!Directory.Exists(savePath))
								Directory.CreateDirectory(savePath);
							DirectoryInfo directoryInfo = new DirectoryInfo(savePath);
							sbUploadsFolder.Clear();
							TraverseDirectory(directoryInfo);
							html = sbUploadsFolder.ToString();
							break;

						case "getpageconfig":
							page = dbcontext.CMSPages.Where(p => p.PageId == Guid.Parse(pathsegments[2])).FirstOrDefault();
							html = AdminPage.GetPageConfig(page);
							break;

						case "getaddblockform":
							page = dbcontext.CMSPages.Where(p => p.PageId == Guid.Parse(pathsegments[2])).FirstOrDefault();
							html = AdminBlock.GetAddBlockForm(page, dbcontext);
							break;

						case "getitempageform":
							if (pathsegments.Length > 2)
							{
								Guid blockId = Guid.Empty;
								Guid.TryParse(pathsegments[2], out blockId);
								html = AdminItem.GetItemPageForm(blockId, dbcontext);
							}
							break;

						case "getedititemform":
							if (pathsegments.Length > 2)
							{
								Guid itemId = Guid.Empty;
								Guid.TryParse(pathsegments[2], out itemId);
								html = AdminItem.GetItemForm(itemId, dbcontext);
							}
							break;

						default:
							string contentformPath = $"{_webRootPath}\\designs\\{design}\\contentform.htm";
							html = Common.GetFileText(contentformPath);
							html = html.Replace("{{pageid}}", page.PageId.ToString());
							html = html.Replace("{{pagetitle}}", page.Title);
							html = html.Replace("{{content}}", page.Summary);
							html = html.Replace("{{blocks}}", AdminBlock.GetPageBlocks(page, dbcontext));
							break;
					}
					context.Response.Headers["Content-Type"] = "text/html";
					context.Response.StatusCode = StatusCodes.Status200OK;
					return html;
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
				html = html.Replace("{{blocks}}", AdminBlock.GetPageBlocks(page, dbcontext));
				html = html.Replace("{{bodybottom}}", site.BodyBottom);
				html = html.Replace("{{footer1}}", site.Footer1.Length > 0 ? $"<div>{site.Footer1}</div>" : "");
				html = html.Replace("{{footer2}}", site.Footer2.Length > 0 ? $"<div>{site.Footer2}</div>" : "");
				html = html.Replace("{{footer3}}", site.Footer3.Length > 0 ? $"<div>{site.Footer3}</div>" : "");
				html = html.Replace("{{footer4}}", site.Footer4.Length > 0 ? $"<div>{site.Footer4}</div>" : "");

				//get navigation
				List<CMSPage> nav = dbcontext.CMSPages.Where(n => n.SiteId == site.SiteId)
					.OrderBy(o => o.ParentId).ThenBy(x => x.Sort).ToList();
				html = html.Replace("{{navigation}}", GetNav(nav, null, page.PageId));
			}
			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;

			// cleanup any curlies that might be left.
			Regex r = new Regex(@"\{\{.*\}\}");
			html = r.Replace(html, String.Empty);
			return html;
		}

		private string GetNav(List<CMSPage> nav, Guid? pId, Guid currentId)
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
					c.Append($"<button class=\"sub\"><i class=\"fa-solid fa-plus\"></i></button>");
				}
				c.Append(GetNav(nav, page.PageId, currentId));
				c.Append($"</li>");
			}
			c.Append("</ul>");
			return c.ToString();
		}

		private bool HasChildren(List<CMSPage> nav, Guid pId)
		{
			IEnumerable<CMSPage> parent = nav.Where(y => y.ParentId == pId);
			if (parent.Count() > 0)
				return true;
			else
				return false;
		}

		private void TraverseDirectory(DirectoryInfo directoryInfo)
		{
			IEnumerable<DirectoryInfo> subdirectories = directoryInfo.EnumerateDirectories();
			sbUploadsFolder.Append("<ul class=\"folders\">");
			foreach (DirectoryInfo subdirectory in subdirectories)
			{
				sbUploadsFolder.Append("<li class=\"folder\">");
				sbUploadsFolder.Append("<i class=\"fa-regular fa-folder\"></i>");
				sbUploadsFolder.Append($"<div><button title=\"{subdirectory.Name}\" onclick=\"navigateToDirectory('{GetRelativePath(directoryInfo, subdirectory)}'); return false;\">{subdirectory.Name}</button></div>");
				sbUploadsFolder.Append($"<button type=\"button\" class=\"deletebutton\" onclick=\"deleteDirectory('{subdirectory.Name}'); return false;\" title=\"Delete folder\"><i class=\"fa-solid fa-trash\"></i></button>");
				sbUploadsFolder.Append("</li>");
				TraverseDirectory(subdirectory);
			}
			sbUploadsFolder.Append("</ul>");

			IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles();
			sbUploadsFolder.Append("<ul class=\"files\">");
			foreach (FileInfo file in files)
			{
				sbUploadsFolder.Append("<li class=\"file\">");
				sbUploadsFolder.Append(Common.GetFAIcon(file.Name));
				sbUploadsFolder.Append($"<div>{file.Name}</div>");
				sbUploadsFolder.Append($"<button type=\"button\" class=\"deletebutton\" onclick=\"deleteFile('{file.Name}'); return false;\" title=\"Delete file\"><i class=\"fa-solid fa-trash\"></i></button>");
				sbUploadsFolder.Append("</li>");
			}
			sbUploadsFolder.Append("</ul>");
		}

		private string GetRelativePath(DirectoryInfo baseDir, DirectoryInfo targetDir)
		{
			string basePath = baseDir.FullName;
			string targetPath = targetDir.FullName;
			if (targetPath.StartsWith(basePath))
			{
				string relative = targetPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
				return relative.Replace(Path.DirectorySeparatorChar, '/');
			}
			return targetDir.Name;
		}

		private string HandleDeleteFile(HttpContext context, CMSSite site)
		{
			string basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
			basePath = Path.Combine(basePath, site.SiteId.ToString());

			string? filename = context.Request.Form["filename"];
			string? directory = context.Request.Form["directory"];

			if (string.IsNullOrEmpty(filename))
				return "<div class=\"error\">No filename provided.</div>";

			try
			{
				string filePath = basePath;
				if (!string.IsNullOrEmpty(directory))
					filePath = Path.Combine(filePath, directory);
				filePath = Path.Combine(filePath, filename);

				// Security check: ensure file is within the upload directory
				string fullBase = Path.GetFullPath(basePath);
				string fullFile = Path.GetFullPath(filePath);
				if (!fullFile.StartsWith(fullBase))
					return "<div class=\"error\">Invalid file path.</div>";

				if (File.Exists(filePath))
				{
					File.Delete(filePath);
					// Refresh directory listing
					string uploadDir = basePath;
					if (!string.IsNullOrEmpty(directory))
						uploadDir = Path.Combine(uploadDir, directory);
					DirectoryInfo dirInfo = new DirectoryInfo(uploadDir);
					sbUploadsFolder.Clear();
					TraverseDirectory(dirInfo);
					return sbUploadsFolder.ToString();
				}
				return "<div class=\"error\">File not found.</div>";
			}
			catch (Exception ex)
			{
				return $"<div class=\"error\">Error deleting file: {ex.Message}</div>";
			}
		}

		private string HandleDeleteDirectory(HttpContext context, CMSSite site)
		{
			string basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
			basePath = Path.Combine(basePath, site.SiteId.ToString());

			string? dirname = context.Request.Form["dirname"];
			string? directory = context.Request.Form["directory"];

			if (string.IsNullOrEmpty(dirname))
				return "<div class=\"error\">No directory name provided.</div>";

			try
			{
				string dirPath = basePath;
				if (!string.IsNullOrEmpty(directory))
					dirPath = Path.Combine(dirPath, directory);
				dirPath = Path.Combine(dirPath, dirname);

				// Security check: ensure directory is within the upload directory
				string fullBase = Path.GetFullPath(basePath);
				string fullDir = Path.GetFullPath(dirPath);
				if (!fullDir.StartsWith(fullBase))
					return "<div class=\"error\">Invalid directory path.</div>";

				if (Directory.Exists(dirPath))
				{
					Directory.Delete(dirPath, true); // true = recursive
					// Refresh directory listing
					string uploadDir = basePath;
					if (!string.IsNullOrEmpty(directory))
						uploadDir = Path.Combine(uploadDir, directory);
					DirectoryInfo dirInfo = new DirectoryInfo(uploadDir);
					sbUploadsFolder.Clear();
					TraverseDirectory(dirInfo);
					return sbUploadsFolder.ToString();
				}
				return "<div class=\"error\">Directory not found.</div>";
			}
			catch (Exception ex)
			{
				return $"<div class=\"error\">Error deleting directory: {ex.Message}</div>";
			}
		}
	}
}