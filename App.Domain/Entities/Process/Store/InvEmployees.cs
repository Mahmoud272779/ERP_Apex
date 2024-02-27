using App.Domain.Common;
using App.Domain.Entities.Notification;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Process.Store;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Domain.Entities.Process
{
    public class InvEmployees
    {
        public int Id { get; set; }
        public int Code { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the Employee 1 if active 2 if inactive
        public string Notes { get; set; }
        public IFormFile Image { get; set; }
        public string ImagePath { get; set; }
        //public int branch_Id { get; set; }
        public int JobId { get; set; }
        public int? FinancialAccountId { get; set; }
        //public virtual ICollection<GLBranch> Branches { get; set; }
        public int? SalesPriceId { get; set; }
        public virtual ICollection<InvEmployeeBranch> EmployeeBranches { get; set; }
        public InvJobs Job { get; set; }
        public ICollection<userAccount> userAccount { get; set; }
        public ICollection<InvoiceMaster> invoiceMasters { get; set; }
        public ICollection<POSInvoiceSuspension> POSInvoiceSuspension { get; set; }
        public ICollection<SystemHistoryLogs> SystemHistoryLogs { get; set; }
        public ICollection<signalR> signalR { get; set; }
        public ICollection<InvPersons> InvPersons { get; set; }
        public ICollection<POSSession> pOSSessionsStart { get; set; }
        public ICollection<POSSession> pOSSessionsEnd { get; set; }



        public ICollection<chatMessages> chatMessagesFrom { get; set; }
        public ICollection<chatMessages> chatMessagesTo { get; set; }
        public ICollection<chatGroups> chatGroups { get; set; }
        public ICollection<chatGroupMembers> chatGroupMembers { get; set; }
        public ICollection<POSSessionHistory> pOSSessionHistories { get; set; }
        public ICollection<NotificationsMaster> NotificationsMaster { get; set; }
        public ICollection<NotificationsMaster> NotificationsMaster_insertedBy { get; set; }
        public ICollection<NotificationSeen> NotificationSeen { get; set; }



        public GLFinancialAccount FinancialAccount { get; set; }
        public bool CanDelete { get; set; }


        public string UserId { get; set; }

        public DateTime UTime { get; set; }

        public virtual ICollection<OfferPriceMaster> OfferPriceMaster { get; set; }


    }
}
