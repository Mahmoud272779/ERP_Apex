using App.Domain.Models.Request.GeneralLedger;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GLServices.ReceiptBusiness.ReceiptsPaid
{
    public interface ICollectionReceipts
    {
        Task<ResponseResult> AddCollectionReceipts(CollectionReceiptsRequest request);

        Task<ResponseResult> DeleteCollectionReceipts(List<int?> Ids);
    }
}
