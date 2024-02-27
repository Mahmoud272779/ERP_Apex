using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public class UpdateItemsPrices: IUpdateItemsPrices
    {
        private readonly IRepositoryCommand<InvStpItemCardUnit> ItemUnitsCommand;
        private readonly IRepositoryQuery<InvStpItemCardUnit> ItemUnitsQuery;


        public UpdateItemsPrices(IRepositoryCommand<InvStpItemCardUnit> itemUnitsCommand, 
            IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            ItemUnitsCommand = itemUnitsCommand;
            ItemUnitsQuery = itemUnitsQuery;
        }

        public async Task setItemsPrices(List<InvoiceDetails> invoiceDetails)
        {
            var itemsDataRequest = invoiceDetails.Where(a => (a.parentItemId == 0 || a.parentItemId == null)
                          && a.ItemTypeId != (int)ItemTypes.Note).Select(a => new { a.ItemId, a.UnitId, a.Price });

            var items = itemsDataRequest.Select(a => a.ItemId);
            var units = itemsDataRequest.Select(a => a.UnitId);

            var itemsDataDb = ItemUnitsQuery.TableNoTracking.Where(a=> items.Contains(a.ItemId) && units.Contains(a.UnitId)).ToList() ;

             foreach (var item in itemsDataRequest)
            {
                var itemExist = itemsDataDb.Where(a => a.ItemId == item.ItemId && a.UnitId == item.UnitId);
                if (itemExist.Any())
                {
                    itemsDataDb.Where(a => a.ItemId == item.ItemId && a.UnitId == item.UnitId).Select(a => a.PurchasePrice = item.Price).ToList();
                }
                

            }
            await ItemUnitsCommand.UpdateAsyn(itemsDataDb);
             ItemUnitsCommand.SaveAsync();

        }
    }
}
