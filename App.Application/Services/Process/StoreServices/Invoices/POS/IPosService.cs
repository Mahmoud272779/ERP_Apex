using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public interface IPosService
    {
        Task<ResponseResult> getItemUnitsForPOS(List<int> itemId);
    }
}
