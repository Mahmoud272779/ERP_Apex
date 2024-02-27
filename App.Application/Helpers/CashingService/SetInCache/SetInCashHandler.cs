using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Helpers.CashingService.SetInCash
{
    public class SetInCashHandler : IRequestHandler<SetInCashRequest, bool>
    {
        private readonly IMemoryCache _memoryCache;

        public SetInCashHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<bool> Handle(SetInCashRequest request, CancellationToken cancellationToken)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.expirationTime,
                // You can also use other expiration options like SlidingExpiration or Priority as needed
            };

            _memoryCache.Set(request.key, request.data, cacheEntryOptions);

            var data = _memoryCache.Get(request.key);
            return true;
        }
    }
}
