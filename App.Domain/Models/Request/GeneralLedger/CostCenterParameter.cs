using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class CostCenterParameter
    {
        public string Code { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double InitialBalance { get; set; }
        public int CC_Nature { get; set; }
        public int Type { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
    }
    public class UpdateCostCenterParameter
    {
        public int Id { get; set; }
       // public string Code { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double? InitialBalance { get; set; }
        public int Type { get; set; }
        public int CC_Nature { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
    }
    
    }
