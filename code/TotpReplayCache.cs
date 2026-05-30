using System.Collections.Concurrent;

namespace dabrelCMS.code
{
    internal static class TotpReplayCache
    {
        // key: "{userId}:{windowIndex}" — windowIndex = unixSeconds / 30
        private static readonly ConcurrentDictionary<string, DateTime> _used = new();

        internal static bool TryMarkUsed(Guid userId, long windowIndex)
        {
            Purge();
            string key = $"{userId}:{windowIndex}";
            return _used.TryAdd(key, DateTime.UtcNow);
        }

        private static void Purge()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-3);
            foreach (var kv in _used)
                if (kv.Value < cutoff)
                    _used.TryRemove(kv.Key, out _);
        }
    }
}
