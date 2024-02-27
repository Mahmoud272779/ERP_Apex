using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryHelper;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Infrastructure.settings;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.GetJournalEntryByFinancialAccountCode
{
    public class GetJournalEntryByFinancialAccountCodeHandler : BusinessBase<GLJournalEntry>,IRequestHandler<GetJournalEntryByFinancialAccountCodeRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery;
        private readonly IPagedList<GLJournalEntryDetails, GLJournalEntryDetails> getJournalEntryByFinancialAccountCodeResponseDto;
        private readonly IRoundNumbers roundNumbers;

        public GetJournalEntryByFinancialAccountCodeHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery, IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery, IPagedList<GLJournalEntryDetails, GLJournalEntryDetails> getJournalEntryByFinancialAccountCodeResponseDto, IRoundNumbers roundNumbers) : base(repositoryActionResult)
        {
            this.journalEntryDetailsRepositoryQuery = journalEntryDetailsRepositoryQuery;
            this.generalSettingsRepositoryQuery = generalSettingsRepositoryQuery;
            this.getJournalEntryByFinancialAccountCodeResponseDto = getJournalEntryByFinancialAccountCodeResponseDto;
            this.roundNumbers = roundNumbers;
        }

        public async Task<IRepositoryActionResult> Handle(GetJournalEntryByFinancialAccountCodeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var costCenterData = journalEntryDetailsRepositoryQuery.TableNoTracking.Include(a => a.journalEntry)
                    .Where(s => s.FinancialAccountId == request.financialId && s.journalEntry.IsBlock != true && s.journalEntry.IsAccredit == true).ToList();
                var settings = generalSettingsRepositoryQuery.TableNoTracking.FirstOrDefault();
                var _res = getJournalEntryByFinancialAccountCodeResponseDto.GetGenericPagination(costCenterData, request.pageNumber, request.pageSize, Mapper);

                var res = new PaginationResponse()
                {
                    Data = _res.Data.Select(x => new
                    {
                        id = x.journalEntry.Id,
                        code = x.journalEntry.Code,
                        time = x.journalEntry.FTDate != null ? x.journalEntry.FTDate.Value.ToString(defultData.datetimeFormat) : null,
                        credit = roundNumbers.GetRoundNumber(x.Credit),
                        debit = roundNumbers.GetRoundNumber(x.Debit),
                        balanceAfterOperation = roundNumbers.GetRoundNumber(journalEntryHelper.countTotal(x.Id, request.financialId, costCenterData)),
                    }).Select(x => new
                    {
                        x.id,
                        x.code,
                        x.credit,
                        x.debit,
                        x.time,
                        balanceAfterOperation = x.balanceAfterOperation < 0 ? (x.balanceAfterOperation * -1) : x.balanceAfterOperation,
                        FA_Nature = x.balanceAfterOperation > 0 ? (int)FA_Nature.Credit : (int)FA_Nature.Debit
                    }),
                    ListCount = _res.ListCount,
                    PageNumber = _res.PageNumber,
                    PageSize = _res.PageSize,
                    TotalPages = _res.TotalPages
                };

                return repositoryActionResult.GetRepositoryActionResult(res, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
    }
}
