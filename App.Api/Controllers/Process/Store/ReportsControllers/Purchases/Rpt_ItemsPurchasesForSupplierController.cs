using App.Api.Controllers.BaseController;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.ReportsControllers.PurchasesControllers
{
    public class Rpt_ItemsPurchasesForSupplierController : ApiControllerBase
    {
        private readonly IItemsPurchasesForSupplierService ItemsPurchasesForSupplier;
        public Rpt_ItemsPurchasesForSupplierController(IItemsPurchasesForSupplierService ItemsPurchasesForSupplier,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.ItemsPurchasesForSupplier = ItemsPurchasesForSupplier;
        }



        [AllowAnonymous]
        [HttpGet("GetItemsPurchasesForSupplierData1")]
        public async Task<ResponseResult> GetItemsPurchasesForSupplierData1(int supplierId ,int InvoiceTypeId,int PaymentTypeId, [FromQuery] int[] branches , DateTime dateFrom , DateTime dateTo)
        {
            ItemsPurchasesForSupplierRequest request = new ItemsPurchasesForSupplierRequest()
            {
                SupplierId = supplierId ,
                InvoiceTypeId= InvoiceTypeId,
                PaymentTypeId= PaymentTypeId,
                Branches = branches,
                DateFrom=dateFrom,
                DateTo=dateTo
            };
            var result = await ItemsPurchasesForSupplier.GetItemsPurchasesForSupplierData(request);
            return result;
        }
    }
}
