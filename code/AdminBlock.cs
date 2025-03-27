using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace dabrelCMS.code
{
	public static class AdminBlock
	{
		public static string GetAddBlockForm(CMSPage page)
		{
			string pageformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogaddblock.htm";
			string html = Common.GetFileText(pageformPath);
			html = html.Replace("{{pageid}}", page.PageId.ToString());
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

		public static string AddNewBlock(HttpContext context, CMSDbContext dbContext, CMSPage page)
		{
			// build new block here
			int id = 0;
			string sid = context.Request.Form["pageid"].ToString().Trim();
			bool addToPage = int.TryParse(sid, out id);
			CMSBlock block = new CMSBlock();

			block.BlockType = context.Request.Form["blocktype"].ToString();
			block.Title1 = context.Request.Form["Title"].ToString();
			dbContext.SaveChanges();
			int blockid = block.BlockId;
			
			if (addToPage)
			{
				CMSPageBlock pageBlock = new CMSPageBlock();
				pageBlock.BlockId = blockid;
				pageBlock.PageId = id;
				dbContext.SaveChanges();
			}
			return "success";
		}
	}
}
