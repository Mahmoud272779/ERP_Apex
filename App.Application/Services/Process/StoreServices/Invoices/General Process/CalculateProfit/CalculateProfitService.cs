using App.Application.Services.Process;
using App.Domain;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.Store;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public class CalculateProfitService : ICalculateProfitService
    {

        readonly private IRepositoryQuery<EditedItems> editedItemsQuery;
        readonly private IRepositoryQuery<InvoiceDetails> InvoiceDetailQuery;
        readonly private IRepositoryCommand<InvoiceDetails> InvoiceDetailCommand;
        readonly private IRepositoryCommand<EditedItems> editedItemsCommand;
      public CalculateProfitService(
          IRepositoryCommand<EditedItems> editedItemsCommand,
          IRepositoryQuery<InvoiceDetails> invoiceDetailQuery,
        IRepositoryCommand<InvoiceDetails> invoiceDetailCommand,
        IRepositoryQuery<EditedItems> editedItemsQuery)
        {
            this.editedItemsCommand = editedItemsCommand;
            this.editedItemsQuery = editedItemsQuery;
            InvoiceDetailCommand = invoiceDetailCommand;
            InvoiceDetailQuery = invoiceDetailQuery;
        }

        public async Task<ResponseResult> CalculateAllProfit(ProfitRequest parameter)
        {
            int? sizeId= parameter.SizeId ==0? null:parameter.SizeId ;
            try
            {
                var invoice =await InvoiceDetailQuery.TableNoTracking.Where(h => h.InvoiceId == parameter.InvoiceId
                           && h.ItemId == parameter.ItemId
                           && h.SizeId == sizeId).FirstOrDefaultAsync();
                //var Invoices =  InvoiceDetailCommand.Table.Where(
                //   h => h.InvoiceId == parameter.InvoiceId
                //           && h.ItemId == parameter.ItemId
                //           && h.SizeId == sizeId).ToList();


                if (invoice==null)
                {
                    return new ResponseResult() { Result = Result.Failed };
                }
                var inv = invoice;
                inv.Cost = parameter.Cost;
                inv.AvgPrice = parameter.AVG;
                await InvoiceDetailCommand.UpdateAsyn(inv);
                return new ResponseResult() { Result = Result.Success };
            }
            catch (System.Exception e)
            {
                return new ResponseResult() { Result = Result.Failed };

            }
        }

        public async Task<ResponseResult> GetEditData()
        {
            var items = await editedItemsQuery.TableNoTracking.ToListAsync();
            return new ResponseResult() { Data = items ,Result= Result.Success};
        }
    }
}
