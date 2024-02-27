using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
   public class UpdateCompanyDataRequest
    {
        //public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string fieldAr { get; set; }
        public string fieldEn { get; set; }
        public string commercialRegister { get; set; }
        public string taxNumber { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string email { get; set; }
        public string notes { get; set; }
        public string latinAddress { get; set; }
        public string arabicAddress { get; set; }
        public IFormFile image { get; set; }
        public bool changeImage { get; set; }
    }

    public class CompanyDataDto
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string FieldAr { get; set; }
        public string FieldEn { get; set; }
        public string CommercialRegister { get; set; }
        public string TaxNumber { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string LatinAddress { get; set; }
        public string ArabicAddress { get; set; }
        public string Image { get; set; }
        public IFormFile imageFile { get; set; }
        public bool ChangeImage { get; set; }

    }
}
