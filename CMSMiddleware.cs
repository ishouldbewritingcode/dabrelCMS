using Microsoft.Extensions.Caching.Memory;

namespace dabrelCMS
{
	public class CMSMiddleware
	{
		private readonly ILogger<CMSMiddleware> _logger;
		private readonly RequestDelegate _next;
		private readonly IMemoryCache _cache;

		public CMSMiddleware(ILogger<CMSMiddleware> logger, RequestDelegate next, IMemoryCache cache)
		{
			_logger = logger;
			_next = next;
			_cache = cache;
		}

		public async Task Invoke(HttpContext context)
		{
			DateTime start = DateTime.Now;
			await _next.Invoke(context);

			string path = context.Request.Path.ToString().ToLower().TrimStart('/');
			string domain = context.Request.Host.Host.ToLower();

			bool cacheable = context.Request.Method == "GET"
				&& !path.StartsWith("admin")
				&& !path.StartsWith("sitemanager")
				&& !path.StartsWith("auth");

			string html;
			if (cacheable)
			{
				string key = $"page:{domain}:{path}:{(context.Request.Headers.ContainsKey("HX-Request") ? 1 : 0)}";
				if (!_cache.TryGetValue(key, out string? cached))
				{
					html = new Site().GetSite(context);
					var options = new MemoryCacheEntryOptions()
						.AddExpirationToken(CMSCache.GetSiteToken(domain))
						.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
					_cache.Set(key, html, options);
					_logger.LogDebug("Cache miss: {Key}", key);
				}
				else
				{
					html = cached!;
				}
			}
			else
			{
				html = new Site().GetSite(context);
			}

			await context.Response.WriteAsync(html);
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
