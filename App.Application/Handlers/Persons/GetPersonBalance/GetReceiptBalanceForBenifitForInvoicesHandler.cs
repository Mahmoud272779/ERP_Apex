using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Persons.GetPersonBalance
{
    public  class GetReceiptBalanceForBenifitForInvoicesHandler:IRequestHandler<GetReceiptBalanceForBenifitForInvoicesRequest,ResponseResult>
    {
        private readonly IRepositoryQuery<GlReciepts> receiptQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _generalSettings;
        private readonly iUserInformation _iUserInformation;


        public GetReceiptBalanceForBenifitForInvoicesHandler(IRepositoryQuery<GlReciepts> receiptQuery, IRepositoryQuery<InvGeneralSettings> generalSettings, iUserInformation iUserInformation)
        {
            this.receiptQuery = receiptQuery;
            _generalSettings = generalSettings;
            _iUserInformation = iUserInformation;
        }
        public async Task<ResponseResult> Handle(GetReceiptBalanceForBenifitForInvoicesRequest request, CancellationToken cancellationToken)
        {


            var BenefitID = request.persons.Select(a => a.Id);
            try
            {
                var benfitBalance = receiptQuery.TableNoTracking
             .Where(h => h.Authority == request.AuthorityId
             && BenefitID.Contains(h.BenefitId)
              && h.IsBlock == false && h.BranchId==request.BranchId
             ).GroupBy(a => a.BenefitId).Select(a => new { BenefitId = a.Key, Creditor = a.Sum(q => q.Creditor), Debtor = a.Sum(q => q.Debtor) });
                // var tt = benfitBalance.ToList().GroupBy(a => a.BenefitId);

                benfitBalance.ToList().ForEach(a => request.persons.Where(e => e.Id == a.BenefitId).Select(e => {
                    e.balance =  Math.Abs((a.Creditor - a.Debtor));
                    e.Creditor = Math.Abs((a.Creditor));
                    e.Debtor = Math.Abs((a.Debtor));
                    e.isCreditor = ((a.Creditor - a.Debtor) > 0 ? true : false); return e;
                }).ToList());
                

                return new ResponseResult() { Data =request.fromGetInvoice? request.persons.First(): request.persons, Result = Result.Success };
            }
            catch (Exception ex)
            {

              return new ResponseResult() { Data=ex.Message ,Result=Result.Failed};
            }
        }
    }
}
