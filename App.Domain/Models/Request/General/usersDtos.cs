using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.General
{
    public class addUsersDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public int employeeId { get; set; }
        public int Status { get; set; }

    }
    public class editUsersDto: addUsersDto
    {
        public int Id { get; set; }
    }
    public class deleteUsersDto
    {
        public int[] Ids { get; set; }
    }
    public class getUsersDto 
    {
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
        public string SearchCriteria { get; set; }
        public int Status { get; set; }
    }
    public class OtherSettingsDto
    {
        public int? Id { get; set; }
        public int userAccountId { get; set; }

        //POS
        public bool? posAddDiscount { get; set; } = false;
        public bool? posAllowCreditSales { get; set; } = false;
        public bool? posEditOtherPersonsInv { get; set; } = false;
        public bool? posShowOtherPersonsInv { get; set; } = false;
        public bool? posShowReportsOfOtherPersons { get; set; } = false;
        public bool? canShowAllPOSSessions { get; set; } = true;
        public bool? allowCloseCloudPOSSession { get; set; } = true;
        //POS Payments
        public bool? posCashPayment { get; set; } = false;
        public bool? posNetPayment { get; set; } = false;
        public bool? posOtherPayment { get; set; } = false;
        /******************************************************************************/
        //Sales
        public bool? salesAddDiscount { get; set; } = false;
        public bool? salesAllowCreditSales { get; set; } = false;
        public bool? salesEditOtherPersonsInv { get; set; } = false;
        public bool? salesShowOtherPersonsInv { get; set; } = false;
        public bool? salesShowReportsOfOtherPersons { get; set; } = false;
        

        //Sales Payments
        public bool? salesCashPayment { get; set; } = false;
        public bool? salesNetPayment { get; set; } = false;
        public bool? salesOtherPayment { get; set; } = false;
        public bool? salesShowBalanceOfPerson { get; set; } = false;
        public bool? salesShowAllPersonBalance { get; set; } = false;
        public bool? salesShowDebitorPersonBalance { get; set; }=false;
        public bool? salesShowCreditorPersonBaalance { get; set; }=false;
        public bool? salesPaymentFromInsideInvoice { get; set; } = false;
        /******************************************************************************/
        //Purchases
        public bool? purchasesAddDiscount { get; set; } = false;
        public bool? purchasesAllowCreditSales { get; set; } = false;
        public bool? purchasesEditOtherPersonsInv { get; set; } = false;
        public bool? purchasesShowOtherPersonsInv { get; set; } = false;
        public bool? purchasesShowReportsOfOtherPersons { get; set; } = false;
        //Purchases Payments
        public bool? PurchasesCashPayment { get; set; } = false;
        public bool? PurchasesNetPayment { get; set; } = false;
        public bool? PurchasesOtherPayment { get; set; } = false;
        public bool? purchaseShowBalanceOfPerson { get; set; } = false;
        public bool purchaseShowAllPersonBalance { get; set; }
        public bool purchaseShowDebitorPersonBalance { get; set; }
        public bool purchaseShowCreditorPersonBaalance { get; set; }
        public bool PurchasesPaymentFromInsideInvoice { get; set; }
        public bool PurchasesallowEdidJournalEntry { get; set; }
        /******************************************************************************/
        //General
        public bool? showHistory { get; set; } = false;
        public bool? accredditForAllUsers { get; set; } = false;
        public bool? showCustomersOfOtherUsers { get; set; } = false;
        public bool? showOfferPricesOfOtherUser { get; set; } = false;
        public bool showDashboardForAllUsers { get; set; } = false;
        public bool AllowPrintBarcode { get; set; } = true;
        public bool CollectionReceipts { get; set; } = true;
        /******************************************************************************/
        //Branches
        public bool? showAllBranchesInCustomerInfo { get; set; } = false;
        public bool? showAllBranchesInSuppliersInfo { get; set; } = false;

        public int[] storeIds { get; set; }
        public int[] bankIds { get; set; }
        public int[] safeIds { get; set; }


    }
}
