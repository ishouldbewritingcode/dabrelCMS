using Microsoft.EntityFrameworkCore;

namespace dabrelCMS.code
{
	public static class AdminUser
	{
		public static string GetUserForm(CMSUser authUser)
		{
			string userformPath = $"{Common.WebRootPath}\\designs\\admin\\dialoguser.htm";
			string html = Common.GetFileText(userformPath);
			html = html.Replace("{{userid}}", authUser.UserId.ToString());
			html = html.Replace("{{siteid}}", authUser.SiteId.ToString());
			html = html.Replace("{{email}}", authUser.Email.ToString());
			html = html.Replace("{{firstname}}", authUser.FirstName.ToString());
			html = html.Replace("{{lastname}}", authUser.LastName.ToString());
			html = html.Replace("{{mobile}}", authUser.Mobile.ToString());
			html = html.Replace("{{roles}}", authUser.Roles.ToString());
			return html;
		}

		public static string SaveUserForm(CMSDbContext dbcontext, HttpContext context, Guid userid)
		{
			CMSUser tempUser = dbcontext.CMSUsers.Where(u => u.UserId == userid).FirstOrDefault();
			tempUser.Email = context.Request.Form["email"].ToString();
			tempUser.FirstName = context.Request.Form["firstname"].ToString();
			tempUser.LastName = context.Request.Form["lastname"].ToString();
			tempUser.Mobile = context.Request.Form["mobile"].ToString();

			if (context.Request.Form["password"].ToString().Length > 0)
			{
				if (context.Request.Form["password"].ToString() == context.Request.Form["vpassword"].ToString())
				{
					int salt = GetRandomSalt();
					tempUser.Salt = salt;
					tempUser.Password = PWHash.ComputePasswordHash(context.Request.Form["password"].ToString(), salt);
				}
			}

			dbcontext.SaveChanges();
			context.Response.Headers["Content-Type"] = "text/html";
			context.Response.StatusCode = StatusCodes.Status200OK;
			return "<div>User saved successfully.</div>";
		}

		private static int GetRandomSalt()
		{
			Random rand = new Random();
			return rand.Next(10, 99999);
		}


	}
}
