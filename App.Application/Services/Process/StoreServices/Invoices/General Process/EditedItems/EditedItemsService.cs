using App.Application.Services.Process;
using App.Domain;
using App.Infrastructure.Interfaces.Repository;
using System.Collections.Generic;
using System.Linq;

namespace App.Application
{
    public class EditedItemsService : IEditedItemsService
    {

        readonly private IRepositoryQuery<EditedItems> editedItemsQuery;
        readonly private IRepositoryCommand<EditedItems> editedItemsCommand;
      public EditedItemsService(IRepositoryCommand<EditedItems> editedItemsCommand,
          IRepositoryQuery<EditedItems> editedItemsQuery)
        {
            this.editedItemsCommand = editedItemsCommand;
            this.editedItemsQuery = editedItemsQuery;
        }

        public async Task AddItemInEditedItem(List<editedItemsParameter> _editedItems)
        {
            

            var editedItems = _editedItems.GroupBy(c => new { c.itemId, c.sizeId, c.branchId }).Select(c => c.FirstOrDefault());
            var itemsWillAdd = new List<EditedItems>();
            var itemsWillDeleteForEdit = new List<EditedItems>();
            var branches = editedItems.Select(a => a.branchId);
            var editedItem = editedItemsQuery.TableNoTracking.Where(a => branches.Contains(a.BranchID) );
          
                foreach (var item in editedItems)
                {
                    var editedItem_ = editedItem.Where(a => a.itemId == item.itemId && a.sizeId == item.sizeId);
                    if (editedItem_.Count() == 0) // add item in editedItem
                    {
                        var itemWillAdd = new EditedItems() { itemId = item.itemId, serialize = item.serialize, sizeId = 0, type = item.itemTypeId, BranchID = item.branchId };
                        itemsWillAdd.Add(itemWillAdd);
                    }
                    else if (editedItem_.First().serialize > item.serialize) // edit the item in editedItem
                    {
                        var itemWillEdit = editedItem_.First();
                        itemsWillDeleteForEdit.Add(itemWillEdit);
                        itemWillEdit.serialize = item.serialize;
                        itemWillEdit.BranchID = item.branchId;
                        itemsWillAdd.Add(itemWillEdit);
                        // await  editedItemsCommand.UpdateAsyn(itemWillEdit);
                    }

                }
            
         
            if(itemsWillDeleteForEdit.Count() > 0)
            {
                editedItemsCommand.RemoveRange(itemsWillDeleteForEdit);
                 await editedItemsCommand.SaveChanges();
            }

            if (itemsWillAdd.Count() > 0)
            {
                await editedItemsCommand.AddAsync(itemsWillAdd);

          //   await   editedItemsCommand.SaveChanges();
            }

        }
    }
}
