using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Currancy
{
    public interface ICurrencyBusiness
    {
        Task<ResponseResult> AddCurrency(CurrencyParameter parameter);
        Task<ResponseResult> UpdateCurrency(UpdateCurrencyParameter parameter);
        Task<ResponseResult> UpdateCurrencyStatus(SharedRequestDTOs.UpdateStatus request);
        Task<ResponseResult> GetCurrencyById(int Id);
        Task<ResponseResult> DeleteCurrencyAsync(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllCurrencyData(currencyRequest paramters);
        Task<ResponseResult> GetAllCurrencyDataDropDown();
        Task<ResponseResult> UpdateCurrencyFactor(UpdateCurrencyFactorList parameter);
        Task<int> AddAutomaticCode();
        Task<ResponseResult> GetAllCurrencyHistory(int CurrencyId);
    }
}
