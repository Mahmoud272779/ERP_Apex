using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class AddItemRequest : IRequest<ResponseResult>
    {
        //public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string NationalBarcode { get; set; }
        [Required]
        public int TypeId { get; set; }
        public bool UsedInSales { get; set; }
        public int? DepositeUnit { get; set; }
        public int? WithdrawUnit { get; set; }
        public int? ReportUnit { get; set; }
        public decimal? VAT { get; set; }
        public bool? ApplyVAT { get; set; }
#nullable enable
        public string? Model { get; set; }
        public int? DefaultStoreId { get; set; }
        public string? Description { get; set; }
        public string? LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        [Required]
        public int GroupId { get; set; }
        [Required]
        public int Status { get; set; }
        public DateTime UTime { get; set; }
        public IFormFile? Image1 { get; set; }
        public virtual ICollection<ItemUnitVM> Units { get; set; } = new List<ItemUnitVM>();
        public virtual ICollection<ItemStoreVM> Stores { get; set; } = new List<ItemStoreVM>();
        public virtual ICollection<ItemPartVM> Parts { get; set; } = new List<ItemPartVM>();
        //    public virtual ICollection<ItemSerialVM> ItemSerials { get; set; }
    }
}
