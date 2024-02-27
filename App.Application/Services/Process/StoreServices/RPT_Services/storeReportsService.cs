using App.Domain.Entities.Process;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.RPT_Services
{
    public class storeReportsService : iStoreReportsService
    {
        private readonly IRepositoryQuery<InvoiceDetails> _InvoiceDetailsQuery;

        public storeReportsService(IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery)
        {
            _InvoiceDetailsQuery = InvoiceDetailsQuery;
        }
        public async Task<ResponseResult> itemsSales(itemSalesReportDTO parm)
        {
            var invoices = _InvoiceDetailsQuery.TableNoTracking.Where(x=> x.ItemId == parm.itemId);
            return new ResponseResult()
            {

            };
        }
    }
}
