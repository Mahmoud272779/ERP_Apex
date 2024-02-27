using App.Application.Handlers.MainData.Payment_Methods.GetPaymentMethodByDate;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Payment_methods
{
    public interface IPaymentMethodsService
    {
        Task<ResponseResult> AddPaymentMethod(PaymentMethodsRequest parameter);
        Task<ResponseResult> GetListOfPaymentMethods(PaymentMethodsSearch parameters);
        Task<ResponseResult> UpdatePaymentMethods(UpdatePaymentMethodsRequest parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> DeletePaymentMethods(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetPaymentMethodHistory(int Code);
        Task<ResponseResult> GetPaymentMethodsDropdown(bool isReceipts);
        Task<ResponseResult> GetPaymentMethodsDropdowninvoic(List<int> paymentIds,int? invoiceTypeId=0);
        Task<ResponseResult> GetPaymentMethodsByDate(GetPaymentMethodsByDateRequest parameter);

    }
}
