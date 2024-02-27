using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process
{
    public  class GetAllExtractPermissionService :BaseClass, IGetAllExtractPermissionService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;
        public GetAllExtractPermissionService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
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
        public async Task<ResponseResult> GetAllExtractPermission(StoreSearchPagination parameter)
        {
            var searchCretiera = parameter.Searches.SearchCriteria;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
         
            var treeData = InvoiceMasterRepositoryQuery.TableNoTracking
                          .Include(x => x.store).Where(q => q.BranchId == userInfo.CurrentbranchId &&
                          ((q.InvoiceTypeId == (int)DocumentType.ExtractPermission && q.IsDeleted == false) ||
                            q.InvoiceTypeId == (int)DocumentType.DeleteExtractPermission));


            var totalCount = treeData.Count();
            if (string.IsNullOrEmpty(searchCretiera))
            {
                treeData = treeData.OrderByDescending(x => x.Code);
            }
            else
            {
                treeData = treeData.OrderBy(x => x.Code.ToString().Contains(searchCretiera));
            }

            if (!string.IsNullOrEmpty(searchCretiera))
            {
                treeData = treeData.Where
                                     (x =>
                                             x.Code.ToString().Contains(searchCretiera) ||
                                             x.InvoiceType == searchCretiera ||
                                             x.BookIndex==searchCretiera
                                     );
            }


            if (parameter.Searches != null)
            { 
                if (parameter.Searches.InvoiceTypeId.Any())
                {
                    treeData = treeData.Where(q => parameter.Searches.InvoiceTypeId.Contains(q.InvoiceTypeId));
                } 
                if (parameter.Searches.StoreId.Count() > 0)
                {
                    treeData = treeData.Where(q => parameter.Searches.StoreId.Contains(q.StoreId));
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
            List<InvoiceMaster> response = new List<InvoiceMaster>();
            var list2 = new List<AllInvoiceDto>();

            if (parameter.PageSize > 0 && parameter.PageNumber > 0)
            {
                response = treeData.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }

            GetAllInvoicesService.GetAllInvoices(response, list2);
            return new ResponseResult() { Id = null, Data = list2, DataCount = count, Result = list2.Count > 0 ? Result.Success : Result.NoDataFound, Note = "", TotalCount = totalCount };


        }

    }
}
