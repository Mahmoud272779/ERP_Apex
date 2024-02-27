using App.Domain.Models.Response.GeneralLedger;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GeneralLedgerHomeData
{
    public class GeneralLedgerHomeDataHandler : IRequestHandler<GeneralLedgerHomeDataRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLJournalEntryDetails> _GLJournalEntryDetailsQuery;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRoundNumbers _roundNumbers;
        public GeneralLedgerHomeDataHandler(IRepositoryQuery<GLJournalEntryDetails> gLJournalEntryDetailsQuery, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRoundNumbers roundNumbers)
        {
            _GLJournalEntryDetailsQuery = gLJournalEntryDetailsQuery;
            _GlRecieptsQuery = glRecieptsQuery;
            _roundNumbers = roundNumbers;
        }

        public async Task<ResponseResult> Handle(GeneralLedgerHomeDataRequest request, CancellationToken cancellationToken)
        {
            DateTime date = DateTime.Now;

            var currentMonth = new DateTime(date.Year, date.Month, 1);
            var monthDays = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
            var currentDay = DateTime.Now.Day;
            var LastMonth = DateTime.Now.AddMonths(-1).Date;
            var LastMonthStart = new DateTime(LastMonth.Year, LastMonth.Month, 1);
            var lastMonthDays = DateTime.DaysInMonth(LastMonthStart.Year, LastMonthStart.Month);
            var lastMonthEnd = new DateTime(LastMonth.Year, LastMonth.Month, lastMonthDays);
            if (request.dateFrom == null)
                request.dateFrom = currentMonth;
            if (request.dateTo == null)
                request.dateTo = DateTime.Now.Date;

            var JournalEntryDetails = _GLJournalEntryDetailsQuery.TableNoTracking.Include(x => x.journalEntry).Where(x => !x.journalEntry.IsBlock);
            var JE_currentMonth = JournalEntryDetails.Where(x => currentMonth <= x.journalEntry.FTDate.Value.Date && DateTime.Now.Date >= x.journalEntry.FTDate.Value.Date);
            var JE_LastMonth = JournalEntryDetails.Where(x => lastMonthEnd >= x.journalEntry.FTDate && LastMonthStart <= x.journalEntry.FTDate);
            var Reciepts = _GlRecieptsQuery.TableNoTracking.Where(x => !x.IsBlock).Where(x => x.RecieptDate.Date >= request.dateFrom.Value.Date && x.RecieptDate.Date <= request.dateTo.Value.Date);


            #region incoming
            var Incoming_currentMonth = JE_currentMonth.Where(x => x.FinancialAccountId.ToString().StartsWith(((int)accountantTree.FinanicalAccountDefultIds.revenues).ToString())).Sum(x => x.Credit - x.Debit);
            var Incoming_LastMonth = JE_LastMonth.Where(x => x.FinancialAccountId.ToString().StartsWith(((int)accountantTree.FinanicalAccountDefultIds.revenues).ToString())).Sum(x => x.Credit - x.Debit);
            var HomeDataResponse_Incoming = new HomeDataResponse_Incoming
            {
                monthDays = monthDays,
                currentDay = currentDay,
                currentMonth = _roundNumbers.GetRoundNumber(Incoming_currentMonth),
                lastMonth = _roundNumbers.GetRoundNumber(Incoming_LastMonth),
                percent = Incoming_LastMonth == 0 && Incoming_currentMonth == 0 ? 0 : (Incoming_LastMonth == 0 && Incoming_currentMonth != 0 ? 100 : _roundNumbers.GetRoundNumber(((Incoming_currentMonth - Incoming_LastMonth) / Incoming_LastMonth) * 100))
            };
            #endregion
            #region Outgoing
            var Outgoing_currentMonth = JE_currentMonth.Where(x => x.FinancialAccountId.ToString().StartsWith(((int)accountantTree.FinanicalAccountDefultIds.expensesAndCosts).ToString())).Sum(x => x.Debit - x.Credit);
            var Outgoing_LastMonth = JE_LastMonth.Where(x => x.FinancialAccountId.ToString().StartsWith(((int)accountantTree.FinanicalAccountDefultIds.expensesAndCosts).ToString())).Sum(x => x.Debit - x.Credit);


            var HomeDataResponse_Outgoing = new HomeDataResponse_Outgoing
            {
                monthDays = monthDays,
                currentDay = currentDay,
                currentMonth = _roundNumbers.GetRoundNumber(Outgoing_currentMonth),
                lastMonth = _roundNumbers.GetRoundNumber(Outgoing_LastMonth),
                percent = Outgoing_LastMonth == 0 && Outgoing_currentMonth == 0 ? 0 : (Outgoing_LastMonth == 0 && Outgoing_currentMonth != 0 ? 100: _roundNumbers.GetRoundNumber(((Outgoing_currentMonth - Outgoing_LastMonth) / Outgoing_LastMonth) * 100))
            };
            #endregion
            #region profit
            var HomeDataResponse_profit = new HomeDataResponse_profit
            {
                monthDays = monthDays,
                currentDay = currentDay,
                currentMonth = _roundNumbers.GetRoundNumber(Incoming_currentMonth - Outgoing_currentMonth),
                lastMonth = _roundNumbers.GetRoundNumber(Incoming_LastMonth - Outgoing_LastMonth),
                percent = Incoming_currentMonth == 0 && Incoming_currentMonth == 0 ? 0 :(Incoming_currentMonth == 0 && Incoming_currentMonth != 0 ? 100 : _roundNumbers.GetRoundNumber(((Incoming_currentMonth - Outgoing_currentMonth) / Incoming_currentMonth) * 100))

            };


            #endregion
            #region Safes Transaction
            var safeReciepts = Reciepts.Where(x => x.SafeID != null && x.BankId == null);
            var safes_Incoming = safeReciepts.Where(x => x.Signal > 0).Sum(x => x.Amount);
            var safes_outgoing = safeReciepts.Where(x => x.Signal < 0).Sum(x => x.Amount);
            var HomeDataResponse_safesTransaction = new HomeDataResponse_safesTransaction
            {
                incoming = Math.Abs(_roundNumbers.GetRoundNumber(safes_Incoming)),
                outgoing = Math.Abs(_roundNumbers.GetRoundNumber(safes_outgoing)),
                balance = _roundNumbers.GetRoundNumber(Math.Abs(safes_Incoming) - Math.Abs(safes_outgoing))
            };
            #endregion
            #region Banks Transaction 
            var BanksReciepts = Reciepts.Where(x => x.SafeID == null && x.BankId != null);
            var banks_Incoming = BanksReciepts.Where(x => x.Signal > 0).Sum(x => x.Amount);
            var banks_outgoing = BanksReciepts.Where(x => x.Signal < 0).Sum(x => x.Amount);
            var HomeDataResponse_banksTransaction = new HomeDataResponse_banksTransaction
            {
                incoming = Math.Abs(_roundNumbers.GetRoundNumber(banks_Incoming)),
                outgoing = Math.Abs(_roundNumbers.GetRoundNumber(banks_outgoing)),
                balance = _roundNumbers.GetRoundNumber(Math.Abs(banks_Incoming) - Math.Abs(banks_outgoing))
            };
            #endregion
            #region newestJournalEntry
            var newestJournalEntry = JournalEntryDetails
                                    .GroupBy(x => x.JournalEntryId)
                                    .Select(x => x.FirstOrDefault())
                                    .ToList()
                                    .OrderByDescending(x => x.JournalEntryId)
                                    .Take(5).Select(x => new HomeDataResponse_newestJournalEntry_Detalies
                                    {
                                        journalEntryCode = x.journalEntry.Code,
                                        note = !string.IsNullOrEmpty(x.journalEntry.Notes) ? x.journalEntry.Notes : x.journalEntry.Code.ToString()
                                    });
            var HomeDataResponse_newestJournalEntry = new HomeDataResponse_newestJournalEntry
            {
                HomeDataResponse_newestJournalEntry_Detalies = newestJournalEntry.ToList()
            };
            #endregion
            #region Incoming And Outgoing Transaction
            var listOf_HomeDataResponse_incomingAndOutgoingTransaction_Detalies = new List<HomeDataResponse_incomingAndOutgoingTransaction_Detalies>();
            for (int i = 0; i < currentDay; i++)
            {
                var JE_OfCurrentIndex = JE_currentMonth.Where(x => x.journalEntry.FTDate.Value.Date == new DateTime(date.Year, date.Month, i + 1));
                listOf_HomeDataResponse_incomingAndOutgoingTransaction_Detalies.Add(new HomeDataResponse_incomingAndOutgoingTransaction_Detalies
                {
                    index = i + 1,
                    incoming = Math.Abs(_roundNumbers.GetRoundNumber(JE_OfCurrentIndex.Where(x => x.FinancialAccountId.ToString().StartsWith(((int)accountantTree.FinanicalAccountDefultIds.revenues).ToString())).Sum(x => x.Credit - x.Debit))),
                    outgoing = Math.Abs(_roundNumbers.GetRoundNumber(JE_OfCurrentIndex.Where(x => x.FinancialAccountId.ToString().StartsWith(((int)accountantTree.FinanicalAccountDefultIds.expensesAndCosts).ToString())).Sum(x => x.Credit - x.Debit)))
                });
            }
            var maxIncoming = listOf_HomeDataResponse_incomingAndOutgoingTransaction_Detalies.Select(x => x.incoming).Max();
            var maxOutgoing = listOf_HomeDataResponse_incomingAndOutgoingTransaction_Detalies.Select(x => x.outgoing).Max();
            var HomeDataResponse_incomingAndOutgoingTransaction = new HomeDataResponse_incomingAndOutgoingTransaction
            {
                incomingAndOutgoingTransactionDetalies = listOf_HomeDataResponse_incomingAndOutgoingTransaction_Detalies,
                maximumValue = maxIncoming >= maxOutgoing ? Math.Ceiling(maxIncoming) : Math.Ceiling(maxOutgoing)
            };
            #endregion

            var res = new HomeDataResponse()
            {
                HomeDataResponse_Incoming = HomeDataResponse_Incoming,
                HomeDataResponse_Outgoing = HomeDataResponse_Outgoing,
                HomeDataResponse_profit = HomeDataResponse_profit,
                HomeDataResponse_safesTransaction = HomeDataResponse_safesTransaction,
                HomeDataResponse_banksTransaction = HomeDataResponse_banksTransaction,
                HomeDataResponse_newestJournalEntry = HomeDataResponse_newestJournalEntry,
                HomeDataResponse_incomingAndOutgoingTransaction = HomeDataResponse_incomingAndOutgoingTransaction
            };
            return new ResponseResult
            {
                Result = Result.Success,
                Data = res
            };
        }
    }
}
