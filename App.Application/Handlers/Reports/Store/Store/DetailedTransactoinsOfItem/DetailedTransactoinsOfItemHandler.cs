using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Infrastructure.settings;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System.Threading;

namespace App.Application.Handlers.Reports.Store.Store.DetailedTransactoinsOfItem
{
    public class DetailedTransactoinsOfItemHandler : IRequestHandler<DetailedTransactoinsOfItemRequest, ResponseResult>
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepositoryQuery<InvStpStores> _storeQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IGeneralAPIsService _IGeneralAPIsService;


        public DetailedTransactoinsOfItemHandler(IHttpContextAccessor httpContext, IRepositoryQuery<InvStpStores> storeQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery, IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery, IRoundNumbers roundNumbers, IGeneralAPIsService iGeneralAPIsService)
        {
            _httpContext = httpContext;
            _storeQuery = storeQuery;
            _InvoiceDetailsQuery = invoiceDetailsQuery;
            _invStpItemCardMasterQuery = invStpItemCardMasterQuery;
            this.roundNumbers = roundNumbers;
            _IGeneralAPIsService = iGeneralAPIsService;
        }
        public async Task<ResponseResult> Handle(DetailedTransactoinsOfItemRequest param, CancellationToken cancellationToken)
        {
            #region validation
            if (string.IsNullOrEmpty(_httpContext.HttpContext.GetTokenAsync("access_token").Result))
                return new ResponseResult()
                {
                    Note = "UnAuthorized",
                    Result = Result.Failed
                };
            if (string.IsNullOrEmpty(param.dateFrom.ToString()) || string.IsNullOrEmpty(param.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Date from and Date to is Required",
                    Result = Result.Failed
                };
            if (!param.isPrint)
            {
                if (string.IsNullOrEmpty(param.PageNumber.ToString()) || string.IsNullOrEmpty(param.PageSize.ToString()))
                    return new ResponseResult()
                    {
                        Note = "Page Number and Page Size are Required",
                        Result = Result.Failed
                    };
            }

            if (string.IsNullOrEmpty(param.dateFrom.ToString()) || string.IsNullOrEmpty(param.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Date From And Date To are Required",
                    Result = Result.Failed
                };
            #endregion
            var returnInvoicesList = Lists.returnInvoiceList;
            var Transfers = Lists.Transfers;
            var transactionList = TransactionTypeList.transactionTypeModels();
            var stores = _storeQuery.TableNoTracking;
            var InvoicesOfItem = _InvoiceDetailsQuery.TableNoTracking
                .Include(x => x.InvoicesMaster)
                .ThenInclude(x => x.storeTo)
                .Include(x => x.InvoicesMaster.store)
                .Include(x => x.Items)
                .Include(x => x.Items.Units)
                .Where(x => x.ItemId == param.itemId)
                .Where(x => x.InvoicesMaster.StoreId == param.storeId)
                .Where(x => !x.InvoicesMaster.IsDeleted)
                .ToList()
                .GroupBy(x => new { x.InvoiceId })
                .OrderBy(x => x.First().InvoicesMaster.InvoiceDate)
                .Select(x => new firstSelection
                {
                    id = x.First().InvoiceId,
                    date = x.First().InvoicesMaster.InvoiceDate.Date,
                    DocumentCode = x.First().InvoicesMaster.InvoiceType.Replace("_", string.Empty),
                    Qyt = ReportData<InvoiceDetails>.Quantity(x, x.First().Items.Units.Where(c => c.UnitId == param.unitId).FirstOrDefault()?.ConversionFactor ?? 0),
                    rowClassName = returnInvoicesList.Contains(x.First().InvoicesMaster.InvoiceTypeId) || x.First().InvoicesMaster.InvoiceSubTypesId == (int)SubType.RejectedTransfer ? defultData.text_danger : "",
                    TransactionTypeAr = (!Transfers.Where(c => c == x.First().InvoicesMaster.InvoiceTypeId).Any() ? transactionList.Find(f => f.id == x.First().InvoicesMaster.InvoiceTypeId)?.arabicName ?? "" : _IGeneralAPIsService.GetTransferDesc(x.First().InvoicesMaster.InvoiceTypeId, x.First().InvoicesMaster.StoreId, x.First().InvoicesMaster.StoreIdTo, x.First().InvoicesMaster.InvoiceSubTypesId == (int)SubType.RejectedTransfer ? true : false).descAr) + (x.First().InvoicesMaster.InvoiceType != x.First().InvoicesMaster.ParentInvoiceCode && !string.IsNullOrEmpty(x.First().InvoicesMaster.ParentInvoiceCode) ? " - (" + x.First().InvoicesMaster.ParentInvoiceCode + ")" : ""),
                    TransactionTypeEn = (!Transfers.Where(c => c == x.First().InvoicesMaster.InvoiceTypeId).Any() ? transactionList.Find(f => f.id == x.First().InvoicesMaster.InvoiceTypeId)?.latinName ?? "" : _IGeneralAPIsService.GetTransferDesc(x.First().InvoicesMaster.InvoiceTypeId, x.First().InvoicesMaster.StoreId, x.First().InvoicesMaster.StoreIdTo, x.First().InvoicesMaster.InvoiceSubTypesId == (int)SubType.RejectedTransfer ? true : false).descEn) + (x.First().InvoicesMaster.InvoiceType != x.First().InvoicesMaster.ParentInvoiceCode && !string.IsNullOrEmpty(x.First().InvoicesMaster.ParentInvoiceCode) ? " - (" + x.First().InvoicesMaster.ParentInvoiceCode + ")" : ""),
                    notes = x.First().InvoicesMaster.Notes,
                    invoiceTypeId = x.First().InvoicesMaster.InvoiceTypeId,
                    pageId = DocHelpers.GetPageIdOfDoc(x.First().InvoicesMaster.InvoiceTypeId),
                    pageEnum = DocHelpers.GetPageEnum(x.First().InvoicesMaster.InvoiceTypeId),
                    DocTypeId = DocHelpers.GetDocTypeId(x.First().InvoicesMaster.InvoiceTypeId),

                    parentType = x.First().InvoicesMaster.ParentInvoiceCode,
                    Serialize = x.First().InvoicesMaster.Serialize

                }).Where(x => x.Qyt != 0)
                  .ToList();

            var finalBalance = InvoicesOfItem.Sum(x => x.Qyt);
            #region Unit Valiation
            var checkIfItemHaveRequestedUnit = _invStpItemCardMasterQuery.TableNoTracking.Include(x => x.Units).Where(x => x.Id == param.itemId);
            if (checkIfItemHaveRequestedUnit.Any())
            {
                if (!checkIfItemHaveRequestedUnit.FirstOrDefault().Units.Where(x => x.UnitId == param.unitId).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.ItemUnitIsNotExist,
                        Result = Result.Failed
                    };
            }
            else
            {
                return new ResponseResult()
                {
                    Note = Actions.ItemIsNotExist,
                    Result = Result.Failed
                };
            }
            #endregion



            var PreviousBalanceInvoices = InvoicesOfItem.Where(x => x.date.Date < param.dateFrom.Date).ToList();
            var PreviousBalance = InvoicesOfItem.Where(x => x.date.Date < param.dateFrom.Date).Sum(x => x.Qyt);
            var invoicesInDate = InvoicesOfItem.Where(x => x.date.Date >= param.dateFrom.Date && x.date.Date <= param.dateTo.Date)
                                               .OrderBy(x => x.date)
                                               .ThenBy(x => x.Serialize);


            var data = new List<Data>();
            var prevBalanceElement = new Data
            {
                date = "",
                TransactionTypeAr = "  السابق",
                TransactionTypeEn = "Previous Balance",
                IncomingBalanc = 0,
                OutgoingBalanc = 0,
                Balanc = roundNumbers.GetRoundNumber(PreviousBalance)
            };
            bool isPrevBalanceAdded = true;
            double balance = 0;
            if (isPrevBalanceAdded && PreviousBalance != 0)
                data.Add(prevBalanceElement);
            foreach (var item in invoicesInDate)
            {
                if (isPrevBalanceAdded)
                {
                    balance = PreviousBalance;
                    isPrevBalanceAdded = false;
                }
                balance += item.Qyt;
                balance = roundNumbers.GetRoundNumber(balance);
                var dta = new Data()
                {
                    id = item.id,
                    date = item.date.ToString(defultData.datetimeFormat),
                    rowClassName = item.rowClassName,
                    DocumentCode = item.DocumentCode,
                    notes = item.notes,
                    TransactionTypeAr = item.TransactionTypeAr,
                    TransactionTypeEn = item.TransactionTypeEn,
                    IncomingBalanc = item.Qyt > 0 ? roundNumbers.GetRoundNumber(item.Qyt) : 0,
                    OutgoingBalanc = item.Qyt < 0 ? roundNumbers.GetRoundNumber(Math.Abs(item.Qyt)) : 0,
                    Balanc = roundNumbers.GetRoundNumber(balance),
                    Serialize = item.Serialize,
                    invoiceTypeId = item.invoiceTypeId,
                    pageId = DocHelpers.GetPageIdOfDoc(item.invoiceTypeId),
                    pageEnum = DocHelpers.GetPageEnum(item.invoiceTypeId),
                    DocTypeId = DocHelpers.GetDocTypeId(item.invoiceTypeId),
                };
                data.Add(dta);
            }

            var _InvoicesInDate = param.isPrint ? data : Pagenation<Data>.pagenationList(param.PageSize, param.PageNumber, data);



            var response = new DetailedMovementOfanItemResponse()
            {
                PreviousBalance = roundNumbers.GetRoundNumber(PreviousBalance),
                data = _InvoicesInDate
            };
            double MaxPageNumber = invoicesInDate.ToList().Count() / Convert.ToDouble(param.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var count = data.Count();
            return new ResponseResult()
            {
                Data = response,
                Result = Result.Success,
                Note = (countofFilter == param.PageNumber ? Actions.EndOfData : ""),
                DataCount = response.data.Count,
                TotalCount = count
            };
        }
    }
}
