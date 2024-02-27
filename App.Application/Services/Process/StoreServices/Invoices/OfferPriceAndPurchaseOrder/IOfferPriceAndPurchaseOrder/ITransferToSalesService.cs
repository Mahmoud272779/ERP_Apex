using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService
{
    public  interface ITransferToSalesService
    {
        Task<ResponseResult> transferTosales(int id);
        Task<ResponseResult> updateStatus(int id);
    }
}
