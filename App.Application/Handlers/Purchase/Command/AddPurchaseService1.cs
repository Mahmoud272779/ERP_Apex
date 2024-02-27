using App.Application.Handlers.Purchase;
using App.Application.Handlers.Transfer;
using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public class AddPurchaseService1 : IRequestHandler<AddPurchaseRequest, ResponseResult>
    {

    
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;
         private IAddInvoice generalProcess;
       
        public AddPurchaseService1(
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery 
                            )
        {
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
         
            this.generalProcess = generalProcess;
        }
      
        public async Task<ResponseResult> Handle(AddPurchaseRequest request, CancellationToken cancellationToken)
        {
            try
            {
               
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);
                
           
                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Purchases_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Purchases_PriceIncludeVat,
                    setDecimal=setting.Other_Decimals,
                };

                var res = await generalProcess.SaveInvoice(request, setting, SettingsOfInvoice, (int)DocumentType.Purchase, Aliases.InvoicesCode.Purchase,0, FilesDirectories.Purchases);

                return  res;
            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }
    }
}
