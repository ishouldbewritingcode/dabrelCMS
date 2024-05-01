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

	}
}
