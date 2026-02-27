using Microsoft.EntityFrameworkCore;
using System.Text;

namespace dabrelCMS.code
{
	public static class AdminItem
	{
		public static string GetItemPageForm(int blockId, CMSDbContext dbContext)
		{
			string itemformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogadditem.htm";
			string html = Common.GetFileText(itemformPath);
			html = html.Replace("{{blockid}}", blockId.ToString());
			return html;
		}

		public static string GetItemForm(int itemId, CMSDbContext dbContext)
		{
			string itemformPath = $"{Common.WebRootPath}\\designs\\{Common.AdminDesign}\\dialogedititem.htm";
			string html = Common.GetFileText(itemformPath);
			CMSItem item = dbContext.CMSItems.Where(i => i.ItemId == itemId).FirstOrDefault();
			if (item != null)
			{
				html = html.Replace("{{itemid}}", item.ItemId.ToString());
				html = html.Replace("{{blockid}}", item.BlockId.ToString());
				html = html.Replace("{{title1}}", item.Title1 ?? "");
				html = html.Replace("{{title2}}", item.Title2 ?? "");
				html = html.Replace("{{data}}", item.Data ?? "");
				html = html.Replace("{{tags}}", item.Tags ?? "");
				if (item.Status == "published")
					html = html.Replace("{{statuspublished}}", "selected" ?? "");
				else
					html = html.Replace("{{statusdraft}}", "selected" ?? "");
				html = html.Replace("{{sort}}", item.Sort.ToString());
			}
			return html;
		}

		public static string AddNewItem(HttpContext context, CMSDbContext dbContext)
		{
			int blockId = int.Parse(context.Request.Form["blockid"].ToString());
			
			// Get the highest sort order for this block
			int maxSort = dbContext.CMSItems.Where(i => i.BlockId == blockId).Max(i => (int?)i.Sort) ?? 0;

			CMSItem item = new()
			{
				BlockId = blockId,
				Title1 = context.Request.Form["title1"].ToString(),
				Title2 = context.Request.Form["title2"].ToString(),
				Data = context.Request.Form["data"].ToString(),
				Tags = context.Request.Form["tags"].ToString(),
				Status = context.Request.Form["status"].ToString() ?? "draft",
				Sort = maxSort + 1,
				ItemType = context.Request.Form["itemtype"].ToString() ?? "entry",
				Start = DateTime.UtcNow,
				Shortcut = context.Request.Form["title1"].ToString().ToLower().Replace(" ", "-")
			};

			dbContext.CMSItems.Add(item);
			dbContext.SaveChanges();
			return "success";
		}

		public static string SaveItem(HttpContext context, CMSDbContext dbContext)
		{
			int itemId = int.Parse(context.Request.Form["itemid"].ToString());
			CMSItem item = dbContext.CMSItems.Where(i => i.ItemId == itemId).FirstOrDefault();

			if (item != null)
			{
				item.Title1 = context.Request.Form["title1"].ToString();
				item.Title2 = context.Request.Form["title2"].ToString();
				item.Data = context.Request.Form["data"].ToString();
				item.Tags = context.Request.Form["tags"].ToString();
				item.Status = context.Request.Form["status"].ToString();
				item.Sort = int.Parse(context.Request.Form["sort"].ToString());
				dbContext.SaveChanges();
				return "success";
			}
			return "error: item not found";
		}

		public static string DeleteItem(int itemId, CMSDbContext dbContext)
		{
			CMSItem item = dbContext.CMSItems.Where(i => i.ItemId == itemId).FirstOrDefault();
			if (item != null)
			{
				dbContext.CMSItems.Remove(item);
				dbContext.SaveChanges();
				return "success";
			}
			return "error: item not found";
		}

		public static string GetBlockItems(int blockId, CMSDbContext dbContext)
		{
			StringBuilder html = new StringBuilder();
			List<CMSItem> items = dbContext.CMSItems.Where(i => i.BlockId == blockId).OrderBy(i => i.Sort).ToList();

			foreach (CMSItem item in items)
			{
				html.Append("<div class=\"blockitem\">");
				html.Append($"<div class=\"itemheader\">");
				html.Append($"<span class=\"itemtitle\">{item.Title1}</span>");
				html.Append($"<span class=\"itemstatus\">{item.Status}</span>");
				html.Append($"<div class=\"buttonright\">");
				html.Append($"<button hx-get=\"/admin/getedititemform/{item.ItemId}\" hx-target=\"#cmsdialogform\" onclick=\"document.getElementById('cmsdialog').showModal();\"><i class=\"fa-solid fa-pen-to-square\"></i></button>");
				html.Append($"<button hx-post=\"/admin/deleteitem/{item.ItemId}\" hx-confirm=\"Delete this entry?\" hx-target=\"#pagecontent\"><i class=\"fa-solid fa-trash\"></i></button>");
				html.Append($"</div>");
				html.Append($"</div>");
				if (!string.IsNullOrEmpty(item.Title2))
					html.Append($"<div class=\"itemsubtitle\">{item.Title2}</div>");
				html.Append($"<div class=\"itemcontent\">{item.Data}</div>");
				html.Append("</div>");
			}

			return html.ToString();
		}
	}
}