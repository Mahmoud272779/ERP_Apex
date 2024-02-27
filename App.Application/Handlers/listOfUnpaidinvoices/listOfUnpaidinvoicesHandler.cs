using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.listOfUnpaidinvoices
{
    public class listOfUnpaidinvoicesHandler : IRequestHandler<listOfUnpaidinvoicesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRoundNumbers _roundNumbers;
        public listOfUnpaidinvoicesHandler(IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, iUserInformation iUserInformation, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRoundNumbers roundNumbers)
        {
            _InvoiceMasterQuery = invoiceMasterQuery;
            _iUserInformation = iUserInformation;
            _GlRecieptsQuery = glRecieptsQuery;
            _roundNumbers = roundNumbers;
        }
        public async Task<ResponseResult> Handle(listOfUnpaidinvoicesRequest request, CancellationToken cancellationToken)
         {
            var userInfo = await _iUserInformation.GetUserInformation();
            int[] recInvoices = { 0 };
            IQueryable<GlReciepts> recs = null;
            if(request.recId != null) 
            {
                recs = _GlRecieptsQuery.TableNoTracking.Where(c => c.MultiCollectionReceiptParentId == request.recId).Where(c => !c.IsAccredit);
                recInvoices = recs.Select(c => c.ParentId??0).ToArray();
            }
            var invoices = _InvoiceMasterQuery.TableNoTracking
                .Where(c => c.Remain > 0 || recInvoices.Any(x=> x == c.InvoiceId))
                .Where(c => request.Authority == (int)Enums.AuthorityTypes.customers ? c.InvoiceTypeId == (int)Enums.DocumentType.Sales : (request.Authority == (int)Enums.AuthorityTypes.suppliers ? c.InvoiceTypeId == (int)Enums.DocumentType.Purchase : false))
                .Where(c => request.personId != 0 ? c.PersonId == request.personId : true)
                .Where(a => a.BranchId == userInfo.CurrentbranchId);

            var totalCount = invoices.Count();
            var res = invoices.Where(c => !string.IsNullOrEmpty(request.code) ? c.InvoiceType.Contains(request.code) : true);
            var dataCount = res.Count();
            double MaxPageNumber = res.Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            res = res.Skip(((request.PageNumber ?? 0) - 1) * (request.PageSize ?? 0)).Take(request.PageSize ?? 0);

            
            var response = res.Select(c => new listOfUnpaidinvoicesResponseDTO
            {
                Id = c.InvoiceId,
                invoiceType = c.InvoiceType,
                net = c.Net,
                paid= _roundNumbers.GetRoundNumber(c.Paid - (request.recId != null ? (recs.Where(x => x.ParentId == c.InvoiceId).Count()>0? recs.FirstOrDefault(x => x.ParentId == c.InvoiceId).Amount :0) : 0)),
                remain= _roundNumbers.GetRoundNumber(c.Remain + (request.recId != null ? (recs.Where(x => x.ParentId == c.InvoiceId).Count() > 0 ? recs.FirstOrDefault(x => x.ParentId == c.InvoiceId).Amount : 0) : 0))
                //paid = _roundNumbers.GetRoundNumber(c.Paid - (request.recId != null? recs.FirstOrDefault(x=> x.ParentId == c.InvoiceId).Amount : 0)),
                //remain = _roundNumbers.GetRoundNumber(c.Remain - (request.recId != null ? recs.FirstOrDefault(x => x.ParentId == c.InvoiceId).Amount : 0))
            });
            
            return new ResponseResult
            {
                Data = response,
                DataCount = dataCount,
                TotalCount = totalCount,
                Result = Result.Success,
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : "")
            };
        }
    }
    public class listOfUnpaidinvoicesResponseDTO
    {
        public int Id { get; set; }
        public string invoiceType { get; set; }
        public double net { get; set; }
        public double paid { get; set; }
        public double remain { get; set; }
    }
}
