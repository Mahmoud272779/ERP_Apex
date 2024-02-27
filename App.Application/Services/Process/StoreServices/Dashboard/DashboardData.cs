using App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan;
using App.Application.Helpers.Dashboard;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports.Sales;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Infrastructure.UserManagementDB;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Dashboard
{
    public class DashboardData : IDashboardData
    {
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRoundNumbers _roundNumbers;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryQuery<GLBank> _glBankQuery;
        private readonly IRepositoryQuery<GLSafe> _glSafeQuery;

        private readonly iRPT_Sales _iRPT_Sales;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;

        public DashboardData(IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, iUserInformation iUserInformation, IRoundNumbers roundNumbers, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryQuery<GLBank> glBankQuery, IRepositoryQuery<GLSafe> glSafeQuery, iRPT_Sales iRPT_Sales, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery)
        {
            this.invoiceMasterQuery = invoiceMasterQuery;
            _iUserInformation = iUserInformation;
            _roundNumbers = roundNumbers;
            _glRecieptsQuery = glRecieptsQuery;
            _glBankQuery = glBankQuery;
            _glSafeQuery = glSafeQuery;
            _iRPT_Sales = iRPT_Sales;
            _invGeneralSettingsQuery = invGeneralSettingsQuery;
        }
        public async Task<ResponseResult> GetCurrenTPeroidTotalsForInvoices(DateTime dateFrom, DateTime dateTo)
        {
          
            var data = await GetInvoices(dateFrom, dateTo);
            return data;

        }

        public async Task<ResponseResult> GetCurrenTYearTotalsForInvoices()
        {

           
            var startDateOfyear = new DateTime(DateTime.Now.Year, 1, 1);
            var CurrentDate= DateTime.Now.Date;
            var data = await GetInvoices(startDateOfyear, CurrentDate);

           
            return data;
        }
        public async Task<ResponseResult> IncommingCurrentPeriod( DateTime dateFrom ,DateTime dateTo)
        {
            //var userInfo= await _iUserInformation.GetUserInformation();
            //if (!userInfo.otherSettings.showDashboardForAllUsers)
            //{
            //    return new ResponseResult { Result = Result.UnAuthorized };
            //}
            
            var data =  await GetReceipts(dateFrom, dateTo);
            if (data.Result == Result.UnAuthorized)
            {
                return new ResponseResult() { Result = Result.UnAuthorized };
            }
            var receipts = (List<ReceiptsResponse>)data.Data;
            var revenues = receipts.Where(r => r.RecieptTypeId == (int)DocumentType.SafeCash || r.RecieptTypeId == (int)DocumentType.BankCash);
            var expenses = receipts.Where(r => r.RecieptTypeId == (int)DocumentType.SafePayment || r.RecieptTypeId == (int)DocumentType.SafePayment);
            var result = new IncomingCurrentPeroidResponse
            {
                incomingCurrentPeroid = new IncomingCurrentPeroid
                {

                    revenues = _roundNumbers.GetRoundNumber(revenues.Sum(r => r.Amount)),
                    expenses = _roundNumbers.GetRoundNumber(expenses.Sum(r => r.Amount)),
                    incoming = _roundNumbers.GetRoundNumber(revenues.Sum(r => r.Amount) - expenses.Sum(r => r.Amount))

                }
                
                
            };
           // result.incoming = result.revenues - result.expenses;
           


            return new ResponseResult() { Data= result ,Result=Result.Success};
        }
        public async Task<ResponseResult> BalancesOfBanksAndSafes()
        {
           
            var userInfo = await _iUserInformation.GetUserInformation();
            if (!userInfo.otherSettings.showDashboardForAllUsers)
            {
                return new ResponseResult() {Result = Result.UnAuthorized };
            }
            var startDateOfyear = new DateTime(DateTime.Now.Year, 1, 1);
            var CurrentDate = DateTime.Now.Date;
            var data= _glRecieptsQuery.TableNoTracking
                                       .Where(x=>x.BranchId==userInfo.CurrentbranchId)
                                       .Where(x => !x.IsBlock)
                                       .Where(x => !x.Deferre)
                                       //.Where(x => x.RecieptDate.Date >= startDateOfyear.Date && x.RecieptDate.Date <= CurrentDate.Date)
                                       .Where(x => x.IsAccredit == true || x.ParentTypeId == (int)Enums.DocumentType.SafeFunds || x.ParentTypeId == (int)Enums.DocumentType.BankFunds)
                                       .ToList();

            var banks = _glBankQuery.TableNoTracking.Select(a => new { a.ArabicName, a.LatinName, a.Id });
            var safes= _glSafeQuery.TableNoTracking.Select(a => new { a.ArabicName, a.LatinName, a.Id });
            var banksBalances = new List<BankSafeBalances>();
            foreach(var bank in banks)
            {
                banksBalances.Add(new BankSafeBalances
                {
                    balance = _roundNumbers.GetRoundNumber(data.Where(x => x.BankId == bank.Id && x.RecieptTypeId == (int)DocumentType.BankPayment).Sum(x => x.Creditor) -
                      data.Where(x => x.BankId == bank.Id && x.RecieptTypeId == (int)DocumentType.BankCash).Sum(x => x.Debtor)),
                    arabicName = bank.ArabicName,
                    latinName = bank.LatinName
                });
                
            }
            var safesBalances = new List<BankSafeBalances>();
            foreach (var safe in safes)
            {
                safesBalances.Add(new BankSafeBalances
                {
                    balance = _roundNumbers.GetRoundNumber(data.Where(x => x.SafeID == safe.Id && x.RecieptTypeId == (int)DocumentType.SafePayment).Sum(x => x.Amount) -
                      data.Where(x => x.SafeID == safe.Id && x.RecieptTypeId == (int)DocumentType.SafeCash).Sum(x => x.Amount)),
                    arabicName = safe.ArabicName,
                    latinName = safe.LatinName
                });

            }

            var result = new BanksSafesBalancesReponse
            {
                banksBalances= banksBalances,
                safesBalances= safesBalances,
                totalBanksBalance= _roundNumbers.GetRoundNumber( banksBalances.Sum(x=>x.balance)),
                totalSafesBalance = _roundNumbers.GetRoundNumber( safesBalances.Sum(x => x.balance))

            };

            return new ResponseResult() { Data =result,Result=Result.Success };
        }
        public async Task<ResponseResult> GetInvoices(DateTime dateFrom, DateTime dateTo)
        {
            
            var data = await GetInvoicesDb(dateFrom, dateTo);
            var totalSales = data.Where(h => h.invoiceTypeId == (int)DocumentType.Sales
          || h.invoiceTypeId == (int)DocumentType.POS);

            var totalCurrentSales = new TotalCurrentSales()
            {
                TotalSales = _roundNumbers.GetRoundNumber(totalSales.Sum(h => h.net)),
                totalPaid = _roundNumbers.GetRoundNumber(totalSales.Sum(h => h.paid)),
                totalRemian = _roundNumbers.GetRoundNumber(totalSales.Sum(h => h.net) - totalSales.Sum(h => h.paid))

            };
            var totalPurchases = data.Where(h => h.invoiceTypeId == (int)DocumentType.Purchase);
            var totalCurrentPurchases = new TotalCurrentPurchases()
            {
                TotalPurchases = _roundNumbers.GetRoundNumber(totalPurchases.Sum(h => h.net)),
                totalPaid =_roundNumbers.GetRoundNumber( totalPurchases.Sum(h => h.paid)),
                totalRemian =_roundNumbers.GetRoundNumber( totalPurchases.Sum(h => h.net) - totalPurchases.Sum(h => h.paid))

            };
            var CurrentPeroidInvoices = new PeroidTotalsForInvoicesResponse
            {
                totalCurrentSales = totalCurrentSales,
                totalCurrentPurchaes = totalCurrentPurchases
            };
            return new ResponseResult() { Data = CurrentPeroidInvoices, Result = Result.Success };
            //return data;
        }
        private async Task<List<GetInvoices>> GetInvoicesDb(DateTime dateFrom, DateTime dateTo,bool allTyes=true,bool isFinancialYear=false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            // userInfo.
            if (isFinancialYear)
            {
               var dates = await GetFinancialYear();
                dateFrom = dates.Item1;
                dateTo=dates.Item2;

            }

            var invoices = await invoiceMasterQuery.TableNoTracking
                .Where(x => userInfo.otherSettings.showDashboardForAllUsers ? true : x.EmployeeId == userInfo.employeeId)
                .Where(x => x.BranchId == userInfo.CurrentbranchId)
                .Where(x => x.InvoiceDate.Date >= dateFrom.Date && x.InvoiceDate.Date <= dateTo.Date)
                .Where(h => allTyes ? ( h.InvoiceTypeId == (int)DocumentType.Sales || h.InvoiceTypeId == (int)DocumentType.POS
                || h.InvoiceTypeId == (int)DocumentType.Purchase): (h.InvoiceTypeId==(int)DocumentType.Sales || h.InvoiceTypeId == (int)DocumentType.POS)).Select(x => new
                GetInvoices
                { net = x.Net, paid = x.Paid, invoiceTypeId = x.InvoiceTypeId,InvoiceDate=x.InvoiceDate })
                .ToListAsync();
            return invoices;
        }

        public async Task<ResponseResult> SalesTrensaction()
        {

            var dates = await GetFinancialYear();
            var startDateOfyear = dates.Item1;
            var CurrentDate = dates.Item2;
            var data = await GetInvoicesDb(startDateOfyear, CurrentDate,false);
            var invoices = new List<GetInvoices>();
            
            var salesTransaction = new List<SalesTransaction>();
            for (DateTime date = startDateOfyear; date <= CurrentDate; date = date.AddMonths(1))
            {
               invoices=  data.Where(s => s.InvoiceDate.Month == date.Month).ToList();

                salesTransaction.Add(
                    new SalesTransaction
                    {
                        Month=date.Month,
                        totalPaid= _roundNumbers.GetRoundNumber( invoices.Sum(h=>h.paid)),
                        TotalSales = _roundNumbers.GetRoundNumber(invoices.Sum(h => h.net)),
                        totalRemian = _roundNumbers.GetRoundNumber(invoices.Sum(h => h.net)- invoices.Sum(h => h.paid))
                    }
                ) ;

            }
            var result = new SalesTransactionResponse
            {
                SalesTransaction= salesTransaction
            };

            return new ResponseResult() { Data=result,Result=Result.Success};
        }

        public async Task<ResponseResult> SalesPurchasesTrensaction(DateTime dateFrom, DateTime dateTo)
        {

            var data = await GetInvoicesDb(dateFrom, dateTo);
            var salesInvoices = new List<GetInvoices>();
            var purchasesInvoices = new List<GetInvoices>();

            var salesTransaction = new List<SalesPurchasesTransaction>();
            var purchasesTransaction = new List<SalesPurchasesTransaction>();


            for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
            {
                salesInvoices = data.Where(s => s.InvoiceDate.Day == date.Day && (s.invoiceTypeId==(int)DocumentType.Sales||
                s.invoiceTypeId == (int)DocumentType.POS)).ToList();

                salesTransaction.Add(
                    new SalesPurchasesTransaction
                    {
                        Day = date.Day,
                        Net= _roundNumbers.GetRoundNumber(salesInvoices.Sum(h => h.net))
                        
                    }
                );
                purchasesInvoices = data.Where(s => s.InvoiceDate.Day == date.Day && (s.invoiceTypeId == (int)DocumentType.Purchase)).ToList();
                purchasesTransaction.Add(
                    new SalesPurchasesTransaction
                    {
                        Day = date.Day,
                        Net = _roundNumbers.GetRoundNumber(salesInvoices.Sum(h => h.net))

                    }
                );


            }

            var result = new SalesPurchasesTransactionRsponse
            {
                SalesTransaction = salesTransaction,
                PurchasesTransaction = purchasesTransaction
            };
            return new ResponseResult() { Data = result, Result = Result.Success };
        }
        public async Task<ResponseResult> RevenuesExpensesTransaction(DateTime dateFrom, DateTime dateTo)
        {
            
            var receipts = await GetReceipts(dateFrom, dateTo);
            var data = (List <ReceiptsResponse>) receipts.Data;
            var revenues = new List<ReceiptsResponse>();
            var expenses = new List<ReceiptsResponse>();


            var revenuesTransaction = new List<RevenuesExpensesTransaction>();
            var expensesTransaction = new List<RevenuesExpensesTransaction>();
            for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
            {
                revenues = data.Where(s => s.ReceiptDate.Day == date.Day && (s.RecieptTypeId == (int)DocumentType.SafeCash ||
                s.RecieptTypeId == (int)DocumentType.BankCash)).ToList();

                revenuesTransaction.Add(
                    new RevenuesExpensesTransaction
                    {
                        Day = date.Day,
                        Net = _roundNumbers.GetRoundNumber(revenues.Sum(h => h.Amount))

                    }
                );
                expenses = data.Where(s => s.ReceiptDate.Day == date.Day && (s.RecieptTypeId == (int)DocumentType.SafePayment
                || s.RecieptTypeId == (int)DocumentType.BankPayment)).ToList();
                expensesTransaction.Add(
                    new RevenuesExpensesTransaction
                    {
                        Day = date.Day,
                        Net = _roundNumbers.GetRoundNumber(expenses.Sum(h => h.Amount))

                    }
                );


            }
            var RevenuesExpensesTransaction = new RevenuesExpensesTransactionRsponse
            {
                revenuesTransaction = revenuesTransaction,
                expensesTransaction = expensesTransaction
            };
            return new ResponseResult() { Data = RevenuesExpensesTransaction, Result = Result.Success };
        }


        public async Task<ResponseResult> FinancailFlow()
        {
            var dates = await GetFinancialYear();
            var startDateOfyear = dates.Item1;
            var CurrentDate =dates.Item2;
            var receipts = await GetReceipts(startDateOfyear, CurrentDate);
            var data = (List<ReceiptsResponse>)receipts.Data;
            var revenues = new List<ReceiptsResponse>();
            var expenses = new List<ReceiptsResponse>();





            var financailFlow = new List<FinancailFlow>();
            for (DateTime date = startDateOfyear; date <= CurrentDate; date = date.AddMonths(1))
            {
                //revenuesExpenses = data.Where(s => s.ReceiptDate.Month == date.Month).ToList();
                revenues = data.Where(s => s.ReceiptDate.Month == date.Month && (s.RecieptTypeId == (int)DocumentType.SafeCash ||
               s.RecieptTypeId == (int)DocumentType.BankCash)).ToList();
                expenses = data.Where(s => s.ReceiptDate.Month == date.Month && (s.RecieptTypeId == (int)DocumentType.SafePayment
                || s.RecieptTypeId == (int)DocumentType.BankPayment)).ToList();
                financailFlow.Add(
                    new FinancailFlow
                    {
                        Month = date.Month,
                        revenues = _roundNumbers.GetRoundNumber(revenues.Sum(h => h.Amount)),
                        expenses = _roundNumbers.GetRoundNumber(expenses.Sum(h => h.Amount)),
                        
                    }
                );

            }
            var financailFlowResponse = new FinancailFlowResponse
            {
                FinancailFlow = financailFlow,
               
            };
            return new ResponseResult() { Data = financailFlowResponse, Result = Result.Success };
        }

        public async Task<ResponseResult> NewestInvoicesAndMostSoldItems( DateTime dateFrom,DateTime dateTo)
        {
            var userInfo =await _iUserInformation.GetUserInformation();
            var invoices = await invoiceMasterQuery.TableNoTracking
                .Where(x => userInfo.otherSettings.showDashboardForAllUsers ? true : x.EmployeeId == userInfo.employeeId)
                .Where(x => x.BranchId == userInfo.CurrentbranchId)
                //.Where(x => x.InvoiceDate.Date >= dateFrom.Date && x.InvoiceDate.Date <= dateTo.Date)
                .Where(h => h.InvoiceTypeId == (int)DocumentType.Sales || h.InvoiceTypeId == (int)DocumentType.POS
                || h.InvoiceTypeId == (int)DocumentType.Purchase).Take(20).Select(x => new
                NewestInvoices
                { Net = x.Net,CusomerSupplierNameAr=x.Person.ArabicName,CusomerSupplierNameEn=x.Person.LatinName,
                    InvoiceCode=x.InvoiceType ,Status=x.PaymentType,
                    InvoiceTypeId=x.InvoiceTypeId
                })
                .ToListAsync();

            foreach (var item in invoices)
            {
                var invTypes = Aliases.listOfInvoicesNames.listOfNames().Where(h => h.invoiceTypeId == item.InvoiceTypeId);
                if (item.InvoiceTypeId == (int)DocumentType.ReturnSales)
                    invTypes.Where(h => h.invoiceTypeId == (int)DocumentType.ReturnSales).FirstOrDefault().NameAr = "مرتجع فاتورة مبيعات";

                if (item.InvoiceTypeId == (int)DocumentType.Sales)

                    invTypes.Where(h => h.invoiceTypeId == (int)DocumentType.Sales).FirstOrDefault().NameAr = "فاتورة مبيعات";


                item.InvoiceTypeAr =  item.InvoiceCode+ " " + invTypes.Select(a => a.NameAr).FirstOrDefault();
                item.InvoiceTypeArEn = invTypes.Select(a => a.NameEn).FirstOrDefault()+" "+item.InvoiceCode;
                
            }

            var parameters = new itemsSoldMostRequstDTO
            {
                PageNumber=1,
                PageSize=20,
                branches=userInfo.CurrentbranchId.ToString(),
                dateFrom= dateFrom,
                dateTo= dateTo,
                itemSoldMostEnum= itemSoldMostEnum.qyt,
                searchValue=10
            };
             var mostSoldItemsResponse = await _iRPT_Sales.getitemsSoldMost(parameters,true);
            var mostSoldItems = (itemsSoldMostResponseDTO)mostSoldItemsResponse.data;

            var NewestInvoicesAndMostSoldItems = new NewestInvoicesAndMostSoldItems
            {
                NewestInvoices= invoices,
                ItemsSoldMost=  mostSoldItems.itemsSoldMostResponseLists
            };

            return  new ResponseResult() { Data= NewestInvoicesAndMostSoldItems ,Result=Result.Success} ;
        }
        public async Task<ResponseResult> SalesMenWhoSoldMost(DateTime dateFrom, DateTime dateTo)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            if (!userInfo.otherSettings.showDashboardForAllUsers)
            {
                return new ResponseResult() { Result = Result.UnAuthorized };
            }

            var salesMenWhoSoldMost = invoiceMasterQuery
                .TableNoTracking
                .Where(h=>h.InvoiceDate.Date >= dateFrom.Date && h.InvoiceDate.Date <= dateTo.Date)
                .Where(h => h.IsDeleted == false)
                .Where(h=>h.BranchId==userInfo.CurrentbranchId)
                
                .Where(h=> h.InvoiceTypeId == (int)DocumentType.Sales)
                .Include(h=>h.Branch).Include(h=>h.salesMan)
                .OrderBy(h=>h.Net).GroupBy(h=>  new { h.SalesManId }).Select(h=> new SalesMenWhoSoldMost{
                    salesManId=(int)h.First().SalesManId,
                    SalesManNameAr=h.First().salesMan.ArabicName,
                    SalesManNameEn=h.First().salesMan.LatinName,
                    BranchesAr=h.First().Branch.ArabicName,
                    BranchesEn=h.First().Branch.LatinName,
                    TotalAmountOfInvoices= _roundNumbers.GetDefultRoundNumber(h.Sum(h=>h.Net)),
                    InvoicesCount=h.Count()
                })
                   
                .ToList();

            //salesMenWhoSoldMost.ForEach(h=>h.)

            var SalesMenWhoSoldMostResponse = new SalesMenWhoSoldMostResponse
            {
                SalesMenWhoSoldMost= salesMenWhoSoldMost
            };

            return new ResponseResult() { Data= SalesMenWhoSoldMostResponse ,Result=Result.Success};

        }
        private async Task<ResponseResult> GetReceipts(DateTime dateFrom, DateTime dateTo)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            if (!userInfo.otherSettings.showDashboardForAllUsers)
            {
                return new ResponseResult { Result = Result.UnAuthorized };
            }
            var receipts = _glRecieptsQuery.TableNoTracking
               .Where(c => (userInfo.userId != 1 ? c.UserId == userInfo.employeeId : true))
               .Where(c => c.BranchId == userInfo.CurrentbranchId)
               .Where(c => c.IsAccredit == true)
               .Where(c => c.Deferre == false)
               //.Where(x => x.ParentTypeId == (int)Enums.DocumentType.SafeFunds || x.ParentTypeId == (int)Enums.DocumentType.BankFunds)
               .Where(x => x.CreationDate.Date >= dateFrom.Date && x.CreationDate.Date <= dateTo.Date)
               .Select(r => new ReceiptsResponse {  Amount=r.Amount,RecieptTypeId= r.RecieptTypeId, ReceiptDate=r.RecieptDate }).ToList();
            return new ResponseResult() { Data=receipts, Result = Result.Success };
        }
        private async Task<Tuple<DateTime,DateTime>> GetFinancialYear()
        {
            var startFinancialYearDate= _invGeneralSettingsQuery.TableNoTracking.FirstOrDefault().Accredite_StartPeriod;
            var endFinancialYearDate = _invGeneralSettingsQuery.TableNoTracking.FirstOrDefault().Accredite_StartPeriod;

            return new Tuple<DateTime, DateTime>(startFinancialYearDate, endFinancialYearDate);

        }
    }
}
