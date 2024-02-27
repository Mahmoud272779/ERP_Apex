using App.Domain.Models.Request.General;
using App.Infrastructure;
using App.Infrastructure.settings;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralAPIsHandler.HomeData
{
    public class HomeDataHandler : IRequestHandler<HomeDataRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRoundNumbers _roundNumbers;

        public HomeDataHandler(iUserInformation iUserInformation, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRoundNumbers roundNumbers, IRepositoryQuery<GlReciepts> glRecieptsQuery)
        {
            _iUserInformation = iUserInformation;
            this.invoiceMasterQuery = invoiceMasterQuery;
            _roundNumbers = roundNumbers;
            _GlRecieptsQuery = glRecieptsQuery;
        }

        public async Task<ResponseResult> Handle(HomeDataRequest param, CancellationToken cancellationToken)
        {
            if (param.dateFrom == null || param.dateTo == null)
                return new ResponseResult() { Result=Result.RequiredData,ErrorMessageAr=ErrorMessagesAr.DataRequired,ErrorMessageEn=ErrorMessagesEn.DataRequired};
            var userInfo = await _iUserInformation.GetUserInformation();
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var currentMonth = new DateTime(date.Year, date.Month, 1);
            var currentDay = DateTime.Now.Day;
            var monthDays = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
            var LastMonth = DateTime.Now.AddMonths(-1).Date;
            var LastMonthStart = new DateTime(LastMonth.Year, LastMonth.Month, 1);
            var lastMonthDays = DateTime.DaysInMonth(LastMonthStart.Year, LastMonthStart.Month);
            var lastMonthEnd = new DateTime(LastMonth.Year, LastMonth.Month, lastMonthDays);
            var countOfDays = (param.dateTo.Value.Date - param.dateFrom.Value.Date).Days +1;

           // var invoices = invoiceMasterQuery.TableNoTracking.Where(x => (!userInfo.otherSettings.showDashboardForAllUsers ? x.EmployeeId == userInfo.employeeId : true) && !x.IsDeleted && x.BranchId == userInfo.CurrentbranchId);
            var invoices = invoiceMasterQuery.TableNoTracking
                .Where(c=> !userInfo.otherSettings.showDashboardForAllUsers ? c.EmployeeId == userInfo.employeeId : true )
                .Where(x => !x.IsDeleted && x.BranchId == userInfo.CurrentbranchId);


            invoices = invoices.Where(a => (a.InvoiceTypeId == (int)DocumentType.Sales && !userInfo.otherSettings.salesShowOtherPersonsInv)
                                       || ((a.InvoiceTypeId == (int)DocumentType.Purchase || a.InvoiceTypeId == (int)DocumentType.wov_purchase)
                                                       && !userInfo.otherSettings.purchasesShowOtherPersonsInv)
                                       || (a.InvoiceTypeId == (int)DocumentType.POS && !userInfo.otherSettings.posShowOtherPersonsInv) ?
                                                    a.EmployeeId == userInfo.employeeId : true);


            var currentMonth_invoices = invoices.Where(x => x.InvoiceDate.Date >= currentMonth && x.InvoiceDate.Date <= DateTime.Now.Date);
            var LastMonth_invoices = invoices.Where(x => x.InvoiceDate.Date >= LastMonthStart && x.InvoiceDate.Date <= lastMonthEnd);
            #region salesAmout
            var salesAmout_CurrentMonthInvoice = currentMonth_invoices.Where(c => c.InvoiceTypeId == (int)DocumentType.Sales || c.InvoiceTypeId == (int)DocumentType.POS).Sum(x => x.Net);
            var salesAmout_LastInvoice = LastMonth_invoices.Where(c => c.InvoiceTypeId == (int)DocumentType.Sales || c.InvoiceTypeId == (int)DocumentType.POS).Sum(x => x.Net);
            salesAmout salesAmout = new salesAmout
            {
                monthDays = monthDays,
                currentDay = currentDay,
                currentMonth = _roundNumbers.GetRoundNumber(salesAmout_CurrentMonthInvoice),
                lastMonth = _roundNumbers.GetRoundNumber(salesAmout_LastInvoice),
                percent = salesAmout_LastInvoice == 0 && salesAmout_CurrentMonthInvoice == 0 ? 0 : (salesAmout_LastInvoice == 0 && salesAmout_CurrentMonthInvoice != 0 ? 100 : _roundNumbers.GetRoundNumber(((salesAmout_CurrentMonthInvoice - salesAmout_LastInvoice) / salesAmout_LastInvoice) * 100))
            };
            #endregion
            #region purchasesAmout
            var purchasesAmout_currentMonth = currentMonth_invoices.Where(c => c.InvoiceTypeId == (int)DocumentType.wov_purchase || c.InvoiceTypeId == (int)DocumentType.Purchase).Sum(x => x.Net);
            var purchasesAmout_lastMonth = LastMonth_invoices.Where(c => c.InvoiceTypeId == (int)DocumentType.wov_purchase || c.InvoiceTypeId == (int)DocumentType.Purchase).Sum(x => x.Net);
            purchasesAmout purchasesAmout = new purchasesAmout
            {
                monthDays = monthDays,
                currentDay = currentDay,
                currentMonth = _roundNumbers.GetRoundNumber(purchasesAmout_currentMonth),
                lastMonth = _roundNumbers.GetRoundNumber(purchasesAmout_lastMonth),
                percent = purchasesAmout_lastMonth == 0 && purchasesAmout_currentMonth == 0 ? 0 : (purchasesAmout_lastMonth == 0 && purchasesAmout_currentMonth != 0 ? 100 : _roundNumbers.GetRoundNumber(((purchasesAmout_currentMonth - purchasesAmout_lastMonth) / purchasesAmout_lastMonth) * 100))
            };
            #endregion
            if (!string.IsNullOrEmpty(param.dateFrom.ToString()) || !string.IsNullOrEmpty(param.dateTo.ToString()))
                invoices = invoices.Where(x => x.InvoiceDate.Date >= param.dateFrom.Value.Date && x.InvoiceDate.Date <= param.dateTo.Value.Date);
            //else
            //    invoices = invoices.Where(x => x.InvoiceDate.Date >= firstDayOfMonth.Date && x.InvoiceDate.Date <= DateTime.Now.Date);
            #region countOfPurchasesInvoice
            CountOfPurchasesInvoice countOfPurchasesInvoice = new CountOfPurchasesInvoice()
            {
                DeportedInvoicesCount = invoices.Where(x => x.InvoiceTypeId == (int)DocumentType.Purchase && x.PaymentType == (int)PaymentType.Delay).Count(),
                PaidInvoicesCount = invoices.Where(x => x.InvoiceTypeId == (int)DocumentType.Purchase && x.PaymentType == (int)PaymentType.Complete).Count(),
                PartInvoicesCount = invoices.Where(x => x.InvoiceTypeId == (int)DocumentType.Purchase && x.PaymentType == (int)PaymentType.Partial).Count(),
            };
            #endregion
            #region countOfSalesInvoices
            CountOfSalesInvoices countOfSalesInvoices = new CountOfSalesInvoices()
            {
                DeportedInvoicesCount = invoices.Where(x => x.InvoiceTypeId == (int)DocumentType.Sales && x.PaymentType == (int)PaymentType.Delay).Count(),
                PaidInvoicesCount = invoices.Where(x => x.InvoiceTypeId == (int)DocumentType.Sales && x.PaymentType == (int)PaymentType.Complete).Count(),
                PartInvoicesCount = invoices.Where(x => x.InvoiceTypeId == (int)DocumentType.Sales && x.PaymentType == (int)PaymentType.Partial).Count(),
            };
            #endregion
            #region NewestInvoices
            List<NewestInvoices> listnewestInvoices = new List<NewestInvoices>();
            var firstFiveInvoices = invoices.Where(x => !x.IsReturn && !x.IsDeleted && (x.InvoiceTypeId == (int)DocumentType.Sales || x.InvoiceTypeId == (int)DocumentType.Purchase))
                                            .OrderByDescending(x => x.InvoiceId)
                                            .Skip(0).Take(5);


            string arabicName = "";
            string latinName = "";
            string invoiceCode = "";
            string invoicePaymentTypeAr = "";
            string invoicePaymentTypeEn = "";

            foreach (var item in firstFiveInvoices)
            {
                if (item.PaymentType == (int)PaymentType.Delay)
                {
                    invoicePaymentTypeAr = "اجلة";
                    invoicePaymentTypeEn = "Deported";

                }
                else if (item.PaymentType == (int)PaymentType.Complete)
                {
                    invoicePaymentTypeAr = "نقدية";
                    invoicePaymentTypeEn = "Paid";

                }
                else if (item.PaymentType == (int)PaymentType.Partial)
                {
                    invoicePaymentTypeAr = "جزئي";
                    invoicePaymentTypeEn = "Partial";

                }
                if (item.InvoiceTypeId == (int)DocumentType.Sales)
                {
                    arabicName = $"فاتورة مبيعات {invoicePaymentTypeAr} رقم";
                    latinName = $"sales Invoice {invoicePaymentTypeEn} Number ";
                    invoiceCode = item.InvoiceType;
                }
                else if (item.InvoiceTypeId == (int)DocumentType.Purchase)
                {
                    arabicName = $"فاتورة مشتريات {invoicePaymentTypeAr} رقم";
                    latinName = $"Purchase Invoice {invoicePaymentTypeEn} Number ";
                    invoiceCode = item.InvoiceType;

                }
                NewestInvoices newestInvoices = new NewestInvoices()
                {
                    Id = item.InvoiceId,
                    invoiceTypeId = item.InvoiceTypeId,
                    ArabicName = arabicName,
                    LatinName = latinName,
                    invoiceCode = invoiceCode
                };
                listnewestInvoices.Add(newestInvoices);
            }
            #endregion
            #region InvoicesMovement
            var invoicesDays = invoices.GroupBy(x => x.InvoiceDate.Date).Select(y => y.FirstOrDefault());
            List<InvoicesMovementDetalies> InvoicesMovementDetalies = new List<InvoicesMovementDetalies>();
            InvoicesMovementDetalies.Add(new InvoicesMovementDetalies()
            {
                index = 0,
                SalesCount = 0,
                PurchasesCount = 0
            });
            var days = invoices.GroupBy(c=> c.InvoiceDate.Date).Select(c=> c.FirstOrDefault());
            var InvoicesMovementDetalies_index = 0;

            var InvoicesMovementDetalies_newDate = param.dateFrom.Value.Date;
            for (int i = 0; i < countOfDays; i++)
            {
                InvoicesMovementDetalies_index++;
                InvoicesMovementDetalies.Add(new InvoicesMovementDetalies()
                {
                    index = InvoicesMovementDetalies_index,
                    SalesCount = invoices.Where(x => x.InvoiceDate.Date == InvoicesMovementDetalies_newDate.AddDays(i).Date && (x.InvoiceTypeId == (int)DocumentType.Sales | x.InvoiceTypeId == (int)DocumentType.POS)).Sum(x => x.Net),
                    PurchasesCount = invoices.Where(x => x.InvoiceDate.Date == InvoicesMovementDetalies_newDate.AddDays(i).Date && x.InvoiceTypeId == (int)DocumentType.Purchase).Sum(x => x.Net),
                    date = InvoicesMovementDetalies_newDate.AddDays(i).ToString(defultData.datetimeFormat)
                });
            }
            var maxSales = InvoicesMovementDetalies.Select(x => x.SalesCount).Max();
            var maxPurchases = InvoicesMovementDetalies.Select(x => x.PurchasesCount).Max();
            InvoicesMovement invoicesMovement = new InvoicesMovement
            {
                data = InvoicesMovementDetalies,
                maximumValue = maxSales >= maxPurchases ? maxSales + (maxSales * .1) : maxPurchases + (maxPurchases * .1)
            };
            #endregion
            #region CountOfPurchasesWithoutVATInvoice
            var wov_purchase = invoices.Where(c => c.InvoiceTypeId == (int)DocumentType.wov_purchase);
            CountOfPurchasesWithoutVATInvoice countOfPurchasesWithoutVATInvoice = new CountOfPurchasesWithoutVATInvoice
            {
                DeportedInvoicesCount = wov_purchase.Where(c => c.PaymentType == (int)PaymentType.Delay).Count(),
                PaidInvoicesCount = wov_purchase.Where(c => c.PaymentType == (int)PaymentType.Complete).Count(),
                PartInvoicesCount = wov_purchase.Where(c => c.PaymentType == (int)PaymentType.Partial).Count()
            };
            #endregion
            #region CountOfPOSInvoices
            var posInvoices = invoices.Where(c => c.InvoiceTypeId == (int)DocumentType.POS);
            CountOfPOSInvoices countOfPOSInvoices = new CountOfPOSInvoices()
            {
                DeportedInvoicesCount = posInvoices.Where(c => c.PaymentType == (int)PaymentType.Delay).Count(),
                PaidInvoicesCount = posInvoices.Where(c => c.PaymentType == (int)PaymentType.Complete).Count(),
                PartInvoicesCount = posInvoices.Where(c => c.PaymentType == (int)PaymentType.Partial).Count()
            };
            #endregion
            #region HomeDataResponse_incomingAndOutgoingTransaction
            var Reciepts = _GlRecieptsQuery.TableNoTracking
                .Where(c=> (!userInfo.otherSettings.showDashboardForAllUsers && userInfo.userId != 1 ? c.UserId == userInfo.employeeId : true) && c.BranchId == userInfo.CurrentbranchId)
                .Where(x=> param.dateFrom !=null && param.dateTo != null ? (x.CreationDate.Date >= param.dateFrom.Value.Date && x.CreationDate.Date <= param.dateTo.Value.Date) : true);

            var incomingAndOutgoingTransactionDetalies = new List<HomeDataResponse_incomingAndOutgoingTransaction_Detalies>();
            incomingAndOutgoingTransactionDetalies.Add(new HomeDataResponse_incomingAndOutgoingTransaction_Detalies
            {
                index = 0,
                incoming = 0,
                outgoing = 0,
            });
            var dates = Reciepts.GroupBy(c => c.CreationDate).Select(c => c.FirstOrDefault());
            int incomingAndOutgoingTransactionDetalies_Index = 0;
            var incomingAndOutgoingTransactionDetalies_NewDate = param.dateFrom.Value.Date;
            for (int i = 0; i < countOfDays; i++)
            {
                incomingAndOutgoingTransactionDetalies_Index ++;
                var recieptsOfDay = Reciepts.Where(c => c.CreationDate.Date == incomingAndOutgoingTransactionDetalies_NewDate.AddDays(i));
                incomingAndOutgoingTransactionDetalies.Add(new HomeDataResponse_incomingAndOutgoingTransaction_Detalies
                {
                    index = incomingAndOutgoingTransactionDetalies_Index,
                    incoming = recieptsOfDay.Where(c => c.Signal > 0).Sum(x => x.Amount),
                    outgoing = recieptsOfDay.Where(c => c.Signal < 0).Sum(x => x.Amount),
                    date = incomingAndOutgoingTransactionDetalies_NewDate.AddDays(i).ToString(defultData.datetimeFormat)
                });
            }
            var maxIncoming = incomingAndOutgoingTransactionDetalies.Max(x => x.incoming);
            var maxoutgoing = incomingAndOutgoingTransactionDetalies.Max(x => x.outgoing);
            HomeDataResponse_incomingAndOutgoingTransaction homeDataResponse_IncomingAndOutgoingTransaction = new HomeDataResponse_incomingAndOutgoingTransaction
            {
                incomingAndOutgoingTransactionDetalies = incomingAndOutgoingTransactionDetalies,
                maximumValue = maxIncoming >= maxoutgoing ? maxIncoming + (maxIncoming * .1) : maxoutgoing + (maxoutgoing * .1)
            };
            #endregion

            HomeResponse homeResponse = new HomeResponse()
            {
                countOfPurchasesInvoice = countOfPurchasesInvoice,
                countOfSalesInvoices = countOfSalesInvoices,
                newestInvoices = listnewestInvoices,
                invoicesMovement = invoicesMovement,
                countOfPurchasesWithoutVATInvoice = countOfPurchasesWithoutVATInvoice,
                CountOfPOSInvoices = countOfPOSInvoices,
                salesAmout = salesAmout,
                purchasesAmout = purchasesAmout,
                HomeDataResponse_incomingAndOutgoingTransaction = homeDataResponse_IncomingAndOutgoingTransaction
            };
            return new ResponseResult()
            {
                Data = homeResponse,
                Result = Result.Success
            };
        }
    }
}
