using App.Application.Handlers.Invoices.OfferPrice.AddOfferPrice;
using App.Infrastructure.Migrations;
using MediatR;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.PriceLists
{
    public class PriceListsHandler : IRequestHandler<salePriceListsRequest, int>
    {
        private readonly IRepositoryQuery<GLBranch> branchQuery;
        private readonly IRepositoryQuery<InvSalesMan> salesManQuery;
        private readonly IRepositoryQuery<InvPersons> personQuery;
        private readonly IRepositoryQuery<InvEmployees> employeeQuery;
        public PriceListsHandler(IRepositoryQuery<GLBranch> branchQuery, IRepositoryQuery<InvSalesMan> salesManQuery, IRepositoryQuery<InvPersons> personQuery, IRepositoryQuery<InvEmployees> employeeQuery)
        {
            this.branchQuery = branchQuery;
            this.salesManQuery = salesManQuery;
            this.personQuery = personQuery;
            this.employeeQuery = employeeQuery;
        }
        public async Task<int> Handle(salePriceListsRequest request, CancellationToken cancellationToken)
        {
            int SalesPriceId = (int)SalePricesList.SalePrice1;
            if (request.setting.PriceListType == (int)PriceListsType.BranchPrice)
                SalesPriceId = branchQuery.TableNoTracking.Where(a => a.Id == request.branchId)
                            .Select(a => a.SalesPriceId.Value).FirstOrDefault();
            else if (request.setting.PriceListType == (int)PriceListsType.SalesManPrice)
            {
                if (request.invoiceTypeId != (int)DocumentType.POS)
                    SalesPriceId = salesManQuery.TableNoTracking.Where(a => a.Id == request.salesManId)
                            .Select(a => a.SalesPriceId.Value).FirstOrDefault();
                else
                    SalesPriceId = (int)SalePricesList.SalePrice1;
            }
            else if (request.setting.PriceListType == (int)PriceListsType.PersonPrice)
                SalesPriceId = personQuery.TableNoTracking.Where(a => a.Id == request.personId)
                            .Select(a => a.SalesPriceId.Value).FirstOrDefault();
            else if (request.setting.PriceListType == (int)PriceListsType.employeePrice)
                SalesPriceId = employeeQuery.TableNoTracking.Where(a => a.Id == request.employeeId)
                            .Select(a => a.SalesPriceId.Value).FirstOrDefault();
            else  // فى حالة تفعيل  قوائم الاسعار 
            {
                if(request.oldSalePriceId!=null && request.oldSalePriceId>0)  // فى حال التعديل 
                     SalesPriceId = request.oldSalePriceId.Value;
                else
                    SalesPriceId = request.PriceListId.Value;
            }
            return SalesPriceId;
        }

    }
}
