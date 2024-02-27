using App.Domain;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Security.Authentication.Request.Store.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process

{
    public  interface IEditedItemsService
    {
        public Task AddItemInEditedItem(List<editedItemsParameter> editedItems);

    }
}
