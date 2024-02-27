using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class UserRequest
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyUniqueName { get; set; }
        public string CompanyActivity { get; set; }
        public int EmployeesCount { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AdminFullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string AdminUserName { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
        public int BundleId { get; set; }
        public int SubscriptionMonths { get; set; }
        public bool IsFixedAssetsSelected { get; set; }
        public bool IsHrrequestsSelected { get; set; }
        public bool IsPosselected { get; set; }
        public int NoOfUsers { get; set; }
        public int NoOfStores { get; set; }
        public int NoOfEmployees { get; set; }
        public decimal Total { get; set; }
        public bool IsCash { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string RequestFilePath { get; set; }
        public string Notes { get; set; }
        public bool IsVisa { get; set; }
        public bool IsBankTransfer { get; set; }
        public string ArabicBankName { get; set; }
        public string LatinBankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIban { get; set; }
        public bool IsInvoiceAsked { get; set; }
        public bool HasTaxNumber { get; set; }
        public string TaxNumber { get; set; }
        public DateTime TransferDepositDate { get; set; }
        public string TransferDepositeNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public bool HasAccountsApproved { get; set; }
        public string AccountsReason { get; set; }
        public string AccountsUserId { get; set; }
        public DateTime AccountsApprovalDate { get; set; }
        public bool HasTechSupportApproved { get; set; }
        public string TechSupportReason { get; set; }
        public string TechSupportUserId { get; set; }
        public DateTime TechSupportApprovalDate { get; set; }
        public int FollowUpSalesPersonId { get; set; }
        public string ConfirmationUserId { get; set; }
        public DateTime RequestConfirmationDate { get; set; }
        public bool IsNewSubscription { get; set; }
    }
}
