using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using static App.Domain.Enums.Enums;

namespace App.Domain.Entities
{
    public class userAccount
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public bool isActive { get; set; } = true;
        public int employeesId { get; set; }
        public int? permissionListId { get; set; }
 

        public ICollection<signinLogs> signinLogs { get; set; }
        public ICollection<userBranches> userBranches { get; set; }
        public ICollection<otherSettings> otherSettings { get; set; }
        public ICollection<UserAndPermission> UserAndPermission { get; set; }
        public ICollection<usersForgetPassword> usersForgetPasswords { get; set; }
        public permissionList permissionList { get; set; }
        public InvEmployees employees { get; set; }
        public DateTime UpdateTime { get; set; }


    }
    public class usersForgetPassword
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int userId { get; set; }
        public userAccount userAccount { get; set; }
    }
    public class signinLogs
    {
        public int Id { get; set; }
        public int userAccountid { get; set; }
        public string token { get; set; }
        public DateTime signinDateTime { get; set; }
        public DateTime lastActionTime { get; set; }
        public bool isLogout { get; set; }
        public bool isLocked { get; set; }
        public DateTime? logoutDateTime { get; set; }
        public userAccount userAccount { get; set; }
        public bool stayLoggedin { get; set; }
    }

    public class userBranches
    {
        public int Id { get; set; }
        public int userAccountId { get; set; }
        public int GLBranchId { get; set; }
        public userAccount userAccount { get; set; }
        public GLBranch GLBranch { get; set; }

    }
    public class otherSettings
    {
        public int Id { get; set; }


        //POS
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
        public bool salesShowBalanceOfPerson { get; set; }
        public bool salesShowAllPersonBalance { get; set; }
        public bool salesShowDebitorPersonBalance { get; set; }
        public bool salesShowCreditorPersonBaalance { get; set; }
        public bool salesPaymentFromInsideInvoice { get; set; }

        /******************************************************************************/
        //Purchases
        public bool purchasesAddDiscount { get; set; }
        public bool purchasesAllowCreditSales { get; set; }
        public bool purchasesEditOtherPersonsInv { get; set; }
        public bool purchasesShowOtherPersonsInv { get; set; }
        public bool purchasesShowReportsOfOtherPersons { get; set; }
        public bool purchaseShowBalanceOfPerson { get; set; }
        
        //Purchases Payments
        public bool PurchasesCashPayment { get; set; }
        public bool PurchasesNetPayment { get; set; }
        public bool PurchasesOtherPayment { get; set; }
        public bool purchaseShowAllPersonBalance { get; set; }
        public bool purchaseShowDebitorPersonBalance { get; set; }
        public bool purchaseShowCreditorPersonBaalance { get; set; }
        public bool PurchasesPaymentFromInsideInvoice { get; set; }
        public bool PurchasesallowEdidJournalEntry { get; set; } = false;

        /******************************************************************************/
        //General
        public bool showHistory { get; set; }
        public bool accredditForAllUsers { get; set; }
        public bool showCustomersOfOtherUsers { get; set; }
        public bool showOfferPricesOfOtherUser { get; set; }
        public bool showDashboardForAllUsers { get; set; }
        public bool AllowPrintBarcode { get; set; } = true;
        public bool CollectionReceipts { get; set; } = false;
        /******************************************************************************/
        //Branches
        public bool showAllBranchesInCustomerInfo { get; set; }
        public bool showAllBranchesInSuppliersInfo { get; set; }


        public int userAccountId { get; set; }
        public userAccount userAccount { get; set; }
        public ICollection<OtherSettingsStores> OtherSettingsStores { get; set; }
        public ICollection<OtherSettingsSafes> otherSettingsSafes { get; set; }
        public ICollection<OtherSettingsBanks> otherSettingsBanks { get; set; }
    }

    public class OtherSettingsStores
    {
        public int Id { get; set; }
        public int InvStpStoresId { get; set; }
        public int otherSettingsId { get; set; }
        public InvStpStores InvStpStores { get; set; }
        public otherSettings otherSettings { get; set; }
    }

    public class OtherSettingsSafes
    {
        public int Id { get; set; }
        public int gLSafeId { get; set; }
        public int otherSettingsId { get; set; }
        public GLSafe GLSafe { get; set; }
        public otherSettings otherSettings { get; set; }
    }

    public class OtherSettingsBanks
    {
        public int Id { get; set; }
        public int gLBankId { get; set; }
        public int otherSettingsId { get; set; }
        public GLBank GLBank { get; set; }
        public otherSettings otherSettings { get; set; }
    }
}
