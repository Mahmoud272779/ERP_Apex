using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Common.Behaviours
{   //k
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        //private readonly ICurrentUserService _currentUserService;
        //private readonly IIdentityService _identityService;

        //public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
        //{
        //    _logger = logger;
        //    _currentUserService = currentUserService;
        //    _identityService = identityService;
        //}
        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Process(TRequest request, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            string requestName = typeof(TRequest).Name;
            //var userId = _currentUserService.UserId ?? string.Empty;
            string userName = string.Empty;
            //hamada
            //if (!string.IsNullOrEmpty(userId))
            //{
            //    //userName = await _identityService.GetUserNameAsync(userId);
            //}

            //_logger.LogInformation("New_folder Request: {Name} {@UserId} {@UserName} {@Request}",
            //    requestName, userId, userName, request);
        }
    }
}
