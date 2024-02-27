using App.Domain.Models.Request.print;
using AutoMapper;
using MediatR;
using System.Linq;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class GetListOfPersonsHandler : IRequestHandler<GetListOfPersonsRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterQuery;
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _financialAccountQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<GLBranch> branchesRepositoryQuery;
        private readonly IRepositoryQuery<InvSalesMan> _salesmanQuery;
        private readonly IMapper _mapper;
        public GetListOfPersonsHandler(iUserInformation iUserInformation, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryQuery<InvPersons> personQuery, IRepositoryQuery<GLFinancialAccount> financialAccountQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<GLBranch> branchesRepositoryQuery, IRepositoryQuery<InvSalesMan> salesmanQuery, IMapper mapper, IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery)
        {
            _iUserInformation = iUserInformation;
            _glRecieptsQuery = glRecieptsQuery;
            PersonQuery = personQuery;
            _financialAccountQuery = financialAccountQuery;
            _invoiceMasterQuery = invoiceMasterQuery;
            this.branchesRepositoryQuery = branchesRepositoryQuery;
            _salesmanQuery = salesmanQuery;
            _mapper = mapper;
            _OfferPriceMasterQuery = offerPriceMasterQuery;
        }
        public async Task<ResponseResult> Handle(GetListOfPersonsRequest parameters, CancellationToken cancellationToken)
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            var reciept = _glRecieptsQuery.TableNoTracking;
            var salesmanIds = parameters.SalesManId !=null? parameters.SalesManId.Split(',').Select(x => int.Parse(x)).ToArray():null;
            var person = Enumerable.Empty<InvPersons>().AsQueryable();
            if (parameters.isSearchData)
            {
                person = PersonQuery.TableNoTracking
               .Include(x => x.PersonBranch)
                .Include(x => x.SalesMan)
                .Include(x => x.FundsCustomerSuppliers)
               .Where(x => (bool)parameters.IsSupplier ? x.IsSupplier == parameters.IsSupplier : x.IsCustomer == true)
               .Where(x => (salesmanIds == null|| salesmanIds.Count() == 0 ) ? true: salesmanIds.Contains((int)x.SalesManId))
               .Where(x => x.PersonBranch.Select(d => d.BranchId).ToArray().Any(c => userinfo.employeeBranches.Contains(c)) || (x.Id == 2 || x.Id == 1)) ;
            }
            else
            {
                string[] Ids = parameters.ids.Split(",");
                foreach (var id in Ids)
                {
                    var item = PersonQuery.TableNoTracking.Where(p => p.Id == Convert.ToInt32(id))
                    .Include(x => x.PersonBranch)
                    .Include(x => x.SalesMan)
                    .Include(x => x.FundsCustomerSuppliers)
               .Where(x => (bool)parameters.IsSupplier ? x.IsSupplier == parameters.IsSupplier : x.IsCustomer == true)
               .Where(x => (salesmanIds == null || salesmanIds.Count() == 0) ? true : salesmanIds.Contains((int)x.SalesManId))
               .Where(x => x.PersonBranch.Select(d => d.BranchId).ToArray().Any(c => userinfo.employeeBranches.Contains(c)) || (x.Id == 2 || x.Id == 1)).FirstOrDefault();
                    person = person.Append(item);
                }

            }

            if (!userinfo.otherSettings.showCustomersOfOtherUsers)
            {
                person = person.Where(a => a.InvEmployeesId == userinfo.employeeId || (a.Id == 2 || a.Id == 1));
            }
            int totalCount = person.Count();
            var financialAccount = _financialAccountQuery.TableNoTracking;
            var invoices = _invoiceMasterQuery.TableNoTracking;
            var ccc = person.Count();

            if (!string.IsNullOrEmpty(parameters.Name))
                person = person
                .Where(a =>
                (a.Code.ToString().Contains(parameters.Name)
                || (a.CodeT + "-" + a.Code.ToString()).Contains(parameters.Name)
                
                || string.IsNullOrEmpty(parameters.Name)
                || parameters.Name.Contains( a.ArabicName)
                || a.Phone == parameters.Name
                || a.LatinName.Contains(parameters.Name))); 

            //if (parameters.TypeArr != null)
            //    person = person.Where(x => parameters.TypeArr.Contains(x.Type));

            if (parameters.Status > 0)
                person = person.Where(a => a.Status == parameters.Status);

            var count = person.Count();
            int nameOutput;
            var tryParseName = int.TryParse(parameters.Name, out nameOutput);

            if (string.IsNullOrEmpty(parameters.Name))
                person = person.OrderByDescending(x => x.Id);
            else
            {
                if (!tryParseName)
                {
                    person = person
                        .OrderByDescending(a => a.ArabicName == parameters.Name)
                        .ThenByDescending(a => a.ArabicName.StartsWith(parameters.Name));

                }
                else
                {
                    person = person
                        .OrderBy(x => x.Code)
                        .ThenBy(x => x.ArabicName)
                        .ThenBy(x => x.LatinName);
                }
            }
                
            person = parameters.isPrint ? person : person.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);
            if (parameters.PageNumber == 0 && parameters.PageSize == 0 && parameters.isPrint == false)
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            }

            var list = new List<InvPersons>();
            foreach (var item in person)
            {
                item.CanDelete = await PersonHelper.canDelete(person.ToList(), item.Id, (bool)parameters.IsSupplier, invoices, reciept, _OfferPriceMasterQuery);
                list.Add(item);
            }
            var branches = branchesRepositoryQuery.TableNoTracking;
            var allSalesMan = _salesmanQuery.TableNoTracking;
            var res = list.Select(x => new PersonsReponseDto
            {
                id = x.Id,
                code = x.CodeT + "-" + x.Code,
                arabicName = x.ArabicName,
                latinName = x.LatinName,
                type = x.Type,
                status = x.Status,
                SalesManId = allSalesMan.Where(c => c.Id == x.SalesManId).Select(c => new { id = c.Id, c.ArabicName, c.LatinName }).FirstOrDefault(),
                responsibleAr = x.ResponsibleAr,
                responsibleEn = x.ResponsibleEn,
                phone = x.Phone,
                fax = x.Fax,
                email = x.Email,
                taxNumber = x.TaxNumber,
                addressAr = x.AddressAr,
                addressEn = x.AddressEn,
                addToAnotherList = x.AddToAnotherList,
                isSupplier = x.IsSupplier,
                branches = x.PersonBranch.Select(d => d.BranchId).ToArray(),
                branchNameAr = string.Join(",", branches.Where(d => x.PersonBranch.Select(d => d.BranchId).ToArray().Contains(d.Id)).Select(d => d.ArabicName).ToArray()),
                branchNameEn = string.Join(",", branches.Where(d => x.PersonBranch.Select(d => d.BranchId).ToArray().Contains(d.Id)).Select(d => d.LatinName).ToArray()),
                CreditLimit= x.CreditLimit,
                CreditPeriod = x.CreditPeriod,
                DiscountRatio = x.DiscountRatio,
                SalesPriceId = x.SalesPriceId,
                LessSalesPriceId = x.LessSalesPriceId,
                CanDelete = x.CanDelete,
                BuildingNumber = x.BuildingNumber,
                StreetName = x.StreetName,
                Neighborhood = x.Neighborhood,
                City = x.City,
                Country = x.Country,
                PostalNumber = x.PostalNumber,
                FinancialAccountId = financialAccount.Where(c => c.Id == x.FinancialAccountId).Select(c => new { c.Id, c.ArabicName, c.LatinName }).FirstOrDefault(),
                isUsedInInvoices = PersonHelper.isUsedInInvoices(_invoiceMasterQuery, (bool)parameters.IsSupplier, x.Id)
            }).ToList();
            if (parameters.isPrint)
            {
                var data = _mapper.Map<IEnumerable<PersonsReponseDto>>(list);
                return new ResponseResult() { TotalCount = totalCount, Data = data, DataCount = count, Id = null, Result = res.Any() ? Result.Success : Result.NotFound };

            }

            return new ResponseResult() { TotalCount = totalCount, Data = res, DataCount = count, Id = null, Result = res.Any() ? Result.Success : Result.NotFound };
        }
    }
}
