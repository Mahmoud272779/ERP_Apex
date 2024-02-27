using System.Threading.Tasks;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public interface IDeletePurchaseService
    {
        Task<ResponseResult> DeletePurchase(SharedRequestDTOs.Delete parameter , int invoiceTypeId);
    }
}
