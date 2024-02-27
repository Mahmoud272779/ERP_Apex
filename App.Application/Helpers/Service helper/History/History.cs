using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.Reposiotries.Configuration;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;
//using static FluentValidation.Validators.PredicateValidator<T, TProperty>;

namespace App.Application.Helpers.Service_helper.History
{
    public class History<T> : IHistory<T> where T : class
    {

        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryCommand<T> repositoryCommand;
        private readonly IRepositoryQuery<T> repositoryQuery;
        private readonly iUserInformation _iUserInformation;
      //  private readonly IRepositoryQuery<InvFundsBanksSafesHistory> FunsBankHistoryRepositoryQuery;


        public History(IHttpContextAccessor httpContext,
             IRepositoryCommand<T> repositoryCommand, IRepositoryQuery<T> repositoryQuery, iUserInformation iUserInformation)//, IRepositoryQuery<InvFundsBanksSafesHistory> funsBankHistoryRepositoryQuery)
        {
            this.httpContext = httpContext;
            this.repositoryCommand = repositoryCommand;
            this.repositoryQuery = repositoryQuery;
            _iUserInformation = iUserInformation;
       //     FunsBankHistoryRepositoryQuery = funsBankHistoryRepositoryQuery;
        }

        public async Task<bool> AddHistory(int EntityId, string LatinName, string ArabicName, string lastTransactionAction, string addTransactionUser)
        {
            bool result = false;
            var userInfo = await _iUserInformation.GetUserInformation();
            var entity = new HistoryParameter()
            {
                employeesId = userInfo.employeeId,
                BranchId = userInfo.CurrentbranchId,
                EntityId = EntityId,
                ArabicName = ArabicName,
                LatinName = LatinName,
                LastTransactionAction = lastTransactionAction,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                LastTransactionUserAr = userInfo.employeeNameAr.ToString(),
                LastTransactionDate = DateTime.Now.ToString(),
                AddTransactionDate = DateTime.Now.ToString(),
                AddTransactionUser = addTransactionUser,
                BrowserName = userInfo.browserName.ToString(),
                isTechnicalSupport=userInfo.isTechincalSupport
            };

            var table = Mapping.Mapper.Map<HistoryParameter, T>(entity);
           
            repositoryCommand.Add(table);
            result = true;

            return result;
        }

        public async Task<ResponseResult> GetHistory(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var userInfo = await _iUserInformation.GetUserInformation();
                if (!userInfo.otherSettings.showHistory)
                    return new ResponseResult()
                    {
                        Note = Actions.YouCantViewTheHistory,
                        Result = Result.Failed
                    };
            }
            catch (Exception e)
            {

                throw e;
            }
         
            var historyList = new List<HistoryResponceDto>();
          //  var prediacte= Expression < Func<T, bool> > 
            var data =await repositoryQuery.FindByAsyn(predicate);
            var dataList = data.ToList();

           // var res = await repositoryQuery.FindByAsyn(predicate);
            var res = Mapping.Mapper.Map<List<T>, List<HistoryDto>>(dataList);


            foreach (var item in res.ToList())
            {
                
                var historyDto = new HistoryResponceDto();
                historyDto.Date = DateTime.Parse(item.LastTransactionDate !=null? item.LastTransactionDate: "01/01/0001 00:00:00 AM");

                var emp = _iUserInformation.GetUserInformationById(item.employeesId);
                if(item.isTechnicalSupport)
                {
                    historyDto.ArabicName = HistoryActions.TechnicalSupportAr;
                    historyDto.LatinName = HistoryActions.TechnicalSupportEn;
                }
                else
                {
                    historyDto.ArabicName = emp.employeeNameAr.ToString();
                    historyDto.LatinName = emp.employeeNameEn.ToString();
                }
                
               
                if (item.LastTransactionAction == Aliases.HistoryActions.Update)
                {
                    historyDto.TransactionTypeAr = "تعديل";
                    historyDto.TransactionTypeEn = "Update";
                }
               else if (item.LastTransactionAction == Aliases.HistoryActions.Add)
                {
                    historyDto.TransactionTypeAr = "اضافة";
                    historyDto.TransactionTypeEn = "Add";

                }
               else if (item.LastTransactionAction == Aliases.HistoryActions.Delete)
                {
                    historyDto.TransactionTypeAr = "حذف";
                    historyDto.TransactionTypeEn = "Delete";
                }
                else if (item.LastTransactionAction == Aliases.HistoryActions.Print)
                {
                    historyDto.TransactionTypeAr = "طباعة";
                    historyDto.TransactionTypeEn = "Print";
                }
                else if (item.LastTransactionAction == Aliases.HistoryActions.ExportToPdf)
                {
                    historyDto.TransactionTypeAr = "تصدير";
                    historyDto.TransactionTypeEn = "Export";
                }
                else
                {
                    historyDto.TransactionTypeAr = item.LastTransactionAction;
                    historyDto.TransactionTypeEn = item.LastTransactionAction;
                }
                //historyDto.userNameAr = item.LastTransactionUser;
                //historyDto.userNameEn = item.LastTransactionUser;

                if (item.BrowserName.Contains("Chrome"))
                {
                    historyDto.BrowserName = "Chrome";
                }
                else if (item.BrowserName.Contains("Firefox"))
                {
                    historyDto.BrowserName = "Firefox";
                }
               else if (item.BrowserName.Contains("Opera"))
                {
                    historyDto.BrowserName = "Opera";
                }
               else if (item.BrowserName.Contains("InternetExplorer"))
                {
                    historyDto.BrowserName = "InternetExplorer";
                }
               else if (item.BrowserName.Contains("Microsoft Edge"))
                {
                    historyDto.BrowserName = "Microsoft Edge";
                }
                historyList.Add(historyDto);
            }


            return new ResponseResult() { Data = historyList, DataCount = historyList.Count, Result = Result.Success, Note = "Ok" };
        }
    }


}
