using App.Infrastructure.settings;
using Microsoft.Extensions.Caching.Memory;

namespace App.Application.Helpers
{
    public class MemoryCashHelper
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCashHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SaveValueIntoCash(string key, string value)
        {
            _memoryCache.Set(key, value);
        }
        public string GetValue(string key)
        {
            var value = _memoryCache.Get(key).ToString();
            return value;
        }
        public async Task AddSignalRCash(SignalRCash signalRCash, string CacheKey )
        {
            var signalRItems = new List<SignalRCash>();
            var getCashedValues = _memoryCache.Get<List<SignalRCash>>(CacheKey);
            if (getCashedValues != null)
            {
                signalRItems.AddRange(getCashedValues);
                var isExist = getCashedValues.Where(x => x.isTechSupport == signalRCash.isTechSupport && x.EmployeeId == signalRCash.EmployeeId && x.DBName == signalRCash.DBName);
                if (isExist.Any())
                {
                    signalRItems.Remove(isExist.FirstOrDefault());
                }
            }

            signalRItems.Add(signalRCash);
            _memoryCache.Set<List<SignalRCash>>(CacheKey, signalRItems);
            var _getCashedValues = _memoryCache.Get<List<SignalRCash>>(CacheKey);

        }
        public List<SignalRCash> GetSignalRCashedValues()
        {
            var value = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
            return value;
        }
        public void DeleteSignalRCahedRecored(string ConnectionId ,string CacheKey,string? companyLogin)
        {
            var getCashedValues = _memoryCache.Get<List<SignalRCash>>(CacheKey).ToList();
            var itemToRemove = getCashedValues.Where(x => companyLogin == null ? ConnectionId == x.connectionId : x.CompanyLogin == companyLogin).ToList();
            var list = new List<SignalRCash>();
            list = getCashedValues.Where(x => !itemToRemove.Select(c => c.connectionId).ToArray().Contains(x.connectionId)).ToList();
            _memoryCache.Set<List<SignalRCash>>(CacheKey, list);
        }

    }
    public class SignalRCash
    {
        public string connectionId { get; set; }
        public string CompanyLogin { get; set; }
        public string DBName { get; set; }
        public int UserID { get; set; }
        public int EmployeeId { get; set; }
        public int isTechSupport { get; set; }
    }

}

