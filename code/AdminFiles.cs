namespace dabrelCMS.code
{
	public static class AdminFiles
	{
		public static string GetFilesForm()
		{
			string filesformPath = $"{Common.WebRootPath}\\designs\\admin\\dialogfiles.htm";
			string html = Common.GetFileText(filesformPath);
			html = html.Replace("{{currentfolder}}", ".");
			return html;
		}



	}
}
