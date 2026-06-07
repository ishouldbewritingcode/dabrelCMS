namespace dabrelCMS
{
	public class CMSMiddleware
	{
		private readonly ILogger<CMSMiddleware> _logger;
		private readonly RequestDelegate _next;

		public CMSMiddleware(ILogger<CMSMiddleware> logger, RequestDelegate next)
		{
			_logger = logger;
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			DateTime start = DateTime.Now;

			await _next.Invoke(context);

			Site theSite = new Site();
			await context.Response.WriteAsync(theSite.GetSite(context));

			_logger.LogInformation("Request {Path}{QueryString}: {Ms}ms",
				context.Request.Path, context.Request.QueryString, (DateTime.Now - start).TotalMilliseconds);
		}
	}

	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseCMSMiddleware(this IApplicationBuilder app)
		{
			return app.UseMiddleware<CMSMiddleware>();
		}
	}
}
