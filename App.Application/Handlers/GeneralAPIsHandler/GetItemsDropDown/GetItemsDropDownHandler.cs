using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetItemsDropDown
{
    public class GetItemsDropDownHandler : IRequestHandler<GetItemsDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;

        public GetItemsDropDownHandler(IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IRepositoryQuery<InvStpUnits> invStpUnitsQuery)
        {
            this.itemCardMasterRepository = itemCardMasterRepository;
            _invStpUnitsQuery = invStpUnitsQuery;
        }

        public async Task<ResponseResult> Handle(GetItemsDropDownRequest request, CancellationToken cancellationToken)
        {

            var ItemsList = itemCardMasterRepository.TableNoTracking.Include(a => a.Units)
                                                    .Where(e => request.invoiceTypeId != null ? ((Lists.salesInvoicesList.Contains(request.invoiceTypeId.Value)||
                                                                    request.invoiceTypeId.Value==(int)DocumentType.OfferPrice) ? e.UsedInSales : true) : true)
                                                    .Where(e => request.isInvoice == true ? e.Status == (int)Status.Active : true)
                                                    .Where(e => request.invoiceTypeId != null ? (Lists.CompositeItemOnInvoice.Contains(request.invoiceTypeId.Value) ? true : e.TypeId != (int)ItemTypes.Composite) : true)
                                                    .Where(e => request.isSearchByCode == true ? (e.ItemCode == request.SearchCriteria || e.Units.Where(e => e.Barcode == request.SearchCriteria).Any() || e.NationalBarcode == request.SearchCriteria) : true)
                                                    .Where(e => request.SearchCriteria != null && request.isSearchByCode == null ? (e.ArabicName.Contains(request.SearchCriteria) || e.LatinName.Contains(request.SearchCriteria)) : true)
                                                    .Where(x => request.itemType != 0 ? (x.TypeId == request.itemType) : true);
            //.Select(a => new { a.Id, a.ItemCode, a.ArabicName, a.LatinName, a.TypeId, a.Status })

            if (request.invoiceTypeId != null)
                if (Lists.transferStore.Contains(request.invoiceTypeId.Value))
                {
                    ItemsList = ItemsList.Where(a => a.TypeId != (int)ItemTypes.Composite && a.TypeId != (int)ItemTypes.Service);
                }

            double MaxPageNumber = ItemsList.Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

          

            var totalCount = itemCardMasterRepository.TableNoTracking.Where(e => e.Status == (int)Status.Active).Count();
            if (request.isSearchByCode != true)
            {
                ItemsList = ItemsList.Where(x => x.Status == (int)Status.Active);
            }
             //ItemsList= ItemsList.OrderBy(a => a.ArabicName);
            if (!string.IsNullOrEmpty(request.SearchCriteria))
                ItemsList = ItemsList.OrderByDescending(a=>a.ArabicName==request.SearchCriteria).ThenByDescending(a => a.ArabicName.StartsWith(request.SearchCriteria));
          
            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                ItemsList = ItemsList.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
            }
            var ReportResponse = new List<ItemsDropdownlistReport>();
            var InvoiceResponse = new List<ItemsDropdownlistInvoice>();
            if (request.isInvoice)
            {
                InvoiceResponse = ItemsList.Select(x => new ItemsDropdownlistInvoice
                {
                    Id = x.Id,
                    ArabicName = x.ArabicName,
                    ItemCode = x.ItemCode,
                    itemType = x.TypeId,
                    LatinName = x.LatinName,
                    Status = x.Status
                }).ToList();
                //if(!string.IsNullOrEmpty(request.SearchCriteria))
                //      InvoiceResponse = InvoiceResponse.OrderByDescending(a => a.ArabicName.StartsWith( request.SearchCriteria)).ToList();
            }
            else
            {
                var units = _invStpUnitsQuery.TableNoTracking;
                ReportResponse = ItemsList.Select(x => new ItemsDropdownlistReport
                {
                    Id = x.Id,
                    Status = x.Status,
                    LatinName = x.LatinName,
                    itemType = x.TypeId,
                    ItemCode = x.ItemCode,
                    ArabicName = x.ArabicName,
                    units = units.Where(c => x.Units.Select(t => t.UnitId).Contains(c.Id)).Select(d => new UnitsForReports
                    {
                        Id = d.Id,
                        ArabicName = d.ArabicName,
                        LatinName = d.LatinName,
                        isDefult = x.ReportUnit == d.Id ? true : false
                    }).ToList()
                }).ToList();

                //if (!string.IsNullOrEmpty(request.SearchCriteria))
                //    ReportResponse = ReportResponse.OrderByDescending(a => a.ArabicName.StartsWith(request.SearchCriteria)).ToList();

            }
            return new ResponseResult()
            {
                Data = request.isInvoice ? InvoiceResponse : ReportResponse,
                DataCount = ItemsList.Count(),
                Id = null,
                Result = ItemsList.Any() ? Result.Success : Result.Failed,
                TotalCount = totalCount,
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : ""),

            };
        }
    }
}
