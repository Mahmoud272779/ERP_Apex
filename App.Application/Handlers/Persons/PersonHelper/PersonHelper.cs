using App.Domain.Models.Security.Authentication.Response.Store;
using System.Linq;

namespace App.Application.Handlers.Persons
{
    public static class PersonHelper 
    {
        public static async Task<bool> canDelete(List<InvPersons> listOfPersons, int id, bool isSupplier, IQueryable<InvoiceMaster> invoiceMasters, IQueryable<GlReciepts> glReciepts, IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterQuery)
        {
            if (id == 1 || id == 2)
                return false;
            var invoices = isSupplier ? invoiceMasters.Where(x => x.PersonId == id && x.InvoiceTypeId == 5).Any() : invoiceMasters.Where(x => x.PersonId == id && x.InvoiceTypeId == 8).Any();

            if (glReciepts.Where(x => x.PersonId == id).Any() || invoices)
                return false;
            var FundsCustomerSuppliers_Debit = listOfPersons.Where(x => x.Id == id).FirstOrDefault()?.FundsCustomerSuppliers;
            var FundsCustomerSuppliers_Credit = listOfPersons.Where(x => x.Id == id).FirstOrDefault().FundsCustomerSuppliers;
            if (FundsCustomerSuppliers_Credit == null ? false :listOfPersons.Where(x => x.Id == id).FirstOrDefault().FundsCustomerSuppliers.Credit != 0 && FundsCustomerSuppliers_Debit == null ? false : listOfPersons.Where(x => x.Id == id).FirstOrDefault()?.FundsCustomerSuppliers.Debit !=0 )
                return false;
            if (_OfferPriceMasterQuery.TableNoTracking.Where(x => id== x.PersonId).Any())
                return false;
            return true;
        }
        public static bool isUsedInInvoices(IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery,bool isSupplier, int personId)
        {
            var invoices = _invoiceMasterQuery.TableNoTracking.Where(x => (!isSupplier ? x.InvoiceTypeId == 5 : x.InvoiceTypeId == 8) && x.PersonId == personId);
            if (invoices.Any())
                return true;
            return false;
        }

        public static List<SupplierResponse> GetSuppliersResponse(IRepositoryQuery<GLBranch> branchesRepositoryQuery, List<InvPersons> result)
        {
            List<SupplierResponse> table = new List<SupplierResponse>();
            Mapping.Mapper.Map(result, table);
            foreach (var item in table)
            {
                var Branches2 = branchesRepositoryQuery.GetAll(e => item.branches.Contains(e.Id))
                                    .Select(e => new { ArabicName = e.ArabicName, LatinName = e.LatinName }).ToList();
                item.BranchNameAr = string.Join(',', Branches2.Select(e => e.ArabicName).ToArray());
                item.BranchNameEn = string.Join(',', Branches2.Select(e => e.LatinName).ToArray());
            }
            return table;
        }
        public static List<CustomerResponse> GetCustomerResponse(IRepositoryQuery<GLBranch> branchesRepositoryQuery, List<InvPersons> result)
        {
            List<CustomerResponse> table = new List<CustomerResponse>();
            Mapping.Mapper.Map(result, table);
            foreach (var item in table)
            {
                var Branches2 = branchesRepositoryQuery.GetAll(e => item.branches.Contains(e.Id))
                                    .Select(e => new { ArabicName = e.ArabicName, LatinName = e.LatinName }).ToList();
                item.BranchNameAr = string.Join(',', Branches2.Select(e => e.ArabicName).ToArray());
                item.BranchNameEn = string.Join(',', Branches2.Select(e => e.LatinName).ToArray());
            }
            return table;
        }
    }
}
