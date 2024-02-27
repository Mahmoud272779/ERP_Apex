using App.Domain.Models.Response.Store.Reports.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.Store.Store.ItemsBalanceInStores
{
    public class ItemsBalanceInStoresRequest : GeneralPageSizeParameter, IRequest<itemBalanceInStoreResponse>
    {
        [Required]
        public int storeId { get; set; }
        public int? itemId { get; set; }
        public int? catId { get; set; }
        public ItemTypes? itemTypes { get; set; }
        public bool isPrint { get; set; }
    }
    public class _ItemsBalanceInStoresRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        [Required]
        public int storeId { get; set; }
        public int? itemId { get; set; }
        public int? catId { get; set; }
        public ItemTypes? itemTypes { get; set; }
        public bool isPrint { get; set; }
    }
}
