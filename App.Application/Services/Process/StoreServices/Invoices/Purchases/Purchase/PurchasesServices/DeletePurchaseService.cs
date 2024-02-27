using System;
using System.Linq;
using System.Threading.Tasks;
using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class DeletePurchaseService : BaseClass, IDeletePurchaseService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;

        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IDeleteInvoice GeneralProcessDelete;
        private readonly IHttpContextAccessor httpContext;

        public DeletePurchaseService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IRepositoryCommand<InvoiceMaster> _InvoiceMasterRepositoryCommand,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IDeleteInvoice generalProcessDelete, 
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvoiceMasterRepositoryCommand = _InvoiceMasterRepositoryCommand;
            httpContext = _httpContext;
            GeneralProcessDelete = generalProcessDelete;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
        }
        public async Task<ResponseResult> DeletePurchase(SharedRequestDTOs.Delete parameter , int invoiceTypeId)
        {

            string InvoiceTypeName = Aliases.InvoicesCode.DeletePurchase;
            int parentInvoiceTypeId = (int)DocumentType.Purchase;
             if (invoiceTypeId == (int)DocumentType.DeleteWov_purchase)
            {
                InvoiceTypeName = Aliases.InvoicesCode.DeleteWOV_Purchase;
                parentInvoiceTypeId = (int)DocumentType.wov_purchase;
            }
            var res=   await  GeneralProcessDelete.DeleteInvoices(parameter.Ids.First(),  invoiceTypeId, InvoiceTypeName , parentInvoiceTypeId);
            return res;// new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
    }
}