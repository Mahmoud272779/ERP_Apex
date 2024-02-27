using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.RPT_Services
{
    public interface iStoreReportsService
    {
        public Task<ResponseResult> itemsSales(itemSalesReportDTO parm);
    }
}
