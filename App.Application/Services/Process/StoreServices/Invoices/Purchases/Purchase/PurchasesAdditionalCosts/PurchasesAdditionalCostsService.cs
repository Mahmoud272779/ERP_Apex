using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.PurchasesAdditionalCosts
{
    public class PurchasesAdditionalCostsService : BaseClass, IPurchasesAdditionalCostsService
    {
        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> PurchasesAdditionalCostsRepositoryQuery;
        private readonly IRepositoryCommand<InvPurchasesAdditionalCosts> PurchasesAdditionalCostsRepositoryCommand;

        private readonly IRepositoryCommand<InvJobsHistory> JobHistoryRepositoryCommand;
        private readonly IRepositoryQuery<InvJobsHistory> JobHistoryRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;

        public PurchasesAdditionalCostsService(IRepositoryQuery<InvPurchasesAdditionalCosts> _purchasesAdditionalCostsRepositoryQuery,
                                   IRepositoryCommand<InvPurchasesAdditionalCosts> _purchasesAdditionalCostsRepositoryCommand,
                                   IRepositoryCommand<InvJobsHistory> _JobHistoryRepositoryCommand,
                                   IRepositoryQuery<InvJobsHistory> _JobHistoryRepositoryQuery,
                                   IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            PurchasesAdditionalCostsRepositoryQuery = _purchasesAdditionalCostsRepositoryQuery;
            PurchasesAdditionalCostsRepositoryCommand = _purchasesAdditionalCostsRepositoryCommand;
            JobHistoryRepositoryCommand = _JobHistoryRepositoryCommand;
            JobHistoryRepositoryQuery = _JobHistoryRepositoryQuery;
            httpContext = _httpContext;
        }
        public async Task<ResponseResult> AddPurchasesAdditionalCosts(PurchasesAdditionalCostsParameter parameter)
        {

            if (string.IsNullOrEmpty(parameter.ArabicName.Trim()))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData };

            if (string.IsNullOrEmpty(parameter.LatinName.Trim()))
                parameter.LatinName = parameter.ArabicName.Trim();

            parameter.LatinName = parameter.LatinName.Trim();
            parameter.ArabicName = parameter.ArabicName.Trim();

            var PurchasesAdditionExist = await PurchasesAdditionalCostsRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName || a.LatinName == parameter.LatinName);
            if (PurchasesAdditionExist == null)
            {
                int NextCode = 0;
                var PurchasesAddition = PurchasesAdditionalCostsRepositoryQuery.FindAll(q => q.PurchasesAdditionalCostsId > 0);
                if (PurchasesAddition.Count() == 0)
                {
                    NextCode = 1;
                }
                else
                    NextCode = PurchasesAdditionalCostsRepositoryQuery.GetMaxCode(e => e.Code) + 1;

                var table = Mapping.Mapper.Map<PurchasesAdditionalCostsParameter, InvPurchasesAdditionalCosts>(parameter);
                table.Code = NextCode;
                PurchasesAdditionalCostsRepositoryCommand.Add(table);
                string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                //AddJobHistory(table.JobId, table.ArabicName, table.LatinName, table.Active, table.Notes, browserName, "A", null);
                return new ResponseResult() { Data = null, Id = table.PurchasesAdditionalCostsId, Result = Result.Success };
            }
            else
            {
                return new ResponseResult() { Data = null, Id = PurchasesAdditionExist.PurchasesAdditionalCostsId, Result = Result.Exist };
            }
        }

        public async Task<ResponseResult> GetListOfPurchasesAdditionalCosts(PurchasesAdditionalCostsSearch parameters)
        {
            var resData = await PurchasesAdditionalCostsRepositoryQuery.GetAllIncludingAsync(0, 0,
                a => (a.Code.ToString().Contains(parameters.SearchCriteria) || string.IsNullOrEmpty(parameters.SearchCriteria)
                || a.ArabicName.Contains(parameters.SearchCriteria) || a.LatinName.Contains(parameters.SearchCriteria))
                && (parameters.Status == 0 || a.Status == parameters.Status));

            var count = resData.Count();
            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            return new ResponseResult() { Data = resData, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> UpdatePurchasesAdditionalCosts(UpdatePurchasesAdditionalCostsParameter parameters)
        {

            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (string.IsNullOrEmpty(parameters.ArabicName.Trim()))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData };

            if (string.IsNullOrEmpty(parameters.LatinName.Trim()))
                parameters.LatinName = parameters.ArabicName.Trim();

            parameters.LatinName = parameters.LatinName.Trim();
            parameters.ArabicName = parameters.ArabicName.Trim();

            var JobsExist = await PurchasesAdditionalCostsRepositoryQuery.GetByAsync(a => (a.ArabicName == parameters.ArabicName || a.LatinName == parameters.LatinName) && a.PurchasesAdditionalCostsId != parameters.Id);

            if (JobsExist != null)
                return new ResponseResult() { Data = null, Id = JobsExist.PurchasesAdditionalCostsId, Result = Result.Exist };

            var data = await PurchasesAdditionalCostsRepositoryQuery.GetByAsync(a => a.PurchasesAdditionalCostsId == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdatePurchasesAdditionalCostsParameter, InvPurchasesAdditionalCosts>(parameters, data);
            if (table.PurchasesAdditionalCostsId == 1)
                table.Status = (int)Status.Active;

            await PurchasesAdditionalCostsRepositoryCommand.UpdateAsyn(table);

            //string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
            //AddJobHistory(table.JobId, table.ArabicName, table.LatinName, table.Active, table.Notes, browserName, "U", null);
            return new ResponseResult() { Data = null, Id = data.PurchasesAdditionalCostsId, Result = data == null ? Result.Failed : Result.Success };

        }

        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var PurchasesAdditionalCosts = PurchasesAdditionalCostsRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.PurchasesAdditionalCostsId));
            var PurchasesAdditionalCostsList = PurchasesAdditionalCosts.ToList();

            PurchasesAdditionalCostsList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            //if (parameters.PurchasesAdditionalCostsList.Contains(1))
            //    PurchasesAdditionalCostsList.Where(q => q.PurchasesAdditionalCostsId == 1).Select(e => { e.Active = (int)Status.Active; return e; }).ToList();
            var rssult = await PurchasesAdditionalCostsRepositoryCommand.UpdateAsyn(PurchasesAdditionalCostsList);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }



        public async Task<ResponseResult> DeletePurchasesAdditionalCosts(SharedRequestDTOs.Delete ListCode)
        {
            await PurchasesAdditionalCostsRepositoryCommand.DeleteAsync(a => ListCode.Ids.Contains(a.PurchasesAdditionalCostsId));

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
        public async Task<ResponseResult> GetAllPurchasesAdditionalCostsDropDown()
        {
            var treeData = PurchasesAdditionalCostsRepositoryQuery.FindAll(q => q.Status == 1).ToList();
            var list = new List<PurchasesAdditionalCostsDto>();
            //foreach (var item in treeData)
            //{
            //    var purchasesAdditionalDto = new PurchasesAdditionalCostsDto();
            //    purchasesAdditionalDto.PurchasesAdditionalCostsId = item.PurchasesAdditionalCostsId;
            //    purchasesAdditionalDto.ArabicName = item.ArabicName;
            //    purchasesAdditionalDto.LatinName = item.LatinName;
            //    purchasesAdditionalDto.Code = item.Code;
            //    purchasesAdditionalDto.AdditionalType = item.AdditionalType;
            //    list.Add(purchasesAdditionalDto);
            //}
            var list2 = Mapping.Mapper.Map<List<InvPurchasesAdditionalCosts>, List<PurchasesAdditionalCostsDto>>(treeData);
            return new ResponseResult() { Id = null, Data = list2, DataCount = list2.Count, Result = list2.Count > 0 ? Result.Success : Result.NoDataFound, Note = "" };
        }


    }
}
