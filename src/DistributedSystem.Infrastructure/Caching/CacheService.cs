using DistributedSystem.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DistributedSystem.Infrastructure.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        /**
         * Why we use ConcurrentDictionary?
         * 
         * Because we don't have method to get all keys in redis
         * => Solution: Store key in memory at set value to redis
         * 
         * => Cache Service can be used concurrently, so we have to make sure that the data structure that we choose is thread-safe
         */
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new ConcurrentDictionary<string, bool>(); 

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            string? cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (cacheValue is null)
            {
                return null;
            }

            T? value = JsonSerializer.Deserialize<T>(cacheValue);
            return value;
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            string cacheValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);

            // Mình đặt true, false cũng được, vì mình chỉ cần lưu key
            CacheKeys.TryAdd(key, true);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);

            CacheKeys.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            //foreach(string key in CacheKeys.Keys)
            //{
            //    if (key.StartsWith(prefixKey))
            //        await RemoveAsync(key, cancellationToken); // Call remove one by one
            //}

            // Performace issue => Parallel
            // Chúng ta gọi như này nó chưa remove hẳn, chúng ta mới tạo ra danh sách mà mỗi phần tử của nó là một Task mà mỗi Task là một Function RemoveAsync
            IEnumerable<Task> tasks = CacheKeys.Keys.Where(key => key.StartsWith(prefixKey))
                .Select(k => RemoveAsync(k, cancellationToken));

            // Xóa một lần => Chạy parallel
            await Task.WhenAll(tasks); // Execute in parallel
        }
    }
}