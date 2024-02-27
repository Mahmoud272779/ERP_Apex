using App.Application.Helpers;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Reposiotries.Configuration;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application.Services.Process.StoreServices
{
    public class HistoryReceiptsService : BaseClass, IHistoryReceiptsService
    {
        private readonly IRepositoryCommand<GLRecieptsHistory> ReceiptsHistoryRepositoryCommand;
        private readonly IRepositoryQuery<GLRecieptsHistory> ReceiptsHistoryRepositoryQuery;
        private readonly IRepositoryQuery<InvEmployees> EmployeeRepositoryQuery;
        private readonly IRepositoryQuery<userAccount> UseraccountQuery;
        private readonly IHttpContextAccessor httpContext;

        public HistoryReceiptsService(
                              IRepositoryCommand<GLRecieptsHistory> receiptsHistoryRepositoryCommand,
                              IRepositoryQuery<GLRecieptsHistory> receiptsHistoryRepositoryQuery,
                              IRepositoryQuery<InvEmployees> employeeRepositoryQuery,
        IRepositoryQuery<userAccount> useraccountQuery,

        IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            ReceiptsHistoryRepositoryQuery = receiptsHistoryRepositoryQuery;
            ReceiptsHistoryRepositoryCommand = receiptsHistoryRepositoryCommand;
            httpContext = _httpContext;
            UseraccountQuery = useraccountQuery;
            EmployeeRepositoryQuery = employeeRepositoryQuery;
        }
        public async void AddReceiptsHistory(int BranchId, int BenefitId,
             string ReceiptsAction, int PaymentMethodId,
             int UserId, int SafeIDOrBank, int Code,
             DateTime RecieptDate, int ReceiptsId, string RecieptType,
             int RecieptTypeId, int Signal, bool IsBlocked,
             bool IsAccredit, double Serialize, int AuthorityType, double Amount, int subTypeId
            , UserInformationModel userInfo)

        {

            var history = new GLRecieptsHistory()
            {
                employeesId = userInfo.employeeId,
                SubTypeId = subTypeId,
                ReceiptsId = ReceiptsId,
                SafeIDOrBank = SafeIDOrBank,
                Code = Code,
                RecieptDate = RecieptDate,
                Amount = Amount,
                AuthorityType = AuthorityType,
                PaymentMethodId = PaymentMethodId,
                RecieptTypeId = RecieptTypeId,
                RecieptType = RecieptType,
                Signal = Signal,
                Serialize = Serialize,
                BranchId = BranchId,
                UserId = UserId,
                IsAccredit = IsAccredit,
                BenefitId = BenefitId,
                IsBlock = IsBlocked,
                ReceiptsAction = ReceiptsAction,
                LastDate = DateTime.Now,
                BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString()),
                isTechnicalSupport = userInfo.isTechincalSupport
            };
            ReceiptsHistoryRepositoryCommand.Add(history);
            await ReceiptsHistoryRepositoryCommand.SaveAsync();

        }



        public async Task<ResponseResult> GetAllReceiptsHistory(int receiptsId)
        {

            var Data = ReceiptsHistoryRepositoryQuery.TableNoTracking.Where(s => s.ReceiptsId == receiptsId).Include(a => a.employees).ToList();

            var historyList = new List<HistoryResponceDto>();

            foreach (var item in Data)
            {
                // var emp = EmployeeRepositoryQuery.TableNoTracking.Where(h => h.Id == item.UserId).FirstOrDefault();
                var historyDto = new HistoryResponceDto();
                historyDto.Date = item.LastDate.Value;
                HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.ReceiptsAction];
                historyDto.TransactionTypeAr = actionName.ArabicName;
                historyDto.TransactionTypeEn = actionName.LatinName;
                if (item.isTechnicalSupport)
                {
                    historyDto.ArabicName = HistoryActions.TechnicalSupportAr;
                    historyDto.LatinName = HistoryActions.TechnicalSupportEn;
                }
                else
                {
                    historyDto.ArabicName = item.employees.ArabicName.ToString();
                    historyDto.LatinName = item.employees.LatinName.ToString();
                }
                historyDto.BrowserName = item.BrowserName;
                if (item.SubTypeId == (int)SubType.CollectionReceipt )
                {
                    historyDto.TransactionTypeAr += " " +  NotesOfReciepts.CollectionReceiptsAr;
                    historyDto.TransactionTypeEn += " " + NotesOfReciepts.CollectionReceiptsEn;
                }
                if (item.SubTypeId == (int)SubType.PaidReceipt)
                {
                    historyDto.TransactionTypeAr += " " + NotesOfReciepts.PaidReciptsAr;
                    historyDto.TransactionTypeEn += " " +  NotesOfReciepts.PaidReciptsEn;
                }
                ///historyDto.LastTransactionAction = specifyHistoryAction(item.ReceiptsAction);
                historyList.Add(historyDto);

            }

            return new ResponseResult() { Data = historyList, Id = null, Result = Result.Success };
        }
    }
}
