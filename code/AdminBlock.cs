using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace dabrelCMS.code
{
	public static class AdminBlock
	{
		public static string GetPageBlocks(CMSPage page, CMSDbContext dbContext)
		{
			string html = String.Empty;
			if (page != null)
			{
				string templatePath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\pageblockadmin.htm";
				string pageblockhtml = Common.GetFileText(templatePath);
				List<CMSPageBlock> pageBlocks = dbContext.CMSPageBlocks.Where(p => p.PageId == page.PageId).OrderBy(p => p.Sort).ToList();
				foreach (CMSPageBlock pblock in pageBlocks)
				{
					CMSBlock? block = dbContext.CMSBlocks.FirstOrDefault(p => p.BlockId == pblock.BlockId);

					if (block != null)
					{
						string blockHtml = pageblockhtml;
						blockHtml = blockHtml.Replace("{{blockid}}", block.BlockId.ToString());

						if (pblock.AltTitle != null)
							blockHtml = blockHtml.Replace("{{title}}", pblock.AltTitle);
						else
							blockHtml = blockHtml.Replace("{{title}}", block.Title1);

						if (pblock.AltSubtitle != null)
							blockHtml = blockHtml.Replace("{{title2}}", pblock.AltSubtitle);
						else
							blockHtml = blockHtml.Replace("{{title2}}", block.Title2);

						// Render block items based on block type
						string itemsHtml = RenderBlockItems(block, dbContext);
						blockHtml = blockHtml.Replace("{{items}}", itemsHtml);

						html += blockHtml;
					}
				}
			}
			return html;
		}

		private static string RenderBlockItems(CMSBlock block, CMSDbContext dbContext)
		{
			if (block.BlockType == "entries" || block.BlockType == "blog")
			{
				return AdminItem.GetBlockItems(block.BlockId, dbContext);
			}
			// Add other block type renderers here
			return string.Empty;
		}

		public static string GetOtherBlocks(Guid siteId, CMSDbContext dbContext)
		{
			string html = String.Empty;
			if (siteId != Guid.Empty)
			{
				// get all blocks for the site from the database
				List<CMSBlock> blocks = dbContext.CMSBlocks.Where(p => p.SiteId == siteId).ToList();
				foreach (CMSBlock block in blocks)
				{
					html += $"<option value=\"{block.BlockId}\">{block.Title1}</option>";
				}
			}
			return html;
		}

		public static string GetAddBlockForm(CMSPage page, CMSDbContext dbContext)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogaddblock.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{pageid}}", page.PageId.ToString());
			html = html.Replace("{{previousblocks}}", GetOtherBlocks(page.SiteId, dbContext));
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

		public static string AddNewBlock(HttpContext context, CMSDbContext dbContext)
		{
			// build new block here
			Guid id = Guid.Empty;
			string sid = context.Request.Form["pageid"].ToString().Trim();
			bool addToPage = Guid.TryParse(sid, out id);
			CMSBlock block = new()
			{
				BlockId = Guid.CreateVersion7(),
				BlockType = context.Request.Form["blocktype"].ToString(),
				Title1 = context.Request.Form["title"].ToString()
			};
			dbContext.CMSBlocks.Add(block);
			dbContext.SaveChanges();
			Guid blockid = block.BlockId;

			if (addToPage)
			{
				CMSPageBlock pageBlock = new CMSPageBlock();
				pageBlock.PageBlockID = Guid.CreateVersion7();
				pageBlock.BlockId = blockid;
				pageBlock.PageId = id;
				dbContext.CMSPageBlocks.Add(pageBlock);
				dbContext.SaveChanges();
			}
			CMSPage? page = dbContext.CMSPages.FirstOrDefault(p => p.PageId == id);

			if (page != null)
				context.Response.Headers["HX-Redirect"] = $"/admin/{page.Shortcut}";

			return "success";
		}
	}
}
