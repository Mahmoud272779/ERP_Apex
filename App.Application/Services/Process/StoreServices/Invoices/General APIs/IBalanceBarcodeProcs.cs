using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.General_APIs
{
    public  interface IBalanceBarcodeProcs
    {
        public BalanceBarcodeResult getItem(BalanceBarcodeInput input);
        public resultQuantity GetItemCost(double price, double num, string BarcodeType);
    }
}
