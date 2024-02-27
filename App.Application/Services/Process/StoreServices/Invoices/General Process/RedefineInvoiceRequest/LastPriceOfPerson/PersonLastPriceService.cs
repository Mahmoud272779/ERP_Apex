using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public class PersonLastPriceService : IPersonLastPriceService
    {
        private readonly IRepositoryCommand<InvPersonLastPrice> personLastPriceCommand;
        private readonly IRepositoryQuery<InvPersonLastPrice> personLastPriceQuery;

        public PersonLastPriceService(IRepositoryCommand<InvPersonLastPrice> personLastPriceCommand,
                                        IRepositoryQuery<InvPersonLastPrice> personLastPriceQuery)
        {
            this.personLastPriceCommand = personLastPriceCommand;
            this.personLastPriceQuery = personLastPriceQuery;
        }

        public async Task setLastPrice(PersonLastPriceRequest request)
        {
            var itemsDataRequest = request.invoiceDetails.Where(a => (a.parentItemId == 0 || a.parentItemId == null)
                          && a.ItemTypeId != (int)ItemTypes.Note).Select(a => new { a.ItemId, a.UnitId, a.Price });

            var items = itemsDataRequest.Select(a => a.ItemId);
            var units = itemsDataRequest.Select(a => a.UnitId);

            var itemsDataDb = personLastPriceQuery.FindAll(a =>a.invoiceTypeId==request.invoiceTypeId && a.personId == request.personId &&
                                                       items.Contains( a.itemId ) && units.Contains( a.unitId));

            var newItems = new List<InvPersonLastPrice>();
            foreach(var item in itemsDataRequest)
            {
                var itemExist = itemsDataDb.Where(a => a.itemId == item.ItemId && a.unitId == item.UnitId);
                if(itemExist.Any())
                {
                    itemsDataDb.Where(a => a.itemId == item.ItemId && a.unitId == item.UnitId).Select(a => a.price = item.Price).ToList();
                }
                else
                {
                    newItems.Add(new InvPersonLastPrice() { personId = request.personId ,itemId = item.ItemId, unitId = item.UnitId.Value,
                        invoiceTypeId=request.invoiceTypeId,price = item.Price }); ;

                }
               
            }
            await personLastPriceCommand.UpdateAsyn(itemsDataDb);
              personLastPriceCommand.AddRangeAsync(newItems);
            personLastPriceCommand.SaveAsync();

        }
    }
}
