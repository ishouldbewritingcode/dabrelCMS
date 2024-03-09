namespace dabrelCMS
{
	public class CMSUploaderMiddleware
	{
		private readonly ILogger<CMSUploaderMiddleware> _logger;
		private readonly RequestDelegate _next;
		private readonly RequestDelegate _uploads;

		public CMSUploaderMiddleware(ILogger<CMSUploaderMiddleware> logger, RequestDelegate next)
		{
			_logger = logger;
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			await _next.Invoke(context); // pass the context

			Uploader uploader = new Uploader();
			await uploader.UploadFiles(context);
		}
	}

	public static class MiddlewareExtensions2
	{
		public static IApplicationBuilder UseCMSUploaderMiddleware(this IApplicationBuilder app)
		{
			return app.UseMiddleware<CMSUploaderMiddleware>();
		}
	}
}