using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Helpers.CashingService.GetFromCache
{
    public class GetFromCacheHandler : IRequestHandler<GetFromCacheRequest, object>
    {
        private readonly IMemoryCache _memoryCache;

        public GetFromCacheHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<object> Handle(GetFromCacheRequest request, CancellationToken cancellationToken)
        {
            return _memoryCache.TryGetValue(request.key, out var data) ? data : null;
        }
    }
}
