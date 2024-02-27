using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Domain.Models.Security.Authentication.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ConvertOffline
{
    public  class addJournalEntryForOfflinePosHandler:IRequestHandler<addJournalEntryForOfflinePosRequest,bool>
    {
        private readonly IRepositoryCommand<GLJournalEntry> journalEntryCommand;
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryQuery;
        private readonly IRepositoryQuery<GLCurrency> currencyQuery;
        private readonly iInvoicesIntegrationService invoicesIntegrationService;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _invoiceQuery;
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> gLJournalEntrydetailsCommand;
        private readonly IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand;
        private readonly IRepositoryQuery<InvEmployees> empQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _GLGeneralSettingQuery;

        public addJournalEntryForOfflinePosHandler(IRepositoryCommand<GLJournalEntry> journalEntryCommand,
            IRepositoryQuery<GLJournalEntry> journalEntryQuery, IRepositoryQuery<GLCurrency> currencyQuery,
            iInvoicesIntegrationService invoicesIntegrationService, IRepositoryCommand<GLJournalEntryDetails> gLJournalEntrydetailsCommand,
            IRepositoryQuery<GLPurchasesAndSalesSettings> invoiceQuery, IRepositoryQuery<InvPersons> invPersonsQuery,
            IRepositoryCommand<GLRecHistory> recHistoryRepositoryCommand, IRepositoryQuery<InvEmployees> empQuery, IRepositoryQuery<GLGeneralSetting> gLGeneralSettingQuery)
        {
            this.journalEntryCommand = journalEntryCommand;
            this.journalEntryQuery = journalEntryQuery;
            this.currencyQuery = currencyQuery;
            this.invoicesIntegrationService = invoicesIntegrationService;
            this.gLJournalEntrydetailsCommand = gLJournalEntrydetailsCommand;
            _invoiceQuery = invoiceQuery;
            _invPersonsQuery = invPersonsQuery;
            this.recHistoryRepositoryCommand = recHistoryRepositoryCommand;
            this.empQuery = empQuery;
            _GLGeneralSettingQuery = gLGeneralSettingQuery;
        }
        public async Task<bool> Handle(addJournalEntryForOfflinePosRequest request , CancellationToken cancellationToken)
        {

            var CurrencyId = currencyQuery.TableNoTracking.Where(x => x.IsDefault);
            List<GLJournalEntry> JournalEntrymasterList = new List<GLJournalEntry>();
         
            var InvoiceJournalEntryDetailsForAll = new List<JournalEntryDetail>();
            var historyList = new List<GLRecHistory>();


            var glSettings_ = _invoiceQuery.TableNoTracking;
            var person_ = _invPersonsQuery.TableNoTracking;
            var employee = empQuery.TableNoTracking;
            journalEntryCommand.StartTransaction();
            foreach (var JEntry in request.data)
            {
                var names = getInvoiceName(JEntry.DocType);

                var table = new GLJournalEntry()
                {
                    Code = AutocodeOgJournalEntry(),
                    Auto = true,
                    CurrencyId = CurrencyId.First().Id,
                    FTDate = JEntry.invDate,
                    InvoiceId = JEntry.invoiceId,
                    Notes = names.Item1 + " - " + JEntry.InvoiceCode,
                    DocType = (int)JEntry.DocType,
                    BranchId = request.data.Where(a=>a.invoiceId== JEntry.invoiceId).First().branchId.Value,
                    AddTransactionDate= JEntry.invDate.ToString(),
                    LastTransactionDate = JEntry.invDate.ToString()
                
                };
                await journalEntryCommand.AddAsync(table);
                JEntry.journalEntryId = table.Id;
                  // set journal entry details
                  GenralData generalData = new GenralData()
                {
                    glSettings = glSettings_.Where(a => a.branchId == JEntry.branchId.Value),
                    invoiceNameAr = names.Item1,
                    invoiceNameEn = names.Item2,
                    person = person_.Where(a => a.Id == JEntry.personId).FirstOrDefault(),
                    gLGeneralSettingQuery = _GLGeneralSettingQuery.TableNoTracking.FirstOrDefault()
                    
            };
                JEntry.branchId = table.BranchId;
                var InvoiceJournalEntryDetails = invoicesIntegrationService.SetJournalEntryDetails(JEntry, generalData,0);
                table.CreditTotal = InvoiceJournalEntryDetails.Sum(a => a.Credit);
                table.DebitTotal = InvoiceJournalEntryDetails.Sum(a => a.Debit);
                JournalEntrymasterList.Add(table);

                InvoiceJournalEntryDetailsForAll.AddRange(InvoiceJournalEntryDetails);
                var emp = employee.Where(a => a.Id == JEntry.employeeId).First();
                historyList.Add( new GLRecHistory()
                {
                    employeesId = JEntry.employeeId.Value,
                    LastDate = JEntry.invDate,
                    LastAction = "A",
                    BranchId = JEntry.branchId.Value,
                    JournalEntryId = JEntry.journalEntryId.Value,
                    Code = table.Code,
                    BrowserName = "Offline",
                    LastTransactionUser =emp.LatinName,
                    LastTransactionUserAr=emp.ArabicName
                });
            }
            List<GLJournalEntryDetails> JEDetails = new List<GLJournalEntryDetails>();
           foreach (var i  in InvoiceJournalEntryDetailsForAll)
            {
                JEDetails.Add(new GLJournalEntryDetails() 
                {     CostCenterId = i.CostCenterId,
                      Credit = i.Credit,
                      Debit = i.Debit,
                      DescriptionAr = i.DescriptionAr,
                      DescriptionEn = i.DescriptionEn,
                      FinancialAccountId = i.FinancialAccountId,
                      FinancialCode = i.FinancialCode,
                      FinancialName = i.FinancialName,
                      isCostSales = i.isCostSales,
                      JournalEntryId = i.JournalEntryId.Value,
                   
                });
            }
            gLJournalEntrydetailsCommand.AddRangeAsync(JEDetails);
          //var res=  await gLJournalEntrydetailsCommand.SaveChanges();

            recHistoryRepositoryCommand.AddRangeAsync(historyList);
            //res = await recHistoryRepositoryCommand.SaveChanges();

            journalEntryCommand.CommitTransaction();
            return true;// res>0? true: false;
        }
        public int AutocodeOgJournalEntry()
        {
            var Code = 1;
            Code = journalEntryQuery.GetMaxCode(e => e.Code);

            if (Code != null )
                Code++;
            if (Code == 0)
                Code++;
            return Code;
        }
        public Tuple<string,string> getInvoiceName(DocumentType docType)
        {
           
            string invoiceNameAr = "";
            string invoiceNameEn = "";

            if (docType == DocumentType.POS)
            {
                invoiceNameAr = InvoiceTransaction.POSAr;
                invoiceNameEn = InvoiceTransaction.POSEn;
            }
            else if (docType == DocumentType.ReturnPOS)
            {
                invoiceNameAr = InvoiceTransaction.returnPOSAr;
                invoiceNameEn = InvoiceTransaction.returnPOSEn;
            }
            return new Tuple<string, string>(invoiceNameAr, invoiceNameEn);
        }
    }
}
