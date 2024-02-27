using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper
{
    public static class journalEntryHelper
    {
        public static IQueryable<GLFinancialAccount> getAllAccounts(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery) => financialAccountRepositoryQuery.TableNoTracking;
        public static async Task<bool> updateAutoJoruanlsCostCenter(IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsRepositoryCommand,IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery,IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery,UpdateJournalEntryParameter parameter, GLJournalEntry JournalEntry)
        {
            var JournalEntryDetails = journalEntryDetailsRepositoryQuery.TableNoTracking.Where(x => x.JournalEntryId == JournalEntry.Id);
            if (parameter.journalEntryDetails.Any() && JournalEntryDetails.Any())
            {
                var listOfJournalEntryDetails = new List<GLJournalEntryDetails>();
                foreach (var item in JournalEntryDetails)
                {
                    if (/*item.CostCenterId != null || */item.CostCenterId != 0)
                    {
                        //check cost center exist
                        var elementCostCenterID = parameter.journalEntryDetails.Where(x => x.id == item.Id).FirstOrDefault()?.CostCenterId ?? 0;
                        if (elementCostCenterID == null || elementCostCenterID == 0)
                            continue;
                        var costCenter = costCenterRepositoryQuery.TableNoTracking.Where(x => x.Id == elementCostCenterID);
                        if (!costCenter.Any())
                            continue;
                        if (costCenter.FirstOrDefault().Type == 2)
                            continue;
                        item.CostCenterId = elementCostCenterID;
                        listOfJournalEntryDetails.Add(item);
                    }
                }
                var updated = await journalEntryDetailsRepositoryCommand.UpdateAsyn(listOfJournalEntryDetails);
                return updated;
            }
            return false;
        }
        public static async void HistoryJournalEntry(IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand,IHttpContextAccessor httpContext,iUserInformation _iUserInformation,int Code, int historyType, string lastTransactionAction)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var history = new GLRecHistory()
            {
                employeesId = userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                //LastAction = lastTransactionAction,
                Code = Code,
                BranchId = 0,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),

                BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString(),
                LastTransactionUserAr = userInfo.employeeNameAr.ToString()
            };
            recHistoryRepositoryCommand.Add(history);
        }
        public static double countTotal(int id, int financialId, List<GLJournalEntryDetails> gLJournalEntries)
        {
            var costCenterData = gLJournalEntries.Where(s => s.Id <= id).Select(x => new { x.Credit, x.Debit });
            return costCenterData.Select(x => x.Credit).Sum() - costCenterData.Select(x => x.Debit).Sum();

        }
        public static async void HistoryJournalEntry(IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand,iUserInformation _iUserInformation,int branchId, int code, int JournalEntryId, string browserName, string lastTransactionAction, string LastTransactionUser)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var history = new GLRecHistory()
            {
                employeesId = userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                BranchId = branchId,
                JournalEntryId = JournalEntryId,
                Code = code,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                BrowserName = browserName,
                LastTransactionUserAr = userInfo.employeeNameAr.ToString()
            };
            recHistoryRepositoryCommand.Add(history);
        }
        public static async Task<bool> CheckIsValidCode(IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery,int Code)
        {
            var journalEntry = await journalEntryRepositoryQuery.SingleOrDefault(
                   cust => cust.Code == Code && cust.Code != 0);
            return journalEntry == null ? false : true;
        }

    }
}
