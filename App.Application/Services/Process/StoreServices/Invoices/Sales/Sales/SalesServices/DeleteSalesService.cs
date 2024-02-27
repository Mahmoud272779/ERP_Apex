using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.SalesServices
{
    public  class DeleteSalesService:BaseClass, IDeleteSalesService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery;
 
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery;
        private readonly IDeleteInvoice GeneralProcessDelete;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;

        public DeleteSalesService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery,
                               IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsQuery,
                              IDeleteInvoice generalProcessDelete, IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterQuery = _InvoiceMasterQuery;
             httpContext = _httpContext;
            GeneralProcessDelete = generalProcessDelete;
            InvGeneralSettingsQuery = _InvGeneralSettingsQuery;
            this.InvoiceMasterRepositoryCommand = InvoiceMasterRepositoryCommand;
        }
        public async Task<ResponseResult> DeleteSales(SharedRequestDTOs.Delete parameter)
        {

          /*  var invoicesMaster = InvoiceMasterQuery.TableNoTracking
                        .Where(e => parameter.Ids.Contains(e.InvoiceId)).ToList();

            // update old invoice , change isdelete = true
            invoicesMaster.Select(e => { e.IsDeleted = true; return e; }).ToList();
            await InvoiceMasterRepositoryCommand.UpdateAsyn(invoicesMaster);

            //  var updateOldMaster = await InvoiceMasterRepositoryCommand.UpdateAsyn(invoicesMaster);

            var setting = await InvGeneralSettingsQuery.GetByAsync(q => 1 == 1);*/
         var res =   await GeneralProcessDelete.DeleteInvoices(parameter.Ids.First(), (int)DocumentType.DeleteSales, Aliases.InvoicesCode.DeleteSales, (int)DocumentType.Sales);
            return res; // new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }

        public async Task<ResponseResult> DeleteInvoiceForPOS(SharedRequestDTOs.Delete parameter)
        {
            var res = await GeneralProcessDelete.DeleteInvoices(parameter.Ids.First(), (int)DocumentType.DeletePOS, Aliases.InvoicesCode.DeletePOS, (int)DocumentType.POS);
            return res; 
        }
    }
}
