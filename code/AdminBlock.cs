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
				List<CMSPageBlock> pageBlocks = dbContext.CMSPageBlocks.Where(p => p.PageId == page.PageId).ToList();
				foreach (CMSPageBlock pblock in pageBlocks)
				{
					CMSBlock block = dbContext.CMSBlocks.FirstOrDefault(p => p.BlockId == pblock.BlockId);
					if (block != null)
					{
						html += pageblockhtml;
						if (pblock.AltTitle != null)
							html = html.Replace("{{title}}", pblock.AltTitle);
						else
							html = html.Replace("{{title}}", block.Title1);
						if (pblock.AltSubtitle != null)
							html = html.Replace("{{title2}}", pblock.AltSubtitle);
						else
							html = html.Replace("{{title2}}", block.Title2);

					}
				}

			}
			return html;
		}

		public static string GetOtherBlocks(int siteId, CMSDbContext dbContext)
		{
			string html = String.Empty;
			if (siteId > 0)
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
			int id = 0;
			string sid = context.Request.Form["pageid"].ToString().Trim();
			bool addToPage = int.TryParse(sid, out id);
			CMSBlock block = new()
			{
				BlockType = context.Request.Form["blocktype"].ToString(),
				Title1 = context.Request.Form["Title"].ToString()
			};
			dbContext.CMSBlocks.Add(block);
			dbContext.SaveChanges();
			int blockid = block.BlockId;
			
			if (addToPage)
			{
				CMSPageBlock pageBlock = new CMSPageBlock();
				pageBlock.BlockId = blockid;
				pageBlock.PageId = id;
				dbContext.CMSPageBlocks.Add(pageBlock);
				dbContext.SaveChanges();
			}
			return "success";
		}
	}
}
