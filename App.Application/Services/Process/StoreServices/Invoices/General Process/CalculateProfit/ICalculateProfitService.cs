using App.Domain;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.Store;
using App.Domain.Models.Security.Authentication.Request.Store.Invoices;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process

{
    public  interface ICalculateProfitService
    {
        public Task<ResponseResult> GetEditData();
        public Task<ResponseResult> CalculateAllProfit(ProfitRequest parameter);

    }
}
