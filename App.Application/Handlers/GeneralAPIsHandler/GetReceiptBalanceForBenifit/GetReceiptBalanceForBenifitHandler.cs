using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetReceiptBalanceForBenifit
{
    public class GetReceiptBalanceForBenifitHandler : IRequestHandler<GetReceiptBalanceForBenifitRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GlReciepts> receiptQuery;
        private readonly IRoundNumbers RoundNumbers;
        private readonly IRepositoryQuery<GLCurrency> CurrencyQuery;

        public GetReceiptBalanceForBenifitHandler(IRepositoryQuery<GlReciepts> receiptQuery, IRoundNumbers roundNumbers, IRepositoryQuery<GLCurrency> currencyQuery)
        {
            this.receiptQuery = receiptQuery;
            RoundNumbers = roundNumbers;
            CurrencyQuery = currencyQuery;
        }

        public async Task<ResponseResult> Handle(GetReceiptBalanceForBenifitRequest request, CancellationToken cancellationToken)
        {
            //var setting = await GeneralSetteingCashing.GetAllGeneralSettings();
            //int decemalNum = setting.Other_Decimals;

            var benfitBalance = receiptQuery.TableNoTracking
                .Where(h => h.Authority == request.AuthorityId
                && h.BenefitId == request.BenefitID
                && h.IsBlock == false
                ).ToList();



            double total = benfitBalance.Sum(h => h.Creditor) - benfitBalance.Sum(h => h.Debtor);
            total = RoundNumbers.GetRoundNumber(total);// Numbers.Roundedvalues(total, decemalNum);
            var curency = await CurrencyQuery.TableNoTracking.Where(h => h.IsDefault == true).FirstOrDefaultAsync();
            object financialBalance = new { total, curency.ArabicName, curency.LatinName };


            return new ResponseResult() { Data = financialBalance, Result = Result.Success, Total = total };
        }
    }
}
