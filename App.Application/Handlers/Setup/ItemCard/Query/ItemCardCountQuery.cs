using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class ItemCardCountQuery : BaseClass, IRequestHandler<GetItemCountRequest, int>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepository;

        public ItemCardCountQuery(IHttpContextAccessor _httpContextAccessor, IRepositoryQuery<InvStpItemCardMaster> itemCardRepository) : base(_httpContextAccessor)
        {
            this.itemCardRepository = itemCardRepository;
        }
        public Task<int> Handle(GetItemCountRequest request, CancellationToken cancellationToken)
        {
            return Task.Run(()=> itemCardRepository.TableNoTracking.DefaultIfEmpty().Count());
        }
    }
}
