using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier;
using App.Application.Services.Reports.Invoices.Purchases.Supplier_Account;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Purchases_transaction;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.Store.ReportsControllers
{
    public class ReportsOfStoreController : ApiStoreControllerBase
    {
        private readonly IPersonAccountService _supplierAccountService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IPersonHelperService _personHelperService;

        public ReportsOfStoreController(
                                        IPersonAccountService supplierAccountService,
                                        iAuthorizationService iAuthorizationService,
                                        IActionResultResponseHandler ResponseHandler,
                                        IPersonHelperService personHelperService) : base(ResponseHandler)
        {
            _supplierAccountService = supplierAccountService;
            _iAuthorizationService = iAuthorizationService;
            _personHelperService = personHelperService;
        }

        
    }
}
