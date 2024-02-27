﻿using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
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

namespace App.Application.Services.Process.Invoices.Return_Purchase.ReturnPurchaseService
{
    public class GetAllReturnPurchaseService : BaseClass, IGetAllReturnPurchaseService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;

        public GetAllReturnPurchaseService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
            IGetAllInvoicesService _GetAllInvoicesService, iUserInformation Userinformation,
            IHttpContextAccessor _httpContext, IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery, IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            GetAllInvoicesService = _GetAllInvoicesService;
            httpContext = _httpContext;
            this.Userinformation = Userinformation;
            InvoiceDetailsRepositoryQuery = invoiceDetailsRepositoryQuery;
            this.itemMasterQuery = itemMasterQuery;
        }
        public async Task<ResponseResult> GetAllReturnPurchase(InvoiceSearchPagination Request)
        {
            var searchCretiera = Request.Searches.SearchCriteria;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            var DataFromDb = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.BranchId == userInfo.CurrentbranchId).ToList().Count();
            if (DataFromDb == 0)
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Success };

            var treeData = InvoiceMasterRepositoryQuery.TableNoTracking.Include(a => a.store)
                .Include(b => b.Person).Where(q => q.InvoiceTypeId == (int)DocumentType.ReturnPurchase  && q.BranchId==userInfo.CurrentbranchId).ToList();
          
                if (Request.Searches.PaymentType.Count() > 0)
                   treeData = treeData.Where(q => Request.Searches.PaymentType.Contains(q.PaymentType)).ToList();

            if (Request.Searches.SubType.Count() > 0)
                treeData = treeData.Where(q => Request.Searches.SubType.Contains(q.InvoiceSubTypesId)).ToList();

            if (Request.Searches.InvoiceDateFrom != null)
                    treeData = treeData.Where(q => q.InvoiceDate >= Request.Searches.InvoiceDateFrom.Value.Date).ToList();
            
            if (Request.Searches.InvoiceDateTo != null)
                    treeData = treeData.Where(q => q.InvoiceDate <= Request.Searches.InvoiceDateTo.Value.Date).ToList();
            if (Request.Searches.itemId > 0)
            {

                var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => a.ItemId == Request.Searches.itemId)
                           .Select(a => a.InvoiceId);
                treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId)).ToList();

            }
            if (Request.Searches.categoryId > 0)
            {

                var items = itemMasterQuery.TableNoTracking.Where(a => a.GroupId == Request.Searches.categoryId)
                           .Select(a => a.Id);
                var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => items.Contains(a.ItemId))
                         .Select(a => a.InvoiceId);
                treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId)).ToList();
            }
            var list = treeData.OrderByDescending(a => a.Code).ToList().ToList();
            var count = list.Count();
            var list2 = new List<AllInvoiceDto>();

            if (Request.PageSize > 0&& Request.PageNumber>0)
            {
                 list = list.Skip((Request.PageNumber-1) * Request.PageSize).Take(Request.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            var totalCount = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a=>a.InvoiceTypeId==(int)DocumentType.ReturnPurchase).Count();
            GetAllInvoicesService.GetAllInvoices(list, list2 );
                return new ResponseResult() { Id = null, Data = list2, DataCount = count, Result = list2.Count > 0 ? Result.Success : Result.NoDataFound, Note = "" , TotalCount=totalCount };
            
        }

    }
}
