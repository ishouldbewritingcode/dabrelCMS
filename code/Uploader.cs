using System.IO;
using System.Net;

namespace dabrelCMS.code
{
	public class Uploader
	{
		public async Task<string> UploadFiles(HttpContext context)
		{
			await Task.Yield();

			try
			{
				string path = context.Request.Path.ToString().ToLower().Trim().Replace("/", string.Empty);
				if (path == "adminupload")
				{
					CMSDbContext cmsDbContext = new();
					using CMSDbContext dbcontext = cmsDbContext;
					string domain = context.Request.Host.ToString().ToLower().Trim();
					// strip off any port numbers that may be involved
					int colonIndex = domain.IndexOf(":");
					if (colonIndex > 0)
						domain = domain.Substring(0, colonIndex);

					CMSSiteUrl url = dbcontext.CMSSiteUrls.Where(x => x.Url == domain).FirstOrDefault();
					if (url != null)
					{
						Guid siteid = url.SiteId;
						string savePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
						savePath = Path.Combine(savePath, siteid.ToString());

						// Handle target directory
						string? targetDir = context.Request.Form["targetDirectory"];
						if (!string.IsNullOrEmpty(targetDir))
						{
							savePath = Path.Combine(savePath, targetDir);
						}

						// Ensure directory exists
						if (!Directory.Exists(savePath))
							Directory.CreateDirectory(savePath);

						List<CMSFile> files = new List<CMSFile>();

						foreach (var file in context.Request.Form.Files)
						{
							string tempfile = Common.CreateTempfilePath();
							using var stream = File.OpenWrite(tempfile);
							await file.CopyToAsync(stream);

							CMSFile thefile = new CMSFile();
							thefile.FileId = Guid.CreateVersion7();
							thefile.Filename = file.FileName;
							thefile.SiteId = siteid;
							thefile.Temp = tempfile;
							files.Add(thefile);
						}

						foreach (CMSFile thefile in files)
						{
							File.Copy(thefile.Temp, Path.Combine(savePath, thefile.Filename));
							File.Delete(thefile.Temp);
							thefile.Temp = null;
							dbcontext.CMSFiles.Add(thefile);
						}
					}
					dbcontext.SaveChanges();
					return "success uploading";
				}
				return "failed";
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
	}
}