using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class ItemCardFilterRequest : PaginationVM, IRequest<ResponseResult>
    {
       

        public ItemCardFilterRequest(int PageNumber, int PageSize, int Status, List<int> Categories, List<int> ItemTypes, List<int> Stores)//, string CodeOrName)
        {
            this.PageNumber = PageNumber;
            this.PageSize = PageSize;
            this.Status = Status;
            this.Categories = Categories;
            this.ItemTypes = ItemTypes;
            this.Stores = Stores;
            //this.CodeOrName = CodeOrName;
        }
        public int? Status { get; set; }
        public List<int> Categories { get; set; }
        public List<int> ItemTypes { get; set; }
        public List<int> Stores { get; set; }
        //public string CodeOrName { get; set; }
    }
}
