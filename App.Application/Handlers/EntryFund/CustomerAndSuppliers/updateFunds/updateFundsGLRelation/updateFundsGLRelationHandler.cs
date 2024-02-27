using App.Application.Handlers.GeneralLedger;
using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Domain.Models.Security.Authentication.Request;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.EntryFund.CustomerAndSuppliers.updateFunds.updateFundsGLRelation
{
    public class updateFundsGLRelationHandler : IRequestHandler<updateFundsGLRelationRequest, bool>
    {
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _GLJournalEntryQuery;
        private readonly IRepositoryCommand<GLJournalEntry> _GLJournalEntryCommand;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IMediator _mediator;

        public updateFundsGLRelationHandler(IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuery, IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery, IRepositoryCommand<GLJournalEntry> gLJournalEntryCommand, IRepositoryQuery<InvPersons> invPersonsQuery)
        {
            _gLPurchasesAndSalesSettingsQuery = gLPurchasesAndSalesSettingsQuery;
            _GLJournalEntryQuery = gLJournalEntryQuery;
            _GLJournalEntryCommand = gLJournalEntryCommand;
            _InvPersonsQuery = invPersonsQuery;
        }

        public async Task<bool> Handle(updateFundsGLRelationRequest request, CancellationToken cancellationToken)
        {
            var GLSettings = _gLPurchasesAndSalesSettingsQuery
                .TableNoTracking
                .Where(c => c.branchId == request.branchId)
                .Where(x => request.isCustomer ? x.RecieptsType == (int)DocumentType.CustomerFunds : x.RecieptsType == (int)DocumentType.SuplierFunds && x.branchId == request.branchId);
            int Fund_FAId = GLSettings.OrderBy(x => x.Id).LastOrDefault().FinancialAccountId ?? 0;

            var DocType = (request.isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds);

            var journalEntry = _GLJournalEntryQuery
                .TableNoTracking
                .Where(c => c.BranchId == request.branchId)
                .Where(c => c.DocType == DocType)
                .FirstOrDefault();

            if (journalEntry == null)
            {
                var nextCode = (_GLJournalEntryQuery.TableNoTracking.OrderBy(c => c.Code).LastOrDefault()?.Code ?? 0) + 1;
                var _journalEntry = new GLJournalEntry
                {
                    DocType = (request.isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds),
                    Auto = true,
                    BranchId = request.branchId,
                    Code = nextCode,
                    CurrencyId = 1,
                    Notes = request.isCustomer ? "ارصدة اول المدة عملاء" : "ارصدة اول المدة موردين",
                    FTDate = DateTime.Now
                };
                _GLJournalEntryCommand.Add(_journalEntry);

                journalEntry = _GLJournalEntryQuery
                .TableNoTracking
                .Where(c => c.BranchId == request.branchId)
                .Where(c => c.DocType == (request.isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds))
                .FirstOrDefault();
                _GLJournalEntryCommand.ClearTracking();
            }


            if (request.isUpdate)
                await DeletePersonFundFromJournalEntry(request.supAndCustUpdateFunds.Select(x => x.Id).ToArray(), request.isCustomer, journalEntry.Id);

            var invPersons = _InvPersonsQuery.TableNoTracking;
            var EntryFundsList = new List<EntryFunds>();
            int docId = journalEntry.Id;
            foreach (var item in request.supAndCustUpdateFunds)
            {
                if (EntryFundsList.Where(x => x.StoreFundId == item.Id).Any())
                    continue;

                var TotalAmount = item.Credit - item.Debit;
                var recNoteAr = request.isCustomer ? "ارصدة اول المدة عملاء" : "ارصدة اول المدة موردين";
                var recNoteEn = request.isCustomer ? "Customer Entry Fund" : "Supplier Entry Fund";

                var personFA_Id = invPersons.Where(x => x.Id == item.Id).FirstOrDefault().FinancialAccountId;

                EntryFundsList.AddRange(new[]
                                            {new EntryFunds
                                             {
                                                 Credit = TotalAmount > 0 ? TotalAmount : 0,
                                                 Debit = TotalAmount < 0 ? TotalAmount * -1 : 0,
                                                 DescriptionAr = recNoteAr,
                                                 DescriptionEn = recNoteEn,
                                                 FinancialAccountId = personFA_Id,
                                                 isStoreFund = true,
                                                 StoreFundId = item.Id,
                                                 JournalEntryId = docId,
                                                 DocType = request.isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds
                                             }
                });
            }
            var journalEntrySaed = await _mediator.Send(new addEntryFundsRequest
            {
                EntryFunds = EntryFundsList,
                date = request.date,
                note = "",
                isFund = true,
                docId = docId,
                Fund_FAId = Fund_FAId

            });



            return true;
        }
        public async Task DeletePersonFundFromJournalEntry(int[] ids, bool isCustomer, int journalId)
        {
            //var tryDelete = await _journalEntryBusiness.removeStoreFundFromJournalDetiales(ids, isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds);
            var tryDelete = await _mediator.Send(new removeStoreFundFromJournalDetialesRequest
            {
                storeFundIds = ids,
                DocType = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds,
                journalId = journalId
            });

        }
    }
}
