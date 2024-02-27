using App.Domain.Models.Security.Authentication.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
   
    public class CategoriesParameter
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public double vatValue { get; set; }
        public string color { get; set; }
        public int status { get; set; }
        public int usedInSales { get; set; }
        public IFormFile image { get; set; }
    
        public string notes { get; set; }
        public DateTime UTime { get; set; }
        public string? printers { get; set; }
        public int categoryType { get; set; }
        public int? kitchenId { get; set; } = null;

    }

    public class UpdateCategoryParameter
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double VatValue { get; set; }
        public string Color { get; set; }
        [Required]
        public int Status { get; set; }
        public int UsedInSales { get; set; }
        public IFormFile Image { get; set; }
        public string Notes { get; set; }
        public bool ChangeImage { get; set; }
        public DateTime UTime { get; set; }
        public string? printers { get; set; }
        public int categoryType { get; set; }
        public int? kitchenId { get; set; } = null;



    }

    public class UpdateActiveCategoriesParameter
    {
        public List<int> Id { get; set; }
        public int Status { get; set; }
    }

    public class CategoriesSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
        public int Id { get; set; }
    }

 
    public class CategoryWithImgDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double VatValue { get; set; }
        public string Color { get; set; }
        public int Status { get; set; }
        public int UsedInSales { get; set; }
        public byte[] Image { get; set; }
        public string ImagePath { get; set; }

        public string ImageName { get; set; }
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public int? kitchenId { get; set; }
        public int categoryType { get; set; }
        public List<PrinterBranchCategoryDto> printers { get; set; } = new List<PrinterBranchCategoryDto>();


    }
    public class PrinterBranchCategoryDto
    {
        public int BranchID { get; set; }
        public string BranchAr { get; set; }
        public string BranchEn { get; set; }
        public int PrinterID { get; set; }
        public string PrinterAr { get; set; }
        public string PrinterEn { get; set; }
        public string PrinterIP { get; set; }


    }


}
