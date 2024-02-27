using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup;
using App.Infrastructure.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using App.Domain.Entities.Setup;
using App.Infrastructure.Interfaces.Repository;
using System.Drawing.Printing;
using Microsoft.EntityFrameworkCore;
using App.Infrastructure.Mapping;
using App.Domain.Models.Shared;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class ItemCardFilterQuery : BaseClass, IRequestHandler<ItemCardFilterRequest, ResponseResult>
    {

        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepository;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemPartsRepository;

        public ItemCardFilterQuery(IHttpContextAccessor httpContextAccessor,
            IRepositoryQuery<InvStpItemCardMaster> itemCardRepository,
            IRepositoryQuery<InvStpItemCardParts> itemPartsRepository) : base(httpContextAccessor)
        {
            this.itemCardRepository = itemCardRepository;
            this.itemPartsRepository = itemPartsRepository;
        }

        public async Task<ResponseResult> Handle(ItemCardFilterRequest request, CancellationToken cancellationToken)
        {

            var allItems = itemCardRepository.TableNoTracking.Where(e => 1 == 1);
            //if (!string.IsNullOrEmpty(request.CodeOrName))
            //    request.CodeOrName = request.CodeOrName.Trim();
            //    allItems = allItems.Where(e => e.ItemCode.ToLower().Contains(request.CodeOrName.ToLower())
            //                        || e.LatinName.ToLower().Contains(request.CodeOrName.ToLower())
            //                        || e.ArabicName.ToLower().Contains(request.CodeOrName.ToLower())
            //                        || e.NationalBarcode.ToLower().Contains(request.CodeOrName.ToLower())
            //                        || e.Units.Select(a => a.Barcode).Contains(request.CodeOrName.ToLower()));
            if (request.Categories.Any() && !request.Categories.Contains(0))
                allItems = allItems.Where(e => request.Categories.Contains(e.GroupId));
            if (request.ItemTypes.Any() && !request.ItemTypes.Contains(0))
                allItems = allItems.Where(e => request.ItemTypes.Contains(e.TypeId));
            if (request.Stores.Any() && !request.Stores.Contains(0))
                allItems = allItems.Where(e => request.Stores.Contains(e.DefaultStoreId ?? 0));
            if (request.Status != 0)
            {
                allItems = allItems.Where(e => e.Status == request.Status);
            }
            allItems = allItems.OrderByDescending(q => q.Id);
            var resultsCount = allItems.Count();
            if (request.PageNumber >= 0 && request.PageSize != 0)
                allItems = allItems.Skip((request.PageNumber) * request.PageSize ?? 0).Take(request.PageSize ?? 0);

            List<GetAllItemsResponse> responses = new List<GetAllItemsResponse>();
            var results = allItems.Include(q => q.Category).Include(q => q.InvoicesDetails).ToList().OrderBy(e => e.ItemCode).ToList();

            if (results.Count > 0)
            {
                for (int k = 0; k < results.Count; k++)
                {
                    var parts = itemPartsRepository.TableNoTracking.Where(a => a.PartId == results[k].Id).Select(a => a.PartId);

                    results[k].CanDelete = true && results[k].Id != 1;

                    var itemExistInParts = parts.Contains(results[k].Id);
                    if (results[k].InvoicesDetails.Count > 0 || itemExistInParts)
                    {
                        results[k].CanDelete = false;
                    }
                }
            }

            Mapping.Mapper.Map(results, responses, typeof(List<InvStpItemCardMaster>), typeof(List<GetAllItemsResponse>));
            return new ResponseResult() { Data = responses, DataCount = resultsCount };
        }


    }
}
