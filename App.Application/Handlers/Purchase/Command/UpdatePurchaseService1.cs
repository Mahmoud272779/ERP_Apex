using App.Application.Handlers.Purchase;
using App.Application.Helpers;
using App.Application.Services.HelperService;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class AddPurchaseService1 : IRequestHandler<UpdatePurchaseRequest, ResponseResult>
    {


        private SettingsOfInvoice SettingsOfInvoice;
        private IUpdateInvoice generalProcess;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;


        public AddPurchaseService1(
                              IUpdateInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> GeneralSettings
                            )
        {
            this.GeneralSettings = GeneralSettings;

            this.generalProcess = generalProcess;
        }

        public async Task<ResponseResult> Handle(UpdatePurchaseRequest request, CancellationToken cancellationToken)
        {
            if (request.InvoiceDetails == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.ItemsIsReuired };

            }
            if(request.InvoiceId==0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note =Actions.IdIsRequired };

            var setting = await GeneralSettings.SingleOrDefault(a=>a.Id==1);

            
            // validation of total result
          // get purchase which will update
            SettingsOfInvoice = new SettingsOfInvoice();

            SettingsOfInvoice.ActiveDiscount = setting.Purchases_ActiveDiscount;
            
          var res =  await generalProcess.UpdateInvoices(request,setting, SettingsOfInvoice, (int)DocumentType.Purchase, Aliases.InvoicesCode.Purchase,FilesDirectories.Purchases);
            return res;
        }

    }
}