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
    public class UpdateItemRequest : IRequest<ResponseResult>
    {
#nullable enable
        [Required]
        public int Id { get; set; }
        [Required]
        public string? ItemCode { get; set; }
        public string? NationalBarcode { get; set; }
        [Required]
        public int TypeId { get; set; }
        public bool? UsedInSales { get; set; }
        public int? DepositeUnit { get; set; }
        public int? WithdrawUnit { get; set; }
        public int? ReportUnit { get; set; }
        public decimal? VAT { get; set; }
        public bool? ApplyVAT { get; set; }
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
        public IFormFile? Images { get; set; }
        public bool? ChangeImage { get; set; }
        public DateTime UTime { get; set; }
        public virtual List<ItemUnitVM> Units { get; set; } = new List<ItemUnitVM>();
        public virtual ICollection<ItemStoreVM> Stores { get; set; } = new List<ItemStoreVM>();
        public virtual ICollection<ItemPartVM> Parts { get; set; } = new List<ItemPartVM>();
        public virtual ICollection<ItemSerialVM> ItemSerials { get; set; } = new List<ItemSerialVM>();
    }
}
