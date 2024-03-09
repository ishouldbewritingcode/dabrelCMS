using System.IO;
using System.Net;

namespace dabrelCMS.code
{
	public class Uploader
	{
		public async Task<bool> UploadFiles(HttpContext context)
		{
			await Task.Yield();

			string path = context.Request.Path.ToString().ToLower().Trim().Replace("/", string.Empty);
			if (path == "adminupload")
			{
				using CMSDbContext dbcontext = new CMSDbContext();
				string domain = context.Request.Host.ToString().ToLower().Trim();
				// strip off any port numbers that may be involved
				domain = domain.Substring(0, domain.IndexOf(":"));
				CMSSiteUrl url = dbcontext.CMSSiteUrls.Where(x => x.Url == domain).FirstOrDefault();
				if (url != null)
				{
					int siteid = url.SiteId;
					string savePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
					savePath = Path.Combine(savePath, siteid.ToString());

					List<CMSFile> files = new List<CMSFile>();

					foreach (var file in context.Request.Form.Files)
					{
						string tempfile = Common.CreateTempfilePath();
						using var stream = File.OpenWrite(tempfile);
						await file.CopyToAsync(stream);

						CMSFile thefile = new CMSFile();
						thefile.Filename = file.FileName;
						thefile.SiteId = siteid;
						thefile.Temp = tempfile;
						files.Add(thefile);
					}

					try
					{
						foreach (CMSFile thefile in files)
						{
							File.Copy(thefile.Temp, Path.Combine(savePath, thefile.Filename));
							File.Delete(thefile.Temp);
							thefile.Temp = null;
							dbcontext.CMSFiles.Add(thefile);
						}
					}
					catch (Exception e)
					{
						// I need to log something here.
						throw;
					}
				}
				dbcontext.SaveChanges();
				return true;
			}
			return false;
		}
	}
}