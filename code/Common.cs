using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;

namespace dabrelCMS.code
{
	public static class Common
	{
		public static string WebRootPath { get; set; }
		public static string AdminDesign { get; set; }

		public static string GetLoginPage(HttpContext context, string path, string message)
		{
			string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			string LoginPage = GetFileText(Path.Combine(webRootPath, "login.htm"));
			LoginPage = LoginPage.Replace("{{redirect}}", path);
			LoginPage = LoginPage.Replace("{{message}}", message);
			context.Response.StatusCode = StatusCodes.Status200OK;
			return LoginPage;
		}

		public static string Send404(HttpContext context)
		{
			string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			context.Response.StatusCode = StatusCodes.Status404NotFound;
			return GetFileText(Path.Combine(webRootPath, "404.htm"));
		}

		public static string GetFileText(string filePath)
		{
			try
			{
				using (var sr = new StreamReader(filePath))
				{
					return sr.ReadToEnd();
				}
			}
			catch (IOException e)
			{
				return e.Message;
			}
		}

		public static string GetAdminPage(HttpContext context)
		{
			context.Response.StatusCode = StatusCodes.Status200OK;
			return "";
		}

		public static string GetManagerPage(HttpContext context)
		{
			context.Response.StatusCode = StatusCodes.Status200OK;
			return "";
		}

		public static string CreateTempfilePath()
		{
			string filename = $"{Guid.NewGuid()}.tmp";
			string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads\\temp");
			if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

			return Path.Combine(directoryPath, filename);
		}

		public static string GetFAIcon(string filename)
		{
			string ext = Path.GetExtension(filename).ToLower();
			switch (ext)
			{
				case ".cs":
				case ".css":
				case ".bat":
				case ".htm":
				case ".html":
				case ".js":
				case ".ps1":
				case ".razor":
				case ".xml":
				case ".xsl":
					return "<i class=\"fa-regular fa-file-code\"></i>";

				case ".doc":
				case ".docx":
					return "<i class=\"fa-regular fa-file-word\"></i>";

				case ".jpg":
				case ".png":
				case ".gif":
				case ".bmp":
				case ".tif":
					return "<i class=\"fa-regular fa-file-image\"></i>";

				case ".mp3":
				case ".wav":
				case ".ogg":
					return "<i class=\"fa-regular fa-file-audio\"></i>";

				case ".pdf":
					return "<i class=\"fa-regular fa-file-pdf\"></i>";

				case ".ppt":
				case ".pptx":
					return "<i class=\"fa-regular fa-file-powerpoint\"></i>";

				case ".log":
				case ".txt":
					return "<i class=\"fa-regular fa-file-lines\"></i>";

				case ".avi":
				case ".mp4":
					return "<i class=\"fa-regular fa-file-video\"></i>";

				case ".xls":
				case ".xlsx":
					return "<i class=\"fa-regular fa-file-excel\"></i>";

				case ".7z":
				case ".rar":
				case ".zip":
					return "<i class=\"fa-regular fa-file-zipper\"></i>";

				default:
					return "<i class=\"fa-regular fa-file\"></i>";
			}
		}
	}
}