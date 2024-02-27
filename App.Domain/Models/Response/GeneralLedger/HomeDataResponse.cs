using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.GeneralLedger
{
    public class HomeDataResponse
    {
        public HomeDataResponse_Incoming HomeDataResponse_Incoming { get; set; }
        public HomeDataResponse_Outgoing HomeDataResponse_Outgoing { get; set; }
        public HomeDataResponse_profit HomeDataResponse_profit { get; set; }
        public HomeDataResponse_safesTransaction HomeDataResponse_safesTransaction { get; set; }
        public HomeDataResponse_banksTransaction HomeDataResponse_banksTransaction { get; set; }
        public HomeDataResponse_newestJournalEntry HomeDataResponse_newestJournalEntry { get; set; }
        public HomeDataResponse_incomingAndOutgoingTransaction HomeDataResponse_incomingAndOutgoingTransaction { get; set; }
    }

    public class HomeDataResponse_Incoming
    {
        public double percent { get; set; }
        public double currentMonth { get; set; }
        public double lastMonth { get; set; }
        public int monthDays { get; set; }
        public int currentDay { get; set; }
    }
    public class HomeDataResponse_Outgoing
    {
        public double percent { get; set; }
        public double currentMonth { get; set; }
        public double lastMonth { get; set; }
        public int monthDays { get; set; }
        public int currentDay { get; set; }
    }
    public class HomeDataResponse_profit
    {
        public double percent { get; set; }
        public double currentMonth { get; set; }
        public double lastMonth { get; set; }
        public int monthDays { get; set; }
        public int currentDay { get; set; }
    }
    public class HomeDataResponse_safesTransaction
    {
        public double incoming { get; set; }
        public double outgoing { get; set; }
        public double balance { get; set; }
    }
    public class HomeDataResponse_banksTransaction
    {
        public double incoming { get; set; }
        public double outgoing { get; set; }
        public double balance { get; set; }

    }
    public class HomeDataResponse_newestJournalEntry
    {
        public List<HomeDataResponse_newestJournalEntry_Detalies> HomeDataResponse_newestJournalEntry_Detalies { get; set; }
    }
    public class HomeDataResponse_newestJournalEntry_Detalies
    {
        public int journalEntryCode { get; set; }
        public string note { get; set; }
    }
    public class HomeDataResponse_incomingAndOutgoingTransaction
    {
        public double maximumValue { get; set; }
        public List<HomeDataResponse_incomingAndOutgoingTransaction_Detalies> incomingAndOutgoingTransactionDetalies { get; set; }
    }
    public class HomeDataResponse_incomingAndOutgoingTransaction_Detalies
    {
        public int index { get; set; }
        public double incoming { get; set; }
        public double outgoing { get; set; }
    }
}
