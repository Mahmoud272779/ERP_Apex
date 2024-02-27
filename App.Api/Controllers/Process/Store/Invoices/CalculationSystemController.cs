using App.Api.Controllers.BaseController;
using App.Application.Services.Process.Invoices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
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
    public class CalculationSystemController: ApiStoreControllerBase
    {
        private readonly ICalculationSystemService calcSystemService;

        public CalculationSystemController(ICalculationSystemService calcSystemService,
                      IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            this.calcSystemService = calcSystemService;
        }

        /// <summary>
        /// This api used to do calculations of invoices as calculating net and total price of invoice
        /// handle validations of setting related to invoices according to invoice type 
        /// </summary>
        /// <param name="parameter"> DiscountType integer : front will determine this enum (0 , 1 or 2)  "Enums -> DiscountType" .
        /// TotalDiscountValue double : total discount value of invoice is calculated or it is splited on all items according to DiscountType.
        /// TotalDiscountRatio double : total discount ratio of invoice is calculated or it is splited on all items according to DiscountType.
        /// InvoiceTypeId      int    : front will determine this enum  "Enums -> DocumentType" use this field to do some validation according to invoice type.
        /// InvoiceId          int    : used to get settings of old invoice
        /// ParentInvoice      string : wile sent like "1-P-3" in case of update or return 
        /// PersonId           int    : get id of customer or supplier from http://192.168.1.253:8091/api/Store/Persons/GetPersonsDropDown
        ///                                to handle settings according to sent person
        /// isCopy             bool   : If we copy the invoice, we will skip the zero quantities of the serial and the expiary
        /// itemDetails  List<InvoiceDetailsAttributes> : list of items data that calculated
        ///    -> itemCode string : used to return error msg for this item.
        ///    -> ItemTypeId int  : it is sent to front in FillItemCardQuery .
        ///    -> Quantity double : quantity foreach item.
        ///    -> Price    double : price foreach item.
        ///    -> DiscountValue double : discount value entered by user .
        ///    -> DiscountRatio double : discount ratio entered by user .
        ///    -> ApplyVat bool : determine if item apply vat or not.
        ///    -> VatRatio double : vat ratio foreach item.
        /// </param>
        /// <returns> list of items with total price and splited discount if there forech item , 
        /// net , total discount and total price for all invoice</returns>
        [HttpPost(nameof(CalculationOfInvoices))]  
        public async Task<ResponseResult> CalculationOfInvoices(CalculationOfInvoiceParameter parameter)
        {
            var result = await calcSystemService.CalculationOfInvoice(parameter);
            return result;
        }


        /// <summary>
        /// check item price according to settings of invoice
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns> return error msg if price not right according to settings else return success </returns>
        [HttpGet(nameof(checkItemPrice))]
        public async Task<ResponseResult> checkItemPrice([FromQuery]checkItemPrice parameter)
        {
            var result = await calcSystemService.checkItemPrice(parameter );
            return result;
        }
    }
}
