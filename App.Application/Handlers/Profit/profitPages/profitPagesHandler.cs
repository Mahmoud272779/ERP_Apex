using App.Application.Helpers;
using App.Application.Helpers.Dashboard;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.SignalRHub;
using App.Domain.Entities.Process.General;
using App.Infrastructure.settings;
using Castle.Core.Internal;
using Dapper;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Profit.profitPages
{
    public class profitPagesHandler : IRequestHandler<profitPagesRequest, ResponseResult>
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMemoryCache _memoryCache;
        private readonly iUserInformation _iUserInformation;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IRepositoryQuery<signalR> _signalRQuery;
        private readonly IRepositoryQuery<EditedItems> _EditedItemsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _GLJournalEntryQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettingsQuary;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand;
        private readonly IRepositoryCommand<GLJournalEntry> _GLJournalEntryCommand;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;//1
        private readonly IRoundNumbers roundNumbers;
        private UserInformationModel userData;
        private readonly iUserInformation Userinformation;
        private readonly ISecurityIntegrationService _securityIntegrationService;

        public profitPagesHandler(IHttpContextAccessor httpContext, IMemoryCache memoryCache, iUserInformation iUserInformation, IHubContext<NotificationHub> hub, IRepositoryQuery<signalR> signalRQuery, IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuary, IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRoundNumbers roundNumbers, iUserInformation userinformation, IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery, IRepositoryCommand<GLJournalEntry> gLJournalEntryCommand, IRepositoryQuery<EditedItems> editedItemsQuery, ISecurityIntegrationService securityIntegrationService)
        {
            _httpContext = httpContext;
            _memoryCache = memoryCache;
            _iUserInformation = iUserInformation;
            _hub = hub;
            _signalRQuery = signalRQuery;
            GLPurchasesAndSalesSettingsQuary = gLPurchasesAndSalesSettingsQuary;
            this.journalEntryDetailsCommand = journalEntryDetailsCommand;
            this.invoiceMasterQuery = invoiceMasterQuery;
            this.roundNumbers = roundNumbers;
            Userinformation = userinformation;
            userData = Userinformation.GetUserInformation().Result;
            _GLJournalEntryQuery = gLJournalEntryQuery;
            _GLJournalEntryCommand = gLJournalEntryCommand;
            _EditedItemsQuery = editedItemsQuery;
            _securityIntegrationService = securityIntegrationService;
        }

        public async Task<ResponseResult> Handle(profitPagesRequest request, CancellationToken cancellationToken)
        {
            var totalItems = _EditedItemsQuery.TableNoTracking.Count();
            if (totalItems == 0)
                return new ResponseResult
                {
                    Result = Result.Success
                };
            var token = await _httpContext.HttpContext.GetTokenAsync("access_token");
            var serverURL = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}/";
            int profitDone = (int)Result.Failed;
            var userData = await _iUserInformation.GetUserInformation();
            ResponseResult profitResponse = new ResponseResult();
            MemoryCashHelper _cashHelper = new MemoryCashHelper(_memoryCache);

            var memoryCash = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey);
            var profitCash = _memoryCache.Get<List<SignalRCash>>(defultData.ProfitKey);

            bool isCompanyExistBefore = profitCash != null ? profitCash.Where(x => x.CompanyLogin == userData.companyLogin).Any() : false;
            if (isCompanyExistBefore)
            {
                return new ResponseResult()
                {
                    Result = Result.InProgress,
                    ErrorMessageAr = " profit is running ",
                    ErrorMessageEn = " profit is running",
                    Alart = new Alart
                    {
                        AlartType = AlartType.warrning,
                        type = AlartShow.note,
                        MessageAr = "برجاء الانتظار جاري حساب الربحيه",
                        MessageEn = "Please wait the profit is in progress",
                    }
                };
            }
            else
            {
                await _cashHelper.AddSignalRCash(new SignalRCash
                {
                    connectionId = userData.signalRConnectionId,
                    CompanyLogin = userData.companyLogin,
                    EmployeeId = userData.employeeId,
                    UserID = userData.userId

                }, defultData.ProfitKey);
            }

            var signalRCash = _memoryCache.Get<List<SignalRCash>>(defultData.ProfitKey).Where(x => x.CompanyLogin.ToLower() == userData.companyLogin.ToLower());
            var EmployeeIds = signalRCash.Select(x => x.EmployeeId).ToArray();
            string[] usersSignalRConnectionId = signalRCash.Select(x => x.connectionId).ToArray();



            if (string.IsNullOrEmpty(usersSignalRConnectionId.Find(a => a == userData.signalRConnectionId)))
            { usersSignalRConnectionId = usersSignalRConnectionId.Append(userData.signalRConnectionId).ToArray(); }


            var counter = 0;
            var isTotalTook = false;

            try
            {
                if (_EditedItemsQuery.TableNoTracking.Any())
                {
                    while (profitDone != (int)Result.profitDone)
                    {
                        var options = new RestClientOptions(serverURL + "api/Store/TestProfit/CalculateProfitProcess");
                        options.Timeout = int.MaxValue;
                        var client = new RestClient(options);
                        var _request = new RestRequest("");
                        _request.AddHeader("accept", "application/json");
                        _request.AddHeader("authorization", "bearer " + token);
                        var response = await client.GetAsync(_request);

                        profitResponse = JsonConvert.DeserializeObject<ResponseResult>(response.Content);
                        profitDone = profitResponse.Id == null ? 0 : profitResponse.Id.Value;

                        counter = totalItems - _EditedItemsQuery.TableNoTracking.Count();
                        try
                        {
                            usersSignalRConnectionId = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey)
                                                    .Where(c => c.CompanyLogin == userData.companyLogin)
                                                    .Select(c => c.connectionId)
                                                    .ToArray()/*  _signalRQuery.TableNoTracking.Select(x => x.connectionId).ToArray()*/;
                        }
                        catch (Exception)
                        {

                        }
                        
                        if(usersSignalRConnectionId.Any())
                            _hub.Clients.Clients(usersSignalRConnectionId).SendAsync(defultData.ProfitProgress,
                            new porgressData { percentage = Percentage(counter, totalItems), Count = counter, totalCount = totalItems, status = Aliases.ProgressStatus.InProgress, Notes = profitResponse.ErrorMessageEn });
                        if (profitResponse.Result == Result.Failed)
                        {
                            return profitResponse;
                        }
                        Thread.Sleep(500);
                    }
                }


                var companyInfo = await _securityIntegrationService.getCompanyInformation();
                if (/*request.calcJournal && */companyInfo.apps.Any(c => c.Id == (int)applicationIds.GeneralLedger))
                {


                    SqlConnection con = new SqlConnection(_GLJournalEntryQuery.connectionString());
                    int[] journalentriesInvoices;
                    try
                    {
                        con.Open();
                        var arr = Lists.SalesTransaction.Where(c => c != (int)DocumentType.ExtractPermission).ToList();
                        string query = $"select je.InvoiceId from GLJournalEntry as je join InvoiceMaster as im on im.InvoiceId = je.InvoiceId where DocType in ({string.Join(", ", arr)}) and CreditTotal !=0 and DebitTotal !=0 and IsBlock = 0 and je.InvoiceId = im.InvoiceId  and (je.lastTimeProfitCalced != im.lastTimeProfitCalced or je.lastTimeProfitCalced is null or im.lastTimeProfitCalced is null)";
                        journalentriesInvoices = con.Query<int>(query).ToArray();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
        

                    if (journalentriesInvoices.Any())
                        await UpdateJournalEntery(journalentriesInvoices);
                }

            }
            catch (Exception)
            {
                _cashHelper.DeleteSignalRCahedRecored(null, defultData.ProfitKey, userData.companyLogin);
                throw;
            }
            finally
            {
                //end signalr and clear cashe
                if (usersSignalRConnectionId != null)
                {
                    usersSignalRConnectionId = _signalRQuery.TableNoTracking.Select(x => x.connectionId).ToArray();
                    await _hub.Clients.Clients(usersSignalRConnectionId).SendAsync(defultData.ProfitProgress, new porgressData { percentage = 100, Count = totalItems, totalCount = totalItems, status = Aliases.ProgressStatus.ProgressFinshed });
                }
                _cashHelper.DeleteSignalRCahedRecored(null, defultData.ProfitKey, userData.companyLogin);
            }


            return profitResponse;
        }
        private async Task UpdateJournalEntery(int[] journalentriesInvoices)
        {

            List<JEnteryInvoicedata> invoices = GetInvoiceDataForJEntery(journalentriesInvoices).GroupBy(c=> c.invoiceId).Select(c=> c.FirstOrDefault()).ToList();

            if (!invoices.Any())
            {
                return;
            }

            int counter = 0;
            string[] usersSignalRConnectionId = null;
            List<GLJournalEntryDetails> gLJournalEntryDetailsList = new List<GLJournalEntryDetails>();
            List<GLJournalEntry> GLJournalEntryList = new List<GLJournalEntry>();

            //var listOfJounrals = jounralEnteryQuery.TableNoTracking.Where(c => c.InvoiceId != null ? invoices.Select(x => x.invoiceId).ToArray().Contains(c.InvoiceId.Value) : false);
            var purchasesAndSalesSettings = GLPurchasesAndSalesSettingsQuary.TableNoTracking.ToList();

            foreach (var invoice in invoices)
            {
                Thread.Sleep(100);
                counter++;
                //usersSignalRConnectionId = _signalRQuery.TableNoTracking.Select(x => x.connectionId).ToArray();
                try
                {
                    usersSignalRConnectionId = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey)
                                                    .Where(c => c.CompanyLogin == userData.companyLogin)
                                                    .Select(c => c.connectionId)
                                                    .ToArray();
                }
                catch (Exception)
                {

                }
                
                if(usersSignalRConnectionId.Any())
                     _hub.Clients.Clients(usersSignalRConnectionId).SendAsync(defultData.ProfitProgress, new porgressData { Notes = "Update Invoices by Profit Data", percentage = Percentage(counter, invoices.Count), Count = counter, totalCount = invoices.Count, status = Aliases.ProgressStatus.InProgress });

                var journalentry = _GLJournalEntryQuery.TableNoTracking.Where(c => c.InvoiceId == invoice.invoiceId).FirstOrDefault();

                gLJournalEntryDetailsList.AddRange(await SetJournalEntery(invoice, journalentry.Id, purchasesAndSalesSettings));
                journalentry.lastTimeProfitCalced = invoice.lastCalcProfit;
                GLJournalEntryList.Add(journalentry);
            }
            try
            {
                journalEntryDetailsCommand.StartTransaction();
                journalEntryDetailsCommand.AddRange(gLJournalEntryDetailsList);
                bool journalEntryDetails_save = await journalEntryDetailsCommand.SaveAsync();
                bool GLJournalEntry_save = await _GLJournalEntryCommand.UpdateAsyn(GLJournalEntryList);
                journalEntryDetailsCommand.CommitTransaction();
                if (!journalEntryDetails_save || !GLJournalEntry_save)
                    journalEntryDetailsCommand.Rollback();
            }
            catch (Exception)
            {
                journalEntryDetailsCommand.Rollback();
            }
            finally
            {
                purchasesAndSalesSettings.Clear();
            }

        }
        public List<JEnteryInvoicedata> GetInvoiceDataForJEntery(int[] journalentriesInvoices)
        {

            //need branches

            var invoices = invoiceMasterQuery.TableNoTracking
                .Include(h => h.InvoicesDetails)
                .Where(c => journalentriesInvoices.Contains(c.InvoiceId))
                .Where(h => h.IsDeleted == false)
                .Where(h => Lists.SalesTransaction.Contains(h.InvoiceTypeId))
                
                .OrderBy(a => a.Serialize)
                .Select(h => new JEnteryInvoicedata
                {
                    invoiceId = h.InvoiceId,
                    cost = roundNumbers.GetRoundNumber(h.InvoicesDetails.Where(c=> c.parentItemId == null || c.parentItemId == 0).Sum(a => a.Cost * a.Quantity * a.ConversionFactor)),
                    serial = h.Serialize,
                    invoiceType = h.InvoiceTypeId,
                    lastCalcProfit = h.lastTimeProfitCalced.Value
                }).ToList();



            return invoices;

        }
        public async Task<List<GLJournalEntryDetails>> SetJournalEntery(JEnteryInvoicedata invoice, int journalentryID, List<GLPurchasesAndSalesSettings> purchasesAndSalesSettings)
        {
            try
            {



                if (journalentryID == 0)
                    return null;
                await journalEntryDetailsCommand.DeleteAsync(a => a.JournalEntryId == journalentryID && a.isCostSales == true);

                List<GLJournalEntryDetails> journalEntryDetails = new List<GLJournalEntryDetails>();

                journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
                {
                    JournalEntryId = journalentryID,


                    FinancialAccountId = purchasesAndSalesSettings
                                          .Where(h => h.RecieptsType == (int)DocumentType.Purchase)
                                          .Where(h => h.branchId == userData.CurrentbranchId)
                                          .Select(a => a.FinancialAccountId)
                                          .FirstOrDefault(),// المشتريات 



                    Credit = Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost, // total cost
                    Debit = !Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost,
                    DescriptionAr = purAndSalesSettingNames.SalesCostAr,
                    DescriptionEn = purAndSalesSettingNames.SalesCostEn,
                    isCostSales = true

                });
                journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
                {
                    JournalEntryId = journalentryID,

                    FinancialAccountId = purchasesAndSalesSettings
                                            .Where(h => h.RecieptsType == (int)DocumentType.Inventory && h.branchId == userData.CurrentbranchId)
                                            .Select(a => a.FinancialAccountId)
                                            .FirstOrDefault(),// تكلفه المبيعات  


                    Debit = Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost, // total cost
                    Credit = !Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost,
                    DescriptionAr = purAndSalesSettingNames.SalesCostAr,
                    DescriptionEn = purAndSalesSettingNames.SalesCostEn,
                    isCostSales = true

                });

                //journalEntryDetailsCommand.AddRange(journalEntryDetails);

                //var saved = await journalEntryDetailsCommand.SaveAsync();

                return journalEntryDetails;
            }
            catch (Exception e)
            {

                throw;
            }

        }
        private double Percentage(int count, double totalCount)
        {
            double percentage = 0;
            if (totalCount != 0)
            {
                percentage = ((count / totalCount) * 100);
            }
            return percentage;
        }
    }
}
