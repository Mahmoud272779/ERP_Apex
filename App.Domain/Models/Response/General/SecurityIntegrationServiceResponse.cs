using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.General
{
    public class Apps
    {
        public int Id { get; set; }
        public string appNameAr { get; set; }
        public string appNameEn { get; set; }
    }
    public class AdditionalPrices
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public class companyInfomation
        {
            public List<Apps> apps { get; set; }
            public List<AdditionalPrices> AdditionalPrices { get; set; }
            //company information
            public int companyId { get; set; }
            public int bundleId { get; set; }
            public string companyLogin { get; set; }
            public string companyName { get; set; }
            public string databaseName { get; set; }
            public string Email { get; set; }
            public string bundleAr { get; set; }
            public string bundleEn { get; set; }
            public DateTime startPeriod { get; set; }
            public DateTime endPeriod { get; set; }
            //sub information
            public int Months { get; set; }
            public int periodId { get; set; }
            public int requestId { get; set; }
            public int periodType { get; set; }
            //
            public bool isInfinityNumbersOfUsers { get; set; }
            public int AllowedNumberOfUser { get; set; }
            //
            public bool isInfinityNumbersOfPOS { get; set; }
            public int AllowedNumberOfPOS { get; set; }
            //
            public bool isInfinityNumbersOfEmployees { get; set; }
            public int AllowedNumberOfEmployee { get; set; }
            //
            public bool isInfinityNumbersOfStores { get; set; }
            public int AllowedNumberOfStore { get; set; }
            //
            public bool isInfinityNumbersOfApps { get; set; } = false;
            public int AllowedNumberOfApps { get; set; }//Number of allowed application for the bundle
                                                        //
            public bool isInfinityNumbersOfBranchs { get; set; } = false;
            public int AllowedNumberOfBranchs { get; set; }//Number of allowed stores Of Branchs
                                                           //
            public bool isInfinityNumbersOfInvoices { get; set; } = false;
            public int AllowedNumberOfInvoices { get; set; }//Number of allowed stores Of Invoices
                                                            //
            public bool isInfinityNumbersOfSuppliers { get; set; } = false;
            public int AllowedNumberOfSuppliers { get; set; }//Number of allowed stores Of Suppliers
                                                             //
            public bool isInfinityNumbersOfCustomers { get; set; } = false;
            public int AllowedNumberOfCustomers { get; set; }//Number of allowed stores Of Suppliers

            public bool isInfinityItems { get; set; } = false;
            public int AllowedNumberOfItems { get; set; }//Number of allowed stores Of Items
                                                         //
            
            public int AllowedNumberOfExtraOfflinePOS { get; set; }//Number of allowed stores Of Items
        }
    }
}
