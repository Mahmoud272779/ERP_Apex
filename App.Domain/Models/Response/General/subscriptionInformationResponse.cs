using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.General
{
    public class subscriptionInformationResponse
    {
        public int bundleId { get; set; }
        public List<Apps> apps { get; set; }
        public List<AdditionalPrices> AdditionalPrices { get; set; }
        public int companyId { get; set; }
        public DateTime periodStart { get; set; }
        public DateTime periodEnd { get; set; }
        public double remainingDays { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int bundleTime { get; set; }
        public periodType periodType { get; set; }
        public bundleInformation bundleInformation { get; set; }
        public AdditionalIformation AdditionalIformation { get; set; }
        public bundleConsumption bundleConsumption { get; set; }
        public int subscriptionId { get; set; }

    }
    public class periodType
    {
        public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
    public class bundleInformation
    {
        public int NumberOfUser { get; set; }

        public int NumberOfPOS { get; set; }

        public int NumberOfEmployee { get; set; }

        public int NumberOfStore { get; set; }

        public int NumberOfBranchs { get; set; }

        public int NumberOfInvoices { get; set; }

        public int NumberOfSuppliers { get; set; }

        public int NumberOfCustomers { get; set; }

        public int NumberOfItems { get; set; }
    }
    public class AdditionalIformation
    {
        public int NumberOfUser { get; set; }

        public int NumberOfPOS { get; set; }

        public int NumberOfEmployee { get; set; }

        public int NumberOfStore { get; set; }

        public int NumberOfBranchs { get; set; }

        public int NumberOfInvoices { get; set; }

        public int NumberOfSuppliers { get; set; }

        public int NumberOfCustomers { get; set; }

        public int NumberOfItems { get; set; }
        public int NumberOfOfflinePOS { get; set; }
    }
    public class bundleConsumption
    {
        public int NumberOfUser { get; set; }
        //public int remainingNumberOfUser { get; set; }
        public int ConsumptionNumberOfUser { get; set; }

        public int NumberOfPOS { get; set; }
        //public int remainingNumberOfPOS { get; set; }
        public int ConsumptionNumberOfPOS { get; set; }

        public int NumberOfEmployee { get; set; }
        //public int remainingNumberOfEmployee { get; set; }
        public int ConsumptionNumberOfEmployee { get; set; }

        public int NumberOfStore { get; set; }
        //public int remainingNumberOfStore { get; set; }
        public int ConsumptionNumberOfStore { get; set; }

        public int NumberOfBranchs { get; set; }
        //public int remainingNumberOfBranchs { get; set; }
        public int ConsumptionNumberOfBranchs { get; set; }

        public int NumberOfInvoices { get; set; }
        //public int remainingNumberOfInvoices { get; set; }
        public int ConsumptionNumberOfInvoices { get; set; }

        public int NumberOfSuppliers { get; set; }
        //public int remainingNumberOfSuppliers { get; set; }
        public int ConsumptionNumberOfSuppliers { get; set; }

        public int NumberOfCustomers { get; set; }
        //public int remainingNumberOfCustomers { get; set; }
        public int ConsumptionNumberOfCustomers { get; set; }

        public int NumberOfItems { get; set; }
        //public int remainingNumberOfItems { get; set; }
        public int ConsumptionNumberOfItems { get; set; }

        //Offline pos
        public int NumberOfOfflinePOS { get; set; }
        public int ConsumptionNumberOfflinePOS { get; set; }
    }

}
