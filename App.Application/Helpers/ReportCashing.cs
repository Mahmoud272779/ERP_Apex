using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using FastReport.Barcode;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public static class ReportCashing
    {
        public static void AddDataToCache(IMemoryCache _memoryCache, string key, object data, TimeSpan expirationTime)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime,
                // You can also use other expiration options like SlidingExpiration or Priority as needed
            };

            _memoryCache.Set(key, data, cacheEntryOptions);
            var ddd = _memoryCache.Get<IEnumerable<SuppliersAccountList>>(key);
        }

        public static object GetDataFromCache(IMemoryCache _memoryCache,string key)
        {   
            var resData =_memoryCache.TryGetValue(key, out var data) ? data : null;
            
            return resData;
        }
    }
}
