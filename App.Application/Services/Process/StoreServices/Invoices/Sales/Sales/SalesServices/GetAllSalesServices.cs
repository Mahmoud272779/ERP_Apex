using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    internal class GetAllSalesServices : BaseClass, IGetAllSalesServices
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;
         private readonly IHttpContextAccessor httpContext;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private readonly iUserInformation Userinformation;

        public GetAllSalesServices(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
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
        public async Task<ResponseResult> GetAllSales(InvoiceSearchPagination parameter  )
        {
            var searchCretiera = parameter.Searches.SearchCriteria;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var treeData = InvoiceMasterRepositoryQuery.TableNoTracking
                .Include(x => x.store)
                .Include(x => x.Person)
                .Where(q => q.BranchId == userInfo.CurrentbranchId);

       
                treeData = treeData.Where(q =>
                           (parameter.InvoiceTypeId == (int)DocumentType.Sales ?
                           ((q.InvoiceTypeId == (int)DocumentType.Sales && q.IsDeleted == false) ||
                           q.InvoiceTypeId == (int)DocumentType.DeleteSales) :
                           q.InvoiceTypeId == (int)DocumentType.ReturnSales));
         

            if (!userInfo.otherSettings.salesShowOtherPersonsInv)
            {
                treeData = treeData.Where(a => a.EmployeeId == userInfo.employeeId);
            }
            //treeData = treeData.Where(q =>  (parameter.InvoiceTypeId == (int)DocumentType.Sales ? ((q.InvoiceTypeId == (int)DocumentType.Sales && q.IsDeleted == false) ||
            //                        q.InvoiceTypeId == (int)DocumentType.DeleteSales) : q.InvoiceTypeId == (int)DocumentType.ReturnSales)
            //    );

            //if (parameter.InvoiceTypeId == (int)DocumentType.POS)      
            //    treeData = treeData.Where(q => ((q.InvoiceTypeId == (int)DocumentType.POS && q.IsDeleted == false) ||
            //                        q.InvoiceTypeId == (int)DocumentType.DeletePOS));
            //else if(parameter.InvoiceTypeId == (int)DocumentType.ReturnPOS)
            //    treeData = treeData.Where(q => (q.InvoiceTypeId == (int)DocumentType.ReturnPOS));
            //else if (parameter.InvoiceTypeId == (int)DocumentType.Sales)
            //    treeData = treeData.Where(q => (parameter.InvoiceTypeId == (int)DocumentType.Sales ? ((q.InvoiceTypeId == (int)DocumentType.Sales && q.IsDeleted == false) ||
            //                        q.InvoiceTypeId == (int)DocumentType.DeleteSales) : q.InvoiceTypeId == (int)DocumentType.ReturnSales));

            //else if (parameter.InvoiceTypeId == (int)DocumentType.ReturnSales)
            //    treeData = treeData.Where(q => ( q.InvoiceTypeId == (int)DocumentType.ReturnSales));

            //if(parameter.InvoiceTypeId == (int)DocumentType.POS || parameter.InvoiceTypeId == (int)DocumentType.pos)


            var totalCount = treeData.Count();
       

            if (!string.IsNullOrEmpty(searchCretiera))
            {
                treeData = treeData.Where
                                     (x =>
                                             x.Code.ToString().Contains(searchCretiera) ||
                                             x.InvoiceType == searchCretiera ||
                                             x.Person.ArabicName.ToLower().Contains(searchCretiera) ||
                                             x.Person.LatinName.ToLower().Contains(searchCretiera) ||
                                           x.Person.Phone==searchCretiera ||
                                             // x.Person.Phone.Contains(searchCretiera) ||
                                             x.BookIndex== searchCretiera||(x.ParentInvoiceCode !=null?x.ParentInvoiceCode==searchCretiera:true)
                                             || (x.ParentInvoiceCode != null ? x.ParentInvoiceCode.Substring(2) == searchCretiera : true)
                                     );
            }


            if (string.IsNullOrEmpty(searchCretiera))
            {
                treeData = treeData.OrderByDescending(q => q.Code);
            }
            else
            {
                treeData = treeData.OrderBy(q => q.Code);
 
            }

            if (parameter.Searches != null)
            {
               
                if (parameter.Searches.PaymentType.Count() > 0)
                {
                    treeData = treeData.Where(q => parameter.Searches.PaymentType.Contains(q.PaymentType));
                }
                if (parameter.Searches.InvoiceTypeId.Count() > 0 || parameter.Searches.SubType.Count() > 0)
                {

                    treeData = treeData.Where(q => parameter.Searches.SubType.Contains(q.InvoiceSubTypesId)
                       || (parameter.Searches.InvoiceTypeId.Contains(q.InvoiceTypeId) && q.InvoiceSubTypesId == (int)SubType.Nothing));
                }


                //if (parameter.Searches.InvoiceTypeId.Any() && parameter.InvoiceTypeId !=(int)DocumentType.ReturnSales)
                //{

                //    treeData = treeData.Where(q => parameter.Searches.InvoiceTypeId.Contains(q.InvoiceTypeId));
                //}
                if (parameter.Searches.SubType.Any() && parameter.InvoiceTypeId != (int)DocumentType.ReturnSales)
                {

                    treeData = treeData.Where(q => parameter.Searches.SubType.Contains(q.InvoiceSubTypesId));
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
                    treeData = treeData.Where(q => q.InvoiceDate.Date >= parameter.Searches.InvoiceDateFrom.Value.Date);
                if (parameter.Searches.InvoiceDateTo != null)
                    treeData = treeData.Where(q => q.InvoiceDate.Date <= parameter.Searches.InvoiceDateTo.Value);
                if(parameter.Searches.itemId>0)
                {
                   
                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a=> a.ItemId==parameter.Searches.itemId)
                               .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));

                }
                if(parameter.Searches.categoryId>0)
                {

                    var items = itemMasterQuery.TableNoTracking.Where(a => a.GroupId==parameter.Searches.categoryId)
                               .Select(a => a.Id);
                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => items.Contains(a.ItemId ))
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
