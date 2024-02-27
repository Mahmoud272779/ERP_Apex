using App.Application.Basic_Process;
using App.Application.Services.Printing.InvoicePrint;
using App.Domain.Entities.Process;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;

using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.EMMA;
using System.Linq;

using System.Threading.Tasks;
using static App.Application.IncomeListAndBudget;
using static App.Domain.Enums.Enums;


namespace App.Application
{
    public class BudgetReportService : BusinessBase<GLFinancialAccount>, IBudgetReportService
    {
        private readonly IIncomeListAndBudget incomeListAndBudget;
        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRoundNumbers _IRoundNumbers;
        private readonly IGeneralPrint _iGeneralPrint;


        public BudgetReportService(
       IIncomeListAndBudget incomeListAndBudget,
            IRepositoryActionResult repositoryActionResult,
            IPrintService iprintService,
            IFilesMangerService filesMangerService,
            ICompanyDataService companyDataService,
            iUserInformation iUserInformation,
            IRoundNumbers iRoundNumbers,
            IGeneralPrint iGeneralPrint) : base(repositoryActionResult)
        {
            this.incomeListAndBudget = incomeListAndBudget;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _IRoundNumbers = iRoundNumbers;
            _iGeneralPrint = iGeneralPrint;
        }
        public async Task<ResponseResult> getAllDataBudgetById(IncomeListSearchParameter parametr, int ID)
        {
            var data = await incomeListAndBudget.getAllDataIncomeinListAndBudgetById(parametr, ID);

            return new ResponseResult() { Data = data.IncomingData, Result = Result.Success };
        }

        public async Task<ResponseResult> getTopLevelBudget(IncomeListSearchParameter parameter)
        {
            var res = await incomeListAndBudget.getTopLevelIncomingListAndBudget(parameter); //list of balanced data 
            var TopLevelOfCredit = res.IncomingData.Where(a => a.ParentId == null && a.accountnNatural == (int)FA_Nature.Credit).ToList();
            //TopLevelOfCredit.ForEach(x=>
            //{
            //    x.
            //})
            var TopLevelOfDebit = res.IncomingData.Where(a => a.ParentId == null && a.accountnNatural == (int)FA_Nature.Debit).ToList();

            var childCredit = res.IncomingData.Where(h => TopLevelOfCredit.Select(a => a.AutoCode).Any(s => h.AutoCode.StartsWith(s)));
            var childDebit = res.IncomingData.Where(h => TopLevelOfDebit.Select(a => a.AutoCode).Any(s => h.AutoCode.StartsWith(s)));
            //var EndTermMerchandise = incomeListAndBudget.SumEndTermMerchandis(parameter.To.Date); // عندد استخدام تقييم المخزون 
            var EndTermMerchandise = 0;//عند عدم استخدام تقييم المخزون 
            if (EndTermMerchandise > 0)
                EndTermMerchandise = EndTermMerchandise * -1;
            childDebit.ToList().ForEach(x =>
            {
                if (x.Id == (int)accountantTree.FinanicalAccountDefultIds.assets || x.Id == (int)accountantTree.FinanicalAccountDefultIds.longTermOpponents)
                    x.AccountBalance = _IRoundNumbers.GetRoundNumber(x.AccountBalance + EndTermMerchandise);
            });

            var totalBalanceOfCredit = _IRoundNumbers.GetRoundNumber(childCredit.Where(h => h.ParentId == null).Sum(a => a.AccountBalance));
            var totalBalanceOfDebit = _IRoundNumbers.GetRoundNumber(childDebit.Where(h => h.ParentId == null).Sum(a => a.AccountBalance));
            //var totalBalanceOfCredit  = childCredit.Sum(a => a.creditor - a.debtor);
            //var totalBalanceOfDebit   = childCredit.Sum(a => a.debtor   - a.creditor);

            parameter.finalAccount = (int)finalAccount.IncomingList;
            var totalBalanceOfIncomeList = incomeListAndBudget.getTopLevelIncomingListAndBudget(parameter).Result.totalBalance;
            totalBalanceOfCredit += _IRoundNumbers.GetRoundNumber(totalBalanceOfIncomeList);
            var newChildCredit = childCredit.ToList().Append(
                new IncomeingListFilterData()
                {
                     LatinName = "Total Incoming List ",
                     ArabicName = "مجموع قائمه الدخل",
                    AccountBalance = _IRoundNumbers.GetRoundNumber(totalBalanceOfIncomeList)
                 });
            BudgetResult finalResult = new BudgetResult()
            {
                CreditDataList = newChildCredit.ToList(),
                DebitDataList = childDebit.ToList(),
                totalCreditor = _IRoundNumbers.GetRoundNumber(totalBalanceOfCredit),
                totalDeptor = _IRoundNumbers.GetRoundNumber(Math.Abs(totalBalanceOfDebit))


            };

            return new ResponseResult() { Data = finalResult, Result = Result.Success };

        }
        public async Task<WebReport> PublicBudgetReport(IncomeListSearchParameter parameter, string ids, exportType exportType, bool isArabic,int fileId=0)
        {
            var data = await getTopLevelBudget(parameter);
            
            var userInfo = await _iUserInformation.GetUserInformation();


            var AllmainData = (BudgetResult)data.Data;
            var debitDataList = AllmainData.DebitDataList.Where(d => d.ParentId == null);
            var creditDataList = AllmainData.CreditDataList.Where(c => c.ParentId == null).ToList();

            
            foreach (var item in debitDataList)
            {
                item.Level = 1;
                item.GroupId = 1;
            }
            foreach (var item in creditDataList)
            {
                item.Level = 1;
                item.GroupId = 2;
            }
            var AllDataList = new List<IncomeingListFilterData>();

            AllDataList.AddRange(creditDataList);
            AllDataList.AddRange(debitDataList);



            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, parameter.From, parameter.To);
            var otherdata = new IncomingListData()
            {


                TotalBalance = AllmainData.totalCreditor,
                TotalBalanceDebitor = AllmainData.totalDeptor,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };
            if (ids == null || ids == "")
            {

            }
            else
            {
                string[] parentId = ids.Split(",");
                if (parentId.Count() > 0)
                {

                    foreach (var id in parentId)
                    {
                        var childList = await getAllDataBudgetById(parameter, Convert.ToInt32(id));
                        var objectData = AllDataList.Where(a => a.Id == Convert.ToInt32(id)).FirstOrDefault();
                        int index = AllDataList.IndexOf(objectData);
                        foreach (var item in (List<IncomeingListFilterData>)childList.Data)
                        {
                            item.GroupId = objectData.GroupId;
                        }  
                        AllDataList.InsertRange(index + 1, (List<IncomeingListFilterData>)childList.Data);
                    }
                }
            }

            var tablesNames = new TablesNames()
            {
                
                FirstListName = "FinancialAccount"
            };



            var report = await _iGeneralPrint.PrintReport<object, IncomeingListFilterData, object>(null, AllDataList, null, tablesNames, otherdata
           , (int)SubFormsIds.PublicBudget_GL, exportType, isArabic, fileId);
            return report;

        }



    }
}
