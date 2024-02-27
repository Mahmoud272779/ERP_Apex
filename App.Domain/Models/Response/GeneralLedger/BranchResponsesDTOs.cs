using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.GeneralLedger
{
    public class BranchResponsesDTOs
    {
        public class GetAll
        {
            public int Id { get; set; }
            public int Code { get; set; }
            public string LatinName { get; set; } = "";
            public string ArabicName { get; set; } = "";
            public string AddressEn { get; set; } = "";
            public string AddressAr { get; set; } = "";
            public string Fax { get; set; } = "";
            public string Phone { get; set; } = "";
            public int Status { get; set; }
            public string Notes { get; set; } = "";
            public object ManagerId { get; set; }
            [JsonIgnore]
            public int? Manager { get; set; }
            //public string ManagerNameAr { get; set; }
            //public string ManagerNameEn { get; set; }
            //public int ManagerStatus { get; set; }
            public string ManagerPhone { get; set; } = "";
            public string BrowserName { get; set; } = "";
            public bool CanDelete { get; set; }
        }
       
    }

}
