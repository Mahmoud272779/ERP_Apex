using App.Application.Handlers.InvoicesHelper.GetItemsDropDown;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralAPIsHandler.GetItemsDropDownForReports
{
    public class GetItemsDropDownForReportsHandler : IRequestHandler<GetItemsDropDownForReportsRequest, ResponseResult>
    {
        private readonly IMediator _mediator;

        public GetItemsDropDownForReportsHandler( IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ResponseResult> Handle(GetItemsDropDownForReportsRequest parm, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetItemsDropDownRequest
            {
                isSearchByCode = parm.code != null ? true : null,
                PageNumber = parm.pageNumber,
                PageSize = parm.pageSize,
                SearchCriteria = parm.code == null ? parm.name : parm.code,
                invoiceTypeId = null,
                itemType = 0,
                isInvoice = false
            });










            //var items = itemCardMasterRepository.TableNoTracking.Include(x => x.Units).ToList();
            //var units = _invStpUnitsQuery.TableNoTracking.Select(x => new
            //{
            //    x.Id,
            //    x.ArabicName,
            //    x.LatinName
            //});
            //if (!string.IsNullOrEmpty(parm.name))
            //    items = items.Where(x => x.ArabicName.Contains(parm.name)).ToList();
            //if (!string.IsNullOrEmpty(parm.code))
            //    items = items.Where(x => x.ItemCode == parm.code || x.NationalBarcode == parm.code || x.Units.Select(c => c.Barcode).Contains(parm.code)).ToList();
            //items = items.Skip((parm.pageNumber - 1) * parm.pageSize).Take(parm.pageSize).ToList();
            //var res = items.Select(x => new
            //{
            //    x.Id,
            //    x.ArabicName,
            //    x.LatinName,
            //    x.ItemCode,
            //    units = units.Where(c => x.Units.Select(t => t.UnitId).Contains(c.Id)).Select(d => new
            //    {
            //        d.Id,
            //        d.ArabicName,
            //        d.LatinName,
            //        isDefult = x.ReportUnit == d.Id ? true : false
            //    })
            //});
            //double MaxPageNumber = items.ToList().Count() / Convert.ToDouble(parm.pageSize);
            //var countofFilter = Math.Ceiling(MaxPageNumber);
            //return new ResponseResult()
            //{
            //    Data = res,
            //    Result = Result.Success,
            //    Note = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
            //    TotalCount = items.Count()
            //};
        }
    }
}
