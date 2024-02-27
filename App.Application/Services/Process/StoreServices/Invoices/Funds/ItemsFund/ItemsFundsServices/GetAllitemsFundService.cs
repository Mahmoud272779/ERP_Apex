using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.IItemsFundsServices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.ItemsFund.ItemsFundsServices
{
    public class GetAllitemsFundService : BaseClass, IGetAllitemsFundService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;
        public GetAllitemsFundService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
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
        public async Task<ResponseResult> GetAllItemsFund(StoreSearchPagination parameter)
        {
            var searchCretiera = parameter.Searches.SearchCriteria;
            UserInformationModel userInfo = Userinformation.GetUserInformation().Result;
            var treeData = await InvoiceMasterRepositoryQuery.GetAllIncludingAsync(0, 0,
                   q => ((q.InvoiceTypeId == (int)DocumentType.itemsFund && q.IsDeleted == false) ||
                   q.InvoiceTypeId == (int)DocumentType.DeleteItemsFund) && q.BranchId == userInfo.CurrentbranchId
                   && (searchCretiera == string.Empty || searchCretiera == null || q.InvoiceType == searchCretiera ||
                   q.Code.ToString().Contains(searchCretiera) ||
              q.BookIndex==searchCretiera),
              e => (string.IsNullOrEmpty(searchCretiera) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(searchCretiera)) ? 0 : 1))
              , a => a.store, a => a.InvoicesDetails);
            var list = treeData.ToList();

            if (parameter.Searches != null)
            {

                if (parameter.Searches.StoreId.Count() > 0)
                {
                    list = list.Where(q => parameter.Searches.StoreId.Contains(q.StoreId)).ToList();
                }
                if (parameter.Searches.InvoiceTypeId.Count() > 0)
                {
                    list = list.Where(q => parameter.Searches.InvoiceTypeId.Contains(q.InvoiceTypeId)).ToList();
                }
                /*  if (parameter.Searches.ItemId.Count() > 0)
                  {
                      list = list.Where(q => ( parameter.Searches.ItemId.Intersect(q.InvoicesDetails.Select(a=>a.ItemId).ToList()).Count()>0)).ToList();
                  }*/
                if (parameter.Searches.InvoiceDateFrom != null)
                    list = list.Where(q => q.InvoiceDate >= parameter.Searches.InvoiceDateFrom.Value.Date).ToList();
                if (parameter.Searches.InvoiceDateTo != null)
                    list = list.Where(q => q.InvoiceDate <= parameter.Searches.InvoiceDateTo.Value.Date).ToList();

                if (parameter.Searches.itemId > 0)
                {

                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => a.ItemId == parameter.Searches.itemId)
                               .Select(a => a.InvoiceId);
                    list = list.Where(a => invoiceIds.Contains(a.InvoiceId)).ToList();

                }
                if (parameter.Searches.categoryId > 0)
                {

                    var items = itemMasterQuery.TableNoTracking.Where(a => a.GroupId == parameter.Searches.categoryId)
                               .Select(a => a.Id);
                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => items.Contains(a.ItemId))
                             .Select(a => a.InvoiceId);
                    list = list.Where(a => invoiceIds.Contains(a.InvoiceId)).ToList();
                }
            }


            var count = list.Count();
            var list2 = new List<AllInvoiceDto>();

            if (parameter.PageSize > 0 && parameter.PageNumber > 0)
            {
                list = list.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            var totalCount = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => Lists.ItemsFundList.Contains(a.InvoiceTypeId) && a.BranchId==userInfo.CurrentbranchId).Count();
            GetAllInvoicesService.GetAllInvoices(list, list2);
            return new ResponseResult() { Id = null, Data = list2, DataCount = count, Result = list2.Count > 0 ? Result.Success : Result.NoDataFound, Note = "", TotalCount = totalCount };


        }

    }
}
