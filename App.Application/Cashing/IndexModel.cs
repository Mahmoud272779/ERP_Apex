using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Cashing
{
    public class IndexModel : PageModel
    {
        private readonly IDistributedCache _cache;

        public IndexModel(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string CachedTimeUTC { get; set; }

        public async Task OnGetAsync()
        {
            CachedTimeUTC = "Cached Time Expired";
            var encodedCachedTimeUTC = await _cache.GetAsync("cachedTimeUTC");

            if (encodedCachedTimeUTC != null)
            {
                CachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);
            }
        }

        public async Task<IActionResult> OnPostResetCachedTime()
        {
            var currentTimeUTC = DateTime.UtcNow.ToString();
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20));
            await _cache.SetAsync("cachedTimeUTC", encodedCurrentTimeUTC, options);

            return RedirectToPage();
        }
    }
}


//    public class IndexModel : PageModel
//    {
//        private readonly IMemoryCache _memoryCache;

//        public IndexModel(IMemoryCache memoryCache) =>
//            _memoryCache = memoryCache;

//        // ...

//        public void OnGet()
//        {
//           // CurrentDateTime = DateTime.Now;

//            if (!_memoryCache.TryGetValue(CacheKeys.Entry, out DateTime cacheValue))
//            {
//                //cacheValue = CurrentDateTime;

//                var cacheEntryOptions = new MemoryCacheEntryOptions()
//                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

//                _memoryCache.Set(CacheKeys.Entry, cacheValue, cacheEntryOptions);
//            }

//           // CacheCurrentDateTime = cacheValue;
//        }
//        public void OnGetCacheGetOrCreate()
//        {
//            var cachedValue = _memoryCache.GetOrCreate(
//                CacheKeys.Entry,
//                cacheEntry =>
//                {
//                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
//                    return DateTime.Now;
//                });

//            // ...
//        }

//        public async Task OnGetCacheGetOrCreateAsync()
//        {
//            var cachedValue = await _memoryCache.GetOrCreateAsync(
//                CacheKeys.Entry,
//                cacheEntry =>
//                {
//                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
//                    return Task.FromResult(DateTime.Now);
//                });

//            // ...
//        }

//    }
//}
