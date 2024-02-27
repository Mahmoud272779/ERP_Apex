using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Purchases_transaction;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.ReportsControllers.PurchasesControllers
{
    public class Rpt_purchasesTransactionController : ApiControllerBase
    {
        private readonly IpurchasesTransactionService IpurchasesTransaction;
        public Rpt_purchasesTransactionController(IpurchasesTransactionService IpurchasesTransaction ,
                                            IActionResultResponseHandler ResponseHandler)
                                                       : base(ResponseHandler)
        {
            this.IpurchasesTransaction = IpurchasesTransaction;
        }
        [AllowAnonymous]
        [HttpGet("PurchasesTransaction1")]
        public async Task<ResponseResult> PurchasesTransaction1(int? supplierId ,[FromQuery] int[] branches , int? paymentMethod , DateTime dateFrom, DateTime dateTo)
        {
            var request = new purchasesTransactionRequest()
            {
                supplierId = supplierId,
                branches = branches,
                paymentMethod = paymentMethod,
                dateFrom = dateFrom,
                dateTo = dateTo
            };
            var result = await IpurchasesTransaction.PurchasesTransaction(request);
            return result;
        }

    }
}
