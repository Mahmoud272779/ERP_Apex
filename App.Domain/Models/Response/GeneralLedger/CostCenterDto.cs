using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class CostCenterParentsDto
    {
        public CostCenterParentsDto()
        {
            costCenterChilds = new List<CostCenterChildsDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public List<CostCenterChildsDto> costCenterChilds{ get; set; }
    }
    public class CostCenterChildsDto 
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
    }
    public class GenaricCostCenterDto
    {
        public GenaricCostCenterDto()
        {
            GenaricCostCenterDtos = new List<GenaricCostCenterDto2>();
        }
        public int Id { get; set; }
        public string currencyAR { get; set; }
        public string currencyEn { get; set; }
        public string Code { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double InitialBalance { get; set; }
        public int CC_Nature { get; set; }
        public int Type { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public List<GenaricCostCenterDto2> GenaricCostCenterDtos { get; set; }
    }
    public class GenaricCostCenterDto2
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double InitialBalance { get; set; }
        public int CC_Nature { get; set; }
        public int Type { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
    }
    public class CostCenterDto
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Code { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public int? ParentId { get; set; }
        public bool IsChecked { get; set; } = false;
    }
    public class CostCenterFinancialAccountDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
