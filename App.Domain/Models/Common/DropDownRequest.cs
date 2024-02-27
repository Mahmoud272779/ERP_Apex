using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Common
{
    public class DropDownRequest : GeneralPageSizeParameter
    {
        public string? SearchCriteria { get; set; }
        public bool? isSearchByCode { get; set; }
        public int? itemType { get; set; }
        public int? invoiceTypeId { get; set; }
        public int costCenterId { get; set; }


    }
    public class DropDownRequestForGL : DropDownRequest
    {
        public bool? isMain { get; set; }
    }
    public class DropDownRequestForPerson : DropDownRequest
    {
        public int? personId { get; set; }
        public string? Code { get; set; }
        public bool isHaveSalesman { get; set; } = false;
        public bool IsSupplier  { get; set; }
        public bool isReport { get; set; } = false;
    }
}
