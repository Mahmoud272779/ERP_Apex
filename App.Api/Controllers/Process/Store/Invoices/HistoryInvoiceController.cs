using App.Api.Controllers.BaseController;
using App.Application.Services.Process.Invoices;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Invoices
{
    public class HistoryInvoiceController : ApiStoreControllerBase
    {
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        public HistoryInvoiceController(IHistoryInvoiceService _HistoryInvoiceService,
        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            HistoryInvoiceService = _HistoryInvoiceService;

        }
        
        [HttpGet("GetAllInvoiceMasterHistory")]
        public async Task<ResponseResult> GetAllInvoiceMasterHistory(int Id , string? ParentInvoiceCode, int invoiceTypeId)
        {
            var result = await HistoryInvoiceService.GetAllInvoiceMasterHistory(Id , ParentInvoiceCode, invoiceTypeId);
            return result;
        }
    }
}
