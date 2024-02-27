using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class GetAllPurchasesService : BaseClass, IGetAllPurchasesService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;
        public GetAllPurchasesService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
            IGetAllInvoicesService _GetAllInvoicesService, iUserInformation Userinformation,
            IHttpContextAccessor _httpContext, IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            GetAllInvoicesService = _GetAllInvoicesService;
            httpContext = _httpContext;
            this.Userinformation = Userinformation;
            this.itemMasterQuery = itemMasterQuery;
            InvoiceDetailsRepositoryQuery = invoiceDetailsRepositoryQuery;
        }
        public async Task<ResponseResult> GetAllPurchase(InvoiceSearchPagination parameter,int invoiceTypeId)
        {
            var searchCretiera = parameter.Searches.SearchCriteria;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var treeData = InvoiceMasterRepositoryQuery.TableNoTracking
                                                       .Include(x => x.store)
                                                       .Include(x => x.Person)
                                                       .Where(x => x.BranchId == userInfo.CurrentbranchId);
         
            //bool expensesFlag = false;
            //bool expensesReturnFlag = false;
            //if(Lists.ExpensesInvoicesList.Contains( parameter.InvoiceTypeId))
            //{
            //    if (parameter.InvoiceTypeId == (int)expensesInvoiceType.Expenses)
            //    {
            //        expensesFlag = true;
            //        parameter.Searches.InvoiceTypeId.Append((int)DocumentType.Purchase);
            //    }
            //    if (parameter.InvoiceTypeId == (int)expensesInvoiceType.ExpensesReturn)
            //    {
            //        expensesReturnFlag = true;
            //        parameter.Searches.InvoiceTypeId.Append((int)DocumentType.ReturnPurchase);

            //    }

            //}
            if(Lists.purchasesInvoicesList.Contains(invoiceTypeId) || Lists.ExpensesInvoicesList.Contains(invoiceTypeId))
            {
                treeData = treeData.Where(q =>
                           (parameter.InvoiceTypeId == (int)DocumentType.Purchase ?
                           ((q.InvoiceTypeId == (int)DocumentType.Purchase && q.IsDeleted == false )||
                               
                           q.InvoiceTypeId == (int)DocumentType.DeletePurchase ):
                          ( q.InvoiceTypeId == (int)DocumentType.ReturnPurchase )));
            }
            else if (Lists.purchasesWithoutVatInvoicesList.Contains(invoiceTypeId))
            {
                treeData = treeData.Where(q =>
                           (parameter.InvoiceTypeId == (int)DocumentType.wov_purchase ?
                           ((q.InvoiceTypeId == (int)DocumentType.wov_purchase && q.IsDeleted == false) ||
                           q.InvoiceTypeId == (int)DocumentType.DeleteWov_purchase) :
                           q.InvoiceTypeId == (int)DocumentType.ReturnWov_purchase));
            }
           if(!userInfo.otherSettings.purchasesShowOtherPersonsInv)
            {
                treeData = treeData.Where(a => a.EmployeeId == userInfo.employeeId);
            }
            var totalCount = treeData.Count();

            if (!string.IsNullOrEmpty(searchCretiera))
            {
                
                treeData = treeData.Where(q =>

                          q.Code.ToString().Contains(searchCretiera) || q.InvoiceType == searchCretiera ||
                          q.Person.ArabicName.ToLower().Contains(searchCretiera) ||
                          q.Person.LatinName.ToLower().Contains(searchCretiera) ||
                          q.Person.Phone==searchCretiera ||
                           //q.Person.Phone.Contains(searchCretiera) ||
                          q.BookIndex== searchCretiera|| (q.ParentInvoiceCode != null ? q.ParentInvoiceCode == searchCretiera : true)
                          || (q.ParentInvoiceCode != null ? q.ParentInvoiceCode.Substring(2) == searchCretiera : true)

                );
            }


           if (string.IsNullOrEmpty(searchCretiera))
            {
                treeData = treeData.OrderByDescending(q => q.Code);
            }
            else
            {
                treeData = treeData.OrderBy(q => q.Code);

                //treeData = treeData.OrderByDescending(a => a.Person.ArabicName == searchCretiera
                //         || a.Person.LatinName == searchCretiera ||  a.Code.ToString() == searchCretiera 
                //         || a.Person.Phone == searchCretiera || a.BookIndex == searchCretiera);
            }


            if (parameter.Searches != null)
            {

                if (parameter.Searches.PaymentType.Count() > 0)
                {
                    treeData = treeData.Where(q => parameter.Searches.PaymentType.Contains(q.PaymentType));
                }
                if (parameter.Searches.InvoiceTypeId.Count() > 0 || parameter.Searches.SubType.Count() > 0 || parameter.Searches.isExpenses)
                {
                  
                    treeData = treeData.Where(q => parameter.Searches.SubType.Contains(q.InvoiceSubTypesId) ||
                    (parameter.Searches.isExpenses?parameter.InvoiceTypeId==(int)DocumentType.Purchase && q.isExpenses== parameter.Searches.isExpenses 
                                       && q.InvoiceSubTypesId!=(int)SubType.PartialReturn && q.InvoiceSubTypesId!=(int)SubType.TotalReturn :false)
                       || (parameter.Searches.InvoiceTypeId.Contains(q.InvoiceTypeId) && !q.isExpenses && q.InvoiceSubTypesId==(int)SubType.Nothing));
                   
                }
                if (parameter.Searches.StoreId.Count() > 0)
                {
                    treeData = treeData.Where(q => parameter.Searches.StoreId.Contains(q.StoreId)); 
                }
                if (parameter.Searches.PersonId.Count() > 0)
                {
                    treeData = treeData.Where(q => parameter.Searches.PersonId.Contains(q.PersonId));
                }
                if (parameter.Searches.InvoiceDateFrom != null)
                    treeData = treeData.Where(q => q.InvoiceDate >= parameter.Searches.InvoiceDateFrom.Value.Date);
                if (parameter.Searches.InvoiceDateTo != null)
                    treeData = treeData.Where(q => q.InvoiceDate <= parameter.Searches.InvoiceDateTo.Value);
                if (parameter.Searches.itemId > 0)
                {

                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => a.ItemId == parameter.Searches.itemId)
                               .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));

                }
                if (parameter.Searches.categoryId > 0)
                {

                    var items = itemMasterQuery.TableNoTracking.Where(a => a.GroupId == parameter.Searches.categoryId)
                               .Select(a => a.Id);
                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => items.Contains(a.ItemId))
                             .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));
                }
            }
            var count = treeData.Count();
           

            var list2 = new List<AllInvoiceDto>();
            List<InvoiceMaster> response = new List<InvoiceMaster>();
            if (parameter.PageSize > 0 && parameter.PageNumber > 0)
            {
                response = treeData.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            }
            
            GetAllInvoicesService.GetAllInvoices(response, list2  );
            return new ResponseResult() { Id = null, Data = list2, DataCount = count, Result = list2.Count > 0 ? Result.Success : Result.NoDataFound, Note = "",TotalCount=totalCount };
           

        }

    }
}
