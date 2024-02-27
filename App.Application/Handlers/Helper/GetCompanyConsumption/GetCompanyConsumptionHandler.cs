using App.Domain.Entities.POS;
using App.Domain.Entities.Setup;
using App.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.GetCompanyConsumption
{
    public class GetCompanyConsumptionHandler : IRequestHandler<GetCompanyConsumptionRequest, List<GetCompanyConsumptionResponse>>
    {
        private readonly IRepositoryQuery<userAccount> _userAccountQuery;
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly IRepositoryQuery<InvEmployees> _InvEmployeesQuery;
        private readonly IRepositoryQuery<InvStpStores> _InvStpStoresQuery;
        private readonly IRepositoryQuery<GLBranch> _GLBranchQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _InvStpItemCardMasterQuery;

        public GetCompanyConsumptionHandler(IRepositoryQuery<userAccount> userAccountQuery, IRepositoryQuery<POSSession> pOSSessionQuery, IRepositoryQuery<InvEmployees> invEmployeesQuery, IRepositoryQuery<InvStpStores> invStpStoresQuery, IRepositoryQuery<GLBranch> gLBranchQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery)
        {
            _userAccountQuery = userAccountQuery;
            _POSSessionQuery = pOSSessionQuery;
            _InvEmployeesQuery = invEmployeesQuery;
            _InvStpStoresQuery = invStpStoresQuery;
            _GLBranchQuery = gLBranchQuery;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _InvPersonsQuery = invPersonsQuery;
            _InvStpItemCardMasterQuery = invStpItemCardMasterQuery;
        }
        public async Task<List<GetCompanyConsumptionResponse>> Handle(GetCompanyConsumptionRequest request, CancellationToken cancellationToken)
        {
            List<GetCompanyConsumptionResponse> list_GetCompanyConsumptionResponse = new List<GetCompanyConsumptionResponse>();
            var CountOfUsers = _userAccountQuery.TableNoTracking.Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse { count = CountOfUsers, Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraUsers });


            var CountOfOpenedPOSSessions = _POSSessionQuery.TableNoTracking.Where(x => x.sessionStatus != (int)POSSessionStatus.closed).Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse { count = CountOfOpenedPOSSessions, Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraPOS });
            
            
            var countOfEmployees = _InvEmployeesQuery.TableNoTracking.Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse 
            {
                count = countOfEmployees, 
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraEmployees 
            });


            var countOfStores = _InvStpStoresQuery.TableNoTracking.Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse
            {
                count = countOfStores,
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraStores
            });

            var countOfBranches = _GLBranchQuery.TableNoTracking.Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse
            {
                count = countOfBranches,
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraBranches
            });

            var countOfInvoices = _InvoiceMasterQuery.TableNoTracking.Where(x => Lists.MainInvoiceForReturn.Contains(x.InvoiceTypeId)).Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse
            {
                count = countOfInvoices,
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraInvoices
            });

            var countOfCustomers = _InvPersonsQuery.TableNoTracking.Where(x => x.IsCustomer).Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse
            {
                count = countOfCustomers,
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraCustomers
            });

            var countOfSuppliers = _InvPersonsQuery.TableNoTracking.Where(x => x.IsSupplier).Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse
            {
                count = countOfSuppliers,
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraSuppliers
            });

            var countOfItems = (double)_InvStpItemCardMasterQuery.TableNoTracking.Count();
            list_GetCompanyConsumptionResponse.Add(new GetCompanyConsumptionResponse
            {
                count = int.Parse(Math.Ceiling(countOfItems / 1000).ToString()),
                Id = (int)Enums.SecurityApplicationAdditionalPriceIndexs.extraItems
            });

            return list_GetCompanyConsumptionResponse;
        }
    }
}
