using System.Linq;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.Profit;
using App.Application.Handlers.Profit.profitPages;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.Branches;
using App.Application.Services.Process.Category;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process
{
    public class TestProfitController : ApiStoreControllerBase
    {
        private readonly ICategoryService CategoriesService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLBranch> _branchBusiness;
        private readonly IMediator _mediator;
        private readonly IPrepareDataForProfit _prepareDataForProfit;


        public TestProfitController(
           iUserInformation userInformationModel,
           IRepositoryQuery<GLBranch> branchBusiness,
            IMediator mediator,
            IPrepareDataForProfit prepareDataForProfit) : base(mediator)
        {
            _iUserInformation = userInformationModel;
            _branchBusiness = branchBusiness;
            _prepareDataForProfit = prepareDataForProfit;
        }

        [HttpGet("startCalculateProfit")]
        public async Task<ResponseResult> startCalculateProfit()
        {

            var res = await QueryAsync<ResponseResult>(new profitPagesRequest());
            return res;
        }
        [HttpGet("startCalculateProfitJournalEntry")]
        public async Task<ResponseResult> startCalculateProfitJournalEntry()
        {

            var res = await QueryAsync<ResponseResult>(new profitPagesRequest { calcJournal = true});
            return res;
        }

        [HttpGet("CalculateProfitProcess")]
        public async Task<ResponseResult> CalculateProfitProcess([FromQuery] bool calcProfit = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var data =
            _branchBusiness.TableNoTracking
            .Where(e => userInfo.employeeBranches.Contains(e.Id) && e.Status == (int)Status.Active)
            .Select(a => a.Id).ToList();

            var result = await _prepareDataForProfit.PreparingDataForProfit(0, calcProfit);

            return result;
        }

    }
}
