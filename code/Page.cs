using Microsoft.EntityFrameworkCore;
using System.Text;

namespace dabrelCMS.code
{
	public class Page
	{
		internal string GetPage(data.CMSDbContext dbcontext, HttpContext context, string html)
		{
			html = html.Replace("{{pagetitle}}", GetPageTitle());

			return html;
		}

		internal string GetPageTitle()
		{
			return string.Empty;
		}

		internal string GetNav()
		{ return string.Empty; }

		internal string GetPageContent()
		{ return string.Empty; }

		internal string GetFooter()
		{ return string.Empty; }
	}
}