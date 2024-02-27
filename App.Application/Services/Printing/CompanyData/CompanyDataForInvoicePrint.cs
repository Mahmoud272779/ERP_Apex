using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.CompanyData
{


    public class CompanyDataForInvoicePrint : ICompanyDataForInvoicePrint
    {
        private readonly ICompanyDataService _CompanyDataService;
        public CompanyDataForInvoicePrint(ICompanyDataService companyDataService)
        {
            _CompanyDataService = companyDataService;
        }
        public async Task<ResponseResult> GetCompanyData()
        {
            return await _CompanyDataService.GetCompanyData(true);
        }
    }
}
