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
	}
}