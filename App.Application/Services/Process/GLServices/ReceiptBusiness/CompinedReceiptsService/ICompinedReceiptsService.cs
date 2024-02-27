using App.Domain.Models.Request.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GLServices.ReceiptBusiness.CompinedReceiptsService
{
    public interface ICompinedReceiptsService
    {
        Task<ResponseResult> AddCompinedReceipt(CompinedRecieptsRequest parameter);
        Task<ResponseResult> UpdateCompinedReceipt(UpdateCompinedRecieptsRequest parameter);
        Task<ResponseResult> GetAllCompinedReciepts(GetRecieptsData parameter);
        Task<ResponseResult> GetCompinedRecieptById(int Id);
        Task<ResponseResult> DeleteCompinedReciept(List<int> Id);
    }
}
