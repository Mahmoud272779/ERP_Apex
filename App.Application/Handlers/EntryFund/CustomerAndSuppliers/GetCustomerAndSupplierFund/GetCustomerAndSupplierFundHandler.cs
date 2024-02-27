using App.Application.Handlers.PrivateChat.Chat;
using App.Domain.Models.Response.Store.Invoices;
using App.Infrastructure.settings;
using AutoMapper.Configuration.Annotations;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.EntryFund.CustomerAndSuppliers.GetCustomerAndSupplierFund
{
    public class GetCustomerAndSupplierFundHandler : IRequestHandler<GetCustomerAndSupplierFundRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvFundsCustomerSupplier> _InvFundsCustomerSupplier;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;

        public GetCustomerAndSupplierFundHandler(iUserInformation iUserInformation, IRepositoryQuery<InvFundsCustomerSupplier> invFundsCustomerSupplier, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery, IRepositoryQuery<InvPersons> invPersonsQuery)
        {
            _iUserInformation = iUserInformation;
            _InvFundsCustomerSupplier = invFundsCustomerSupplier;
            _invGeneralSettingsQuery = invGeneralSettingsQuery;
            _InvPersonsQuery = invPersonsQuery;
        }
        public async Task<ResponseResult> Handle(GetCustomerAndSupplierFundRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var allData = _InvPersonsQuery
                .TableNoTracking
                .Include(c=> c.PersonBranch)
                .Where(c => c.PersonBranch.Select(x => x.BranchId).Contains(userInfo.CurrentbranchId))
                .Where(c => c.IsCustomer == request.isCustomer)
                .Where(c => c.IsSupplier == request.isSupplier);

            var totalCount = allData.Count();
            if(!string.IsNullOrEmpty(request.SearchCriteria))
                allData = allData
                    .Where(c =>  (c.ArabicName.Contains(request.SearchCriteria) || c.LatinName.Contains(request.SearchCriteria) || c.Code.ToString().Contains(request.SearchCriteria)) || c.Phone == request.SearchCriteria);
            var dataCount = allData.Count();

            int nameOutput;
            var tryParseName = int.TryParse(request.SearchCriteria, out nameOutput);
            if (tryParseName)
            {
                allData = allData
                        .OrderBy(x => x.Code)
                        .ThenBy(x => x.ArabicName)
                        .ThenBy(x => x.LatinName);
            }
            else
            {
                allData = allData
                        .OrderByDescending(a => a.ArabicName == request.SearchCriteria)
                        .ThenByDescending(a => a.ArabicName.StartsWith(request.SearchCriteria));

            }

            var res = allData.Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
                .ToList();

            var funds = _InvFundsCustomerSupplier.TableNoTracking
                .Where(c=> c.branchId == userInfo.CurrentbranchId)
                .Where(c => res.Select(x => x.Id).Contains(c.PersonId));

            var response = res
                .ToList()
                .Select(c => new FundsCustomerandSuppliersDto
                {
                    Id = c.Id,
                    ArabicName = c.ArabicName,
                    LatinName = c.LatinName,
                    Code = c.Code.ToString(),
                    Credit = funds.FirstOrDefault(x=> x.PersonId == c.Id).Credit,
                    Debit = funds.FirstOrDefault(x => x.PersonId == c.Id).Debit,
                    Type = c.Type,
                    IsCustomer = c.IsCustomer,
                    PersonId = c.Id
                });


               
            var generalSettings = _invGeneralSettingsQuery.TableNoTracking.Where(x => x.Id == 1).FirstOrDefault();
            var Funds_Supplires_Date = generalSettings.Funds_Supplires_Date != null ? generalSettings.Funds_Supplires_Date : DateTime.Now;
            var Funds_Customers_Date = generalSettings.Funds_Customers_Date != null ? generalSettings.Funds_Customers_Date : DateTime.Now;
            var final = new App.Domain.Models.Response.Store.Invoices.response()
            {
                FundsList = response,
                Date = request.Type == 1  ? Funds_Supplires_Date.Value.ToString(defultData.datetimeFormat) : Funds_Customers_Date.Value.ToString(defultData.datetimeFormat)
            };
            return new ResponseResult
            {
                Result = Result.Success,
                Data = final,
                DataCount = dataCount,
                TotalCount = totalCount
            };
        }
    }
}
