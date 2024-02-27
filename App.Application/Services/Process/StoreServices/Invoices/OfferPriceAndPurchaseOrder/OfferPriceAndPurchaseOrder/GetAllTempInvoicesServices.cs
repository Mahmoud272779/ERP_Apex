using App.Application.Handlers.Invoices.sales.GetAllSales;
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
    internal class GetAllTempInvoicesServices : BaseClass, IGetAllTempInvoicesServices
    {
        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<OfferPriceDetails> OfferPriceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;

        public GetAllTempInvoicesServices(IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterRepositoryQuery,
            IGetAllInvoicesService _GetAllInvoicesService, iUserInformation Userinformation,
            IHttpContextAccessor _httpContext, IRepositoryQuery<OfferPriceDetails> offerPriceDetailsRepositoryQuery, IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery) : base(_httpContext)
        {
            OfferPriceMasterRepositoryQuery = _OfferPriceMasterRepositoryQuery;
            GetAllInvoicesService = _GetAllInvoicesService;
            httpContext = _httpContext;
            this.Userinformation = Userinformation;
            OfferPriceDetailsRepositoryQuery = offerPriceDetailsRepositoryQuery;
            this.itemMasterQuery = itemMasterQuery;
        }
        public async Task<ResponseResult> GetAllTempInvoices(GetAllOfferPriceRequest parameter,int mainInvoiceTypeId,int deletedInvoiceTypeId)
        {
            var searchCretiera = parameter.Searches.SearchCriteria;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var treeData = OfferPriceMasterRepositoryQuery.TableNoTracking
                .Include(x => x.store)
                .Include(x => x.Person)
                .Where(q => q.BranchId == userInfo.CurrentbranchId);
            

     
                treeData = treeData.Where(q => ((q.InvoiceTypeId == mainInvoiceTypeId && q.IsDeleted == false) ||
                                    q.InvoiceTypeId == deletedInvoiceTypeId));
            if (!userInfo.otherSettings.showOfferPricesOfOtherUser)
            {
                treeData = treeData.Where(a => a.EmployeeId == userInfo.employeeId);
            }

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
                                             //     x.Person.Phone.Contains(searchCretiera) ||
                                             x.BookIndex.Contains(searchCretiera)
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

             
                if (parameter.Searches.SubType.Any())
                {
                    treeData = treeData.Where(q => parameter.Searches.SubType.Contains(q.InvoiceSubTypesId));
                }
              
                if (parameter.Searches.PersonId.Count() > 0)
                {
                    treeData = treeData.Where(q => parameter.Searches.PersonId.Contains(q.PersonId));
                }
                if (parameter.Searches.InvoiceDateFrom != null)
                    treeData = treeData.Where(q => q.InvoiceDate.Date >= parameter.Searches.InvoiceDateFrom.Value.Date);
                if (parameter.Searches.InvoiceDateTo != null)
                    treeData = treeData.Where(q => q.InvoiceDate.Date <= parameter.Searches.InvoiceDateTo.Value);

                if (parameter.Searches.itemId > 0)
                {

                    var invoiceIds = OfferPriceDetailsRepositoryQuery.TableNoTracking.Where(a => a.ItemId == parameter.Searches.itemId)
                               .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));

                }
                if (parameter.Searches.categoryId > 0)
                {

                    var items = itemMasterQuery.TableNoTracking.Where(a => a.GroupId == parameter.Searches.categoryId)
                               .Select(a => a.Id);
                    var invoiceIds = OfferPriceDetailsRepositoryQuery.TableNoTracking.Where(a => items.Contains(a.ItemId))
                             .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));
                }
            }
          

            var count = treeData.Count();
            List<OfferPriceMaster> response = new List<OfferPriceMaster>();

            var list2 = new List<AllInvoiceDto>();


            if (parameter.PageSize > 0 && parameter.PageNumber > 0)
            {
                response = treeData.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            var data = Mapping.Mapper.Map<List<OfferPriceMaster>, List<InvoiceMaster>>(response);
            GetAllInvoicesService.GetAllInvoices(data, list2);
            return new ResponseResult() { Id = null, Data = list2, DataCount = count, Result = list2.Count > 0 ? Result.Success : Result.NoDataFound, Note = "", TotalCount = totalCount };


        }

    }
}
