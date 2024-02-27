using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class AddPermissionDto
    {
        public int InvoiceId { get; set; }
        public AddPermissionDto()
        {
            AddPermissionDetails = new List<AddPermissionDetailsDto>();
        }
        public string BookIndex { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Notes { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public List<AddPermissionDetailsDto> AddPermissionDetails { get; set; }
    }
    public class AddPermissionDetailsDto
    {
        public AddPermissionDetailsDto()
        {
            addPermissionSerialDtos = new List<AddPermissionSerialDto>();
        }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public DateTime? ExpireDate { get; set; }
        public List<AddPermissionSerialDto> addPermissionSerialDtos { get; set; }
    }
    public class AddPermissionSerialDto
    {
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string SerialNumber { get; set; }
    }
    public class AddPermissionAllDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceType { get; set; }
        public string BookIndex { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Notes { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public double TotalPrice { get; set; }
        public bool  IsDeleted { get; set; }
        public bool CanDeleted { get; set; }
    }
    public class AddPermissionSearch : GeneralPageSizeParameter
    {
       
        public string SearchCriteria { get; set; }
        public Search Search { get; set; }
    }
    public class Search
    {
        public int[] StoreId { get; set; }
        public int[] ItemId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
    public class ItemTypeDto
    {
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public string ItemTypeName { get; set; }
    }
}
