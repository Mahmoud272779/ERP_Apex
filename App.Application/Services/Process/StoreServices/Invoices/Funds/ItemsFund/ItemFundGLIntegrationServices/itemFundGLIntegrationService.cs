using App.Application.Handlers.GeneralLedger.JournalEntry;
using MediatR;

namespace App.Application.Services.Process.StoreServices.Invoices.Funds.ItemsFund.ItemFundGLIntegrationServices
{
    public class itemFundGLIntegrationService : iItemFundGLIntegrationService
    {
        private readonly IRepositoryCommand<GLJournalEntryDetails> _gLJournalEntryDetailsCommand;
        private readonly IRepositoryQuery<GLJournalEntryDetails> _gLJournalEntryDetailsQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery;
        private readonly IRepositoryCommand<GLJournalEntry> gLJournalEntryCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly IMediator _mediator;

        public itemFundGLIntegrationService(IRepositoryCommand<GLJournalEntryDetails> GLJournalEntryDetailsCommand,
                                            IRepositoryQuery<GLJournalEntryDetails> GLJournalEntryDetailsQuery,
                                            IRepositoryQuery<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettingsQuery,
                                            IRepositoryQuery<GLJournalEntry> GLJournalEntryQuery,
                                            IRepositoryCommand<GLJournalEntry> GLJournalEntryCommand,
                                            iUserInformation iUserInformation,
                                            IMediator mediator

            )
        {
            _gLJournalEntryDetailsCommand = GLJournalEntryDetailsCommand;
            _gLJournalEntryDetailsQuery = GLJournalEntryDetailsQuery;
            _gLPurchasesAndSalesSettingsQuery = GLPurchasesAndSalesSettingsQuery;
            gLJournalEntryQuery = GLJournalEntryQuery;
            gLJournalEntryCommand = GLJournalEntryCommand;
            _iUserInformation = iUserInformation;
            _mediator = mediator;
        }
        public async Task AddItemFundJournalEntry(ItemFundJournalEntryDTO param)
        {
            var usreInfo = await _iUserInformation.GetUserInformation();
            var FA_Id = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => x.branchId == usreInfo.CurrentbranchId).Where(x => x.RecieptsType == (int)DocumentType.itemsFund);
            var creditorFA_Id = FA_Id.Where(x => x.ReceiptElemntID == (int)DebitoAndCredito.creditor).FirstOrDefault().FinancialAccountId;
            var debitFA_Id = FA_Id.Where(x => x.ReceiptElemntID == (int)DebitoAndCredito.debit).FirstOrDefault().FinancialAccountId;

            GLJournalEntry gLJournalEntry = new GLJournalEntry();
            if (!param.isUpdate && !param.isDelete)
            {
                var glDetails = new List<JournalEntryDetail>();
                glDetails.AddRange(new[]
                    {
                        new JournalEntryDetail()
                        {
                            Debit = param.totalAmount,
                            DescriptionEn = $"Item Entry Fund - {param.documentId}",
                            DescriptionAr = $"ارصدة اول المدة اصناف - {param.documentId}",
                            Credit = 0,
                            FinancialAccountId = debitFA_Id
                        },
                        new JournalEntryDetail()
                        {
                            Debit = 0,
                            DescriptionEn = "Item Entry Fund",
                            DescriptionAr = "ارصدة اول المدة اصناف",
                            Credit = param.totalAmount,
                            FinancialAccountId = creditorFA_Id,
                        }
                    });

                await _mediator.Send(new AddJournalEntryRequest
                {
                    BranchId = usreInfo.CurrentbranchId,
                    DocType = (int)DocumentType.itemsFund,
                    FTDate = param.date,
                    InvoiceId = param.documentId,
                    IsAccredit = true,
                    Notes = $"ارصدة اول المدة اصناف - {param.documentId}",
                    isAuto = true,
                    JournalEntryDetails = glDetails
                });
                //await _journalEntryBusiness.AddJournalEntry(new Domain.Models.Security.Authentication.Request.JournalEntryParameter()
                //{
                //    BranchId = usreInfo.CurrentbranchId,
                //    DocType = (int)DocumentType.itemsFund,
                //    FTDate = param.date,
                //    InvoiceId = param.documentId,
                //    IsAccredit = true,
                //    Notes = $"ارصدة اول المدة اصناف - {param.documentId}",
                //    isAuto = true,
                //    JournalEntryDetails = glDetails
                //});
            }
            else if (param.isUpdate || !param.isDelete)
            {
                var journalEntryId = gLJournalEntryQuery.TableNoTracking.Where(x => x.InvoiceId == param.documentId && x.DocType == (int)DocumentType.itemsFund).FirstOrDefault()?.Id ?? 0;
                if (journalEntryId == 0)
                    return;
                var jgDetails = new List<JournalEntryDetail>();
                jgDetails.AddRange(new[]
                {
                    new JournalEntryDetail()
                        {
                            Debit = param.totalAmount,
                            DescriptionEn = $"Item Entry Fund - {param.documentId}",
                            DescriptionAr = $"ارصدة اول المدة اصناف - {param.documentId}",
                            Credit = 0,
                            FinancialAccountId = debitFA_Id
                    },
                    new JournalEntryDetail()
                    {
                        Debit = 0,
                        DescriptionEn = "Item Entry Fund",
                        DescriptionAr = "ارصدة اول المدة اصناف",
                        Credit = param.totalAmount,
                        FinancialAccountId = creditorFA_Id,
                    }
                });
                await _mediator.Send(new UpdateJournalEntryRequest
                {
                    BranchId = usreInfo.CurrentbranchId,
                    FTDate = param.date,
                    Notes = $"ارصدة اول المدة اصناف - {param.documentId}",
                    fromSystem = true,
                    IsAccredit = true,
                    Id = journalEntryId,
                    journalEntryDetails = jgDetails
                });
                //await _journalEntryBusiness.UpdateJournalEntry(new UpdateJournalEntryParameter()
                //{
                //    BranchId = usreInfo.CurrentbranchId,
                //    FTDate = param.date,
                //    Notes = $"ارصدة اول المدة اصناف - {param.documentId}",
                //    fromSystem = true,
                //    IsAccredit = true,
                //    Id = journalEntryId,
                //    journalEntryDetails = jgDetails
                //});
            } else if (!param.isUpdate || param.isDelete)
            {
                var journalEntryId = gLJournalEntryQuery.TableNoTracking.Where(x => x.InvoiceId == param.documentId && x.DocType == (int)DocumentType.itemsFund).Select(c => c.Id).ToArray();
                await _mediator.Send(new BlockJournalEntryReqeust
                {
                    Ids= journalEntryId
                });
                //await _journalEntryBusiness.BlockJournalEntry(new BlockJournalEntry()
                //{
                //    Ids = journalEntryId
                //});
            }

            #region old
            //if (param.isUpdate || param.isDelete)
            //{
            //    await _gLJournalEntryDetailsCommand.DeleteAsync(x => x.JournalEntryId == -6 && x.StoreFundId == param.documentId && x.isStoreFund == true && x.DocType == (int)DocumentType.itemsFund);
            //}
            //if (!param.isDelete)
            //{

            //    var add = _gLJournalEntryDetailsCommand.Add(new GLJournalEntryDetails()
            //    {
            //        Debit = param.totalAmount,
            //        DescriptionEn = $"Item Entry Fund - {param.documentId}",
            //        DescriptionAr = $"ارصدة اول المدة اصناف - {param.documentId}",
            //        Credit = 0,
            //        DocType = (int)DocumentType.itemsFund,
            //        FinancialAccountId = (int)FinanicalAccountDefultIds.FirstTermMerchandise,
            //        JournalEntryId = -6, // This is the static journal Entry in database for item fund
            //        isStoreFund = true,
            //        StoreFundId = param.documentId,

            //    });
            //    await _gLJournalEntryDetailsCommand.SaveAsync();


            // var JEntery = _gLJournalEntryDetailsQuery.TableNoTracking.Include(a => a.journalEntry).Where(a => a.JournalEntryId == -6);
            //    var firstStoreFundId = JEntery.Where(x => x.StoreFundId != null).FirstOrDefault().StoreFundId;
            //    if (JEntery.Where(x=> x.StoreFundId != null).OrderBy(x=> x.StoreFundId).FirstOrDefault().StoreFundId == param.documentId)
            //    {
            //        //JEntery.FirstOrDefault().journalEntry.FTDate = param.date;
            //        var masterJournalEnrey = await gLJournalEntryQuery.GetByIdAsync(-6);
            //        masterJournalEnrey.FTDate = param.date;
            //        var updated = await gLJournalEntryCommand.UpdateAsyn(masterJournalEnrey);
            //    }
            //}

            //var itemFundJournalEntry = _gLJournalEntryDetailsQuery.TableNoTracking.Where(x => x.DocType == (int)DocumentType.itemsFund && x.JournalEntryId == -6);
            //var checkIfMasterJournalIsExist = itemFundJournalEntry.Where(x => x.StoreFundId == null).FirstOrDefault();
            //if (checkIfMasterJournalIsExist != null)
            //{
            //    var total = itemFundJournalEntry.Where(x => x.StoreFundId != null).Sum(x => x.Debit);
            //    checkIfMasterJournalIsExist.Credit = total;
            //    if(total == 0)
            //    {
            //        await _gLJournalEntryDetailsCommand.DeleteAsync(x=> x.Id == checkIfMasterJournalIsExist.Id);
            //    }
            //    else
            //    {
            //        await _gLJournalEntryDetailsCommand.UpdateAsyn(checkIfMasterJournalIsExist);
            //    }

            //}
            //else
            //{
            //    _gLJournalEntryDetailsCommand.Add(new GLJournalEntryDetails()
            //    {
            //        Debit = 0,
            //        DescriptionEn = "Item Entry Fund",
            //        DescriptionAr = "ارصدة اول المدة اصناف",
            //        Credit = param.totalAmount,
            //        DocType = (int)DocumentType.itemsFund,
            //        FinancialAccountId = FA_Id,
            //        JournalEntryId = -6, // This is the static journal Entry in database for item fund
            //        isStoreFund = true,
            //    });
            //    await _gLJournalEntryDetailsCommand.SaveAsync();
            //}
            #endregion
        }
    }
}
