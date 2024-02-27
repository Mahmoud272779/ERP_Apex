using System;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.GeneralLedger.FinancialAccounts;
using App.Application.Handlers.GeneralLedger.FinancialAccounts.GetAllFinancialAccountDropDown2;
using App.Application.Handlers.GeneralLedger.FinancialAccounts.RecodingFinancialAccount;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.Invoices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    /// <remarks>
    /// شجرة الحسابات العامة
    /// !</remarks>
    public class FinancialAccountController : ApiGeneralLedgerControllerBase
    {
        private readonly IFinancialAccountBusiness financialAccountBusiness;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;

        public FinancialAccountController(/*IFinancialAccountBusiness FinancialAccountBusiness*/ iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler,
IMediator mediator) :
            base(ResponseHandler)
        {
            //financialAccountBusiness = FinancialAccountBusiness;
            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GenerateAutomaticCode")]
        public async Task<string> GenerateAutomaticCode()
        {
            var add = await _mediator.Send(new AddAutomaticCodeRequest());
            return add;
        }
        /// <remarks>
        /// This is API for creating a new financial Account in the GL Tree<br/>
        /// to use this api you have to insert some of the data :<br/>
        /// 1-LatinName as string field<br/>
        /// 2-ArabicName as string field<br/>
        /// 3-AccountCode as string field this field is optinal you use it only when you use auto coding<br/>
        /// 4-CurrencyId as int field this is the id of the currency to use you can get it from http://gl.ttechapps.info/api/GeneralLedger/Currency/GetAllCurrency API<br/>
        /// 5-Status as int field this is the value of the statues 1 for active and 2 for deactivate the account <br/>
        /// 6-FA_Nature as int field this is the value of the FinancialAccount Nature as Depit or Credit use 1 for Depitor and 2 for creditor<br/>
        /// 7-FinalAccount as int field this is the value of the final account use 1 for budget "ميزانية" and 2 for income statement "قائمة الدخل"<br/>
        /// 8-ParentId as int field has self relationship with table his self we use it only when we add a child account and if we add a main account we set it as Empty value <br/>
        /// 9-Notes as string field use it to add notes for the account<br/>
        /// 10-IsMain as boolean field use it as true to set the account as main account to be folder for subs account or false to make it sub account <br/>
        /// 11-BranchesId as array of int we use to to set the branche  that will use this account<br/>
        /// 12-CostCenterId as array of int we use it to set the cost centers of this account we<br/>
        /// Created by Ahmed Atif<br/>
        /// !</remarks>
        [HttpPost(nameof(CreateNewFinancialAccount))]
        public async Task<IRepositoryResult> CreateNewFinancialAccount([FromBody] AddFinancialAccountRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Add);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }



        /// <remarks>
        ///  This is API for updating and edit the information of that accountant account in the GL Tree <br/>
        ///  to use this API you have to insert some of the data : <br/>
        ///  1-LatinName as string field to edit the english account name<br/>
        ///  2-ArabicName as string field to edit the arabic account name <br/>
        ///  3-CurrencyId as int field to edit the currency of the account<br/>
        ///  4-Status as int field to edit the statues of the account statues 1 for active and 2 for deactivate the account <br/>
        ///  5-FA_Nature as int field to eidt the nature of the account Nature as Depit or Credit use 1 for Depitor and 2 for creditor<br/>
        ///  6-FinalAccount as int field to eidt the final account use 1 for budget "ميزانية" and 2 for income statement "قائمة الدخل"<br/>
        ///  7-ParentId as int field to edit the parent id of the the current account this  field has self relationship with table his self we use it only when we add a child account and if we add a main account we set it as Empty value <br/>
        ///  8-Notes as string field use it to add or edit notes for the account<br/>
        ///  9-IsMain as boolean field use it as true to set the account as main account to be folder for subs account or false to make it sub account <br/>
        ///  10-CostCenterId as array of int use this to add or edit cost centers of for the account <br/> 
        ///  11-BranchesId as array of int use this to add or edit the branchs that will use this account
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpPut(nameof(UpdateFinancialAccount))]
        public async Task<IRepositoryResult> UpdateFinancialAccount(UpdateFinancialAccountRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Edit);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        /// <remarks>
        /// This is the API for editing the Statues of multi Financial Account<br/>
        /// to use this api you have to inset some data :<br/>
        /// 1- Id as array of int  this is the id of the financial Accounts that you want to change the statues of it<br/>
        /// 2-Status as int field this the statues of the account 1 for active and 2 for deactivate the account <br/>
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpPut(nameof(UpdateFinancialAccountsStatus))]
        public async Task<IRepositoryResult> UpdateFinancialAccountsStatus(UpdateStatusRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Edit);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        /// <remarks>
        /// This is API for deleting multi Financial Accounts <br/>
        /// to use this API you have to insert Financial Accounts ids as array of int you can just delete one account by only insrt one id in the array<br/>
        /// Note:   make sure the account is not used before deleteing because this api wont work if you send one account used in the system
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpDelete("DeleteFinancialAccount")]
        public async Task<IRepositoryResult> DeleteFinancialAccount([FromBody] int[] Ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Delete);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            {
                Ids = Ids
            };
            var Delete = await _mediator.Send(new DeleteFinancialAccountRequest { Ids = Ids });
            var result = ResponseHandler.GetResult(Delete);
            return result;
        }
        /// <remarks>
        /// This API for getting financial Account by id to use this api you have to send the id as query parm 
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpGet("GetFinancialAccountById/{id}")]
        public async Task<IRepositoryResult> GetFinancialAccountById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await _mediator.Send(new GetFinancialAccountByIdRequest { Id = id });
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        /// <remarks>
        /// This is the API to getting all finacial Account fot the GL accountant tree<br/>
        /// to use this API you have to insrt some Data:<br/>
        /// 1-FA_Nature as int field its optional field use it to choose the statues of the accounts you wanna get to get all Accounts insert 0 to get activated Accounts insert 1 to get deactivated Accounts insert 2<br/>
        /// FA_Nature  as int field its optional field use it to choose the nature of the accounts you wanna get to get all Accounts insert 0 to get activated accounts insert 1 to get deactivated accounts insert 2<br/>
        /// CostCenter as int field its optional field use it to select the accounts by the cost center id<br/>
        /// SearchCriteria as string field and its optional use it to search in accounts by name arabic or english or account code
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpGet("GetAllFinancialAccount")]
        public async Task<IRepositoryResult> GetAllFinancialAccount([FromQuery] GetAllFinancialAccountRequest parameters)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            //   parameters.CostCenter = (CostCenter == 1 ? true : false);

            var account = await _mediator.Send(parameters);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllFinancialAccountForOpeningBalance")]
        public async Task<IRepositoryResult> GetAllFinancialAccountForOpeningBalance()
        {
            var account = await _mediator.Send(new GetAllFinancialAccountForOpeningBalanceRequest());
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetAllFinancialAccountDropDown")]
        public async Task<IRepositoryResult> GetAllFinancialAccountDropDown()
        {
            var account = await _mediator.Send(new GetAllFinancialAccountDropdownRequest());
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        /// <remarks>
        /// This API to get Financial Account History, To use it just send the id of the account as query parm
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpGet("GetAllFinancialAccountHistory/{id}")]
        public async Task<ResponseResult> GetAllFinancialAccountHistory(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            // return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await _mediator.Send(new GetAllFinancialAccountHistoryRequest { id = id });
            //var result = ResponseHandler.GetResult(repositoryActionResult: account.Data);
            return account;
        }

        [HttpGet("GetAllFinancialAccountWihOutChilds")]
        public async Task<IRepositoryResult> GetAllFinancialAccountWihOutChilds()
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await _mediator.Send(new getAllFinancialAccountDropDownRequest());
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        /// <remarks>
        /// This API to move the Account in the tree by drag and drop<br/>
        /// to use this api you need to send some data:<br/>
        /// 1-FinancialIdWillMove as int field its requred and this is the id of the account you wanna move<br/>
        /// 2-FinancialIdMovedTo as int field it requred and this is the id of the account you wanna move to<br/>
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpPost(nameof(MoveFinancialAccount))]
        public async Task<IRepositoryResult> MoveFinancialAccount(MoveFinancialAccountRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Edit);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        /// <remarks>
        /// This API to get drop down list of financial accounts<br/>
        /// to use this api you need to send some data:<br/>
        /// 1-SearchCriteria as string field its optional use it to find account by arabic name or english name or account code<br/>
        /// 2-PageSize as int field it requred this field to get the size of the page<br/>
        /// 3-PageNumber as int field it requred this field to get the number of the page<br/>
        /// 4-isMain as boolean field it optional this field to get main or sub account true for main false for sub<br/>
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpGet("GetFinancialAccountDropDown")]
        public async Task<IActionResult> GetFinancialAccountDropDown(string? SearchCriteria, int PageSize, int PageNumber, bool? isMain,int costCenterId = 0)
        {
            DropDownRequestForGL request = new DropDownRequestForGL()
            {
                SearchCriteria = SearchCriteria,
                PageNumber = PageNumber,
                PageSize = PageSize,
                isMain = isMain
            };
            var account = await _mediator.Send(new GetFinancialAccountDropDownRequest { isMain = isMain, SearchCriteria = SearchCriteria, PageNumber = PageNumber, PageSize = PageSize, costCenterId = costCenterId });
            if (account.Result == Result.Success)
            {
                return Ok(account);
            }
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, account);


        }

        /// <remarks>
        /// This API to to recoding financial account this api using the Auto coding column in database to recoding the accounts code<br/>
        /// to use this api you need to just excute the api and it will auto recoding the accounts code<br/>
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpPost(nameof(RecodingFinancialAccount))]
        public async Task<IRepositoryResult> RecodingFinancialAccount()
        {
            var account = await _mediator.Send(new RecodingFinancialAccountRequest());
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        /// <remarks>
        /// This API to to the account information<br/>
        /// to use this api you need to send the id of the account as query parm and excute the api<br/>
        /// Created by Ahmed Atif<br/>
        /// </remarks>
        [HttpGet(nameof(GetAccountInformation) + "/{id}")]
        public async Task<IActionResult> GetAccountInformation(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.CalculationGuide_GL, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var account = await _mediator.Send(new GetAccountInformationRequest { id = id });
            if (account.Result == Result.Success)
                return Ok(account);
            else
                return StatusCode(StatusCodes.Status422UnprocessableEntity, account);
        }
    }
}
