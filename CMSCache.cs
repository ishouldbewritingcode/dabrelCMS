using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace dabrelCMS
{
    public static class CMSCache
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();

        public static IChangeToken GetSiteToken(string domain)
        {
            var cts = _tokens.GetOrAdd(domain, _ => new CancellationTokenSource());
            return new CancellationChangeToken(cts.Token);
        }

        public static void InvalidateSite(string domain)
        {
            if (_tokens.TryRemove(domain, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }
        }
    }
}
