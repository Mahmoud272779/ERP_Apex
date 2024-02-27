using App.Domain.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvCompanyData 
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
        public byte [] imageFile { get; set; }

    }
}
