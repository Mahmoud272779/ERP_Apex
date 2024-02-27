using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.General
{
    public class currentUserInformation
    {
        public bool posAddDiscount { get; set; }
        public bool posAllowCreditSales { get; set; }
        public bool posEditOtherPersonsInv { get; set; }
        public bool posShowOtherPersonsInv { get; set; }
        public bool posShowReportsOfOtherPersons { get; set; }
        public bool allowCloseCloudPOSSession { get; set; }
        public bool canShowAllPOSSessions { get; set; }
        //POS Payments
        public bool posCashPayment { get; set; }
        public bool posNetPayment { get; set; }
        public bool posOtherPayment { get; set; }
        /******************************************************************************/
        //Sales
        public bool salesAddDiscount { get; set; }
        public bool salesAllowCreditSales { get; set; }
        public bool salesEditOtherPersonsInv { get; set; }
        public bool salesShowOtherPersonsInv { get; set; }
        public bool salesShowReportsOfOtherPersons { get; set; }
        //Sales Payments
        public bool salesCashPayment { get; set; }
        public bool salesNetPayment { get; set; }
        public bool salesOtherPayment { get; set; }
        /******************************************************************************/
        //Purchases
        public bool purchasesAddDiscount { get; set; }
        public bool purchasesAllowCreditSales { get; set; }
        public bool purchasesEditOtherPersonsInv { get; set; }
        public bool purchasesShowOtherPersonsInv { get; set; }
        public bool purchasesShowReportsOfOtherPersons { get; set; }
        //Purchases Payments
        public bool PurchasesCashPayment { get; set; }
        public bool PurchasesNetPayment { get; set; }
        public bool PurchasesOtherPayment { get; set; }
        /******************************************************************************/
        //General
        public bool showHistory { get; set; }
        public bool accredditForAllUsers { get; set; }
        public bool showCustomersOfOtherUsers { get; set; }
        public bool showOfferPricesOfOtherUser { get; set; }
        public bool showDashboardForAllUsers { get; set; }
        /******************************************************************************/
        //Branches
        public bool showAllBranchesInCustomerInfo { get; set; }
        public bool showAllBranchesInSuppliersInfo { get; set; }
    }
}
