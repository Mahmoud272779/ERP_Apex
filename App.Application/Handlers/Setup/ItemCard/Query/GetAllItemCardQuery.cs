using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class GetAllItemCardQuery : BaseClass, IRequestHandler<GetAllItemCardRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepository;
        private readonly IRepositoryQuery<OfferPriceDetails> _OfferPriceDetailsQuery;
        private readonly IRepositoryQuery<InvCategories> invCategoriesRepository;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsRepository;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemPartsRepository;

        public GetAllItemCardQuery(IHttpContextAccessor _httpContextAccessor,
                               IRepositoryQuery<InvStpItemCardMaster> ItemCardRepository,
                               IRepositoryQuery<InvCategories> invCategoriesRepository,
                               IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepository,
                               IRepositoryQuery<InvStpItemCardParts> itemPartsRepository,
                               IRepositoryQuery<OfferPriceDetails> offerPriceDetailsQuery) : base(_httpContextAccessor)
        {
            itemCardRepository = ItemCardRepository;
            this.invCategoriesRepository = invCategoriesRepository;
            _invoiceDetailsRepository = InvoiceDetailsRepository;
            this.itemPartsRepository = itemPartsRepository;
            _OfferPriceDetailsQuery = offerPriceDetailsQuery;
        }

        public async Task<ResponseResult> Handle(GetAllItemCardRequest request, CancellationToken cancellationToken)
        {
            List<GetAllItemsResponse> responses = new List<GetAllItemsResponse>();
            // IQueryable<InvStpItemCardMaster> initialresults;
            var initialresults = Enumerable.Empty<InvStpItemCardMaster>().AsQueryable();
            if (request.IsSearchData)
            {
                initialresults = itemCardRepository.TableNoTracking.Include(i => i.StorePlace);

            }
            else
            {
                string[] listId = request.Ids.Split(",");
                foreach (var id in listId)
                {
                    var item = itemCardRepository.TableNoTracking.Include(a => a.Category).Include(a => a.StorePlace).Where(a => a.Id == Convert.ToInt32(id)).FirstOrDefault();
                    initialresults = initialresults.Append(item);

                }
            }
            if (string.IsNullOrEmpty(request.Name))
                initialresults = initialresults.OrderByDescending(keySelector: q => q.Id);
            var categories = request.categories.Split(',').ToList().Select(x => int.Parse(x)).ToArray();
            var itemTypes = request.itemTypes.Split(',').ToList().Select(x => int.Parse(x)).ToArray();
            var storesPlaces = request.storesPlaces.Split(',').ToList().Select(x => int.Parse(x)).ToArray();
            var PageNumber = request.PageNumber ?? 1;
            var PageSize = request.PageSize ?? 1;
            initialresults = initialresults
                .Include(e => e.Units).ThenInclude(e => e.Unit)
                .Include(e => e.Category)
                .Include(e => e.ItemParts)
                .Where(e => 1 == 1)
                .Where(x => request.Statues != 0 ? x.Status == request.Statues : true)
                .Where(x => categories.First() != 0 ? categories.Contains(x.GroupId) : true)
                .Where(x => itemTypes.First() != 0 ? itemTypes.Contains(x.TypeId) : true)
                .Where(x => storesPlaces.First() != 0 ? storesPlaces.Contains(x.DefaultStoreId ?? 0) : true);

            if (!string.IsNullOrEmpty(request.Name) && request.IsSearchData)
            {
                request.Name = request.Name.ToLower().Trim();
                initialresults = initialresults.Where(e => e.ItemCode.ToLower().Contains(request.Name)
                                    || e.LatinName.ToLower().Contains(request.Name)
                                    || e.ArabicName.ToLower().Contains(request.Name)
                                    || e.NationalBarcode.ToLower().Contains(request.Name)
                                    || e.Units.Where(e => e.Barcode.Contains(request.Name)).Any())
                    .OrderByDescending(a => (a.ItemCode == request.Name ? a.ItemCode == request.Name : a.ArabicName == request.Name))
                    .ThenByDescending(a => a.ItemCode.EndsWith(request.Name))
                    .ThenByDescending(a => a.ArabicName.StartsWith(request.Name));//||

            }
                var count = initialresults.Count();
                initialresults = request.isPrint ? initialresults : initialresults.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                var invoices = _invoiceDetailsRepository.TableNoTracking;
                var offerPrices = _OfferPriceDetailsQuery.TableNoTracking;
                var results = new List<InvStpItemCardMaster>();
                foreach (var item in initialresults)
                {
                    if (item.Id == 1)
                        item.CanDelete = false;
                    else
                        item.CanDelete = invoices.Where(x => x.ItemId == item.Id).Any() || offerPrices.Where(x => x.ItemId == item.Id).Any() ? false : true;
                    results.Add(item);
                }
                return new ResponseResult() { Data = results, DataCount = count, TotalCount = initialresults.Count(), Result = results.Count > 0 ? Domain.Enums.Enums.Result.Success : Domain.Enums.Enums.Result.NoDataFound }; ;
            }

        
    }
    
    
}
