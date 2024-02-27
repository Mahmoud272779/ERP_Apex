using App.Api.Controllers.BaseController;
using App.Domain.Entities.Process;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class SettingCachController : ApiStoreControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryQuery<InvGeneralSettings> _SettingRepositoryQuery;

        public SettingCachController(IMemoryCache memoryCache, 
            IRepositoryQuery<InvGeneralSettings> SettingRepositoryQuery ,
            IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            _memoryCache = memoryCache;
            _SettingRepositoryQuery = SettingRepositoryQuery;
        }

        [HttpGet("GetAllSettings")]
        
        public async Task<IActionResult> GetAllSettings()
        {
            var cacheKey = "GeneralSettingErp";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<InvGeneralSettings> settingsList))
            {
                //calling the server
                settingsList = await _SettingRepositoryQuery.Get();

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromDays(1)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, settingsList, cacheExpiryOptions);
            }
            return Ok(settingsList);
        }
    }
}
