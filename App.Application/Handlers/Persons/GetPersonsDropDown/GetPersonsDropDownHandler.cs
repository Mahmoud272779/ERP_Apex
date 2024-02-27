using App.Application.Handlers.Persons.GetPersonBalance;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.Inv_General_Settings;
using App.Domain.Models.Security.Authentication.Response.Store;
using FastReport.Utils;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class GetPersonsDropDownHandler : IRequestHandler<GetPersonsDropDownRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly IReceiptsService receiptsService;
        private readonly IMediator mediator;
        private readonly IInvGeneralSettingsService invGeneralSettingsService;

        public GetPersonsDropDownHandler(IRepositoryQuery<InvPersons> personQuery, iUserInformation iUserInformation, IReceiptsService receiptsService, IMediator mediator, IInvGeneralSettingsService invGeneralSettingsService)
        {
            PersonQuery = personQuery;
            _iUserInformation = iUserInformation;
            this.receiptsService = receiptsService;
            this.mediator = mediator;
            this.invGeneralSettingsService = invGeneralSettingsService;
        }

        public async Task<ResponseResult> Handle(GetPersonsDropDownRequest request, CancellationToken cancellationToken)
        {
            var userInformation = await _iUserInformation.GetUserInformation();
            if (userInformation == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };

            var setting = (Other)invGeneralSettingsService.GetOtherSettings().Result.Data;
            ResponseResult responseResult = new ResponseResult();
            await Task.Run(async () =>
            {
                var person = new List<personsForBalanceDto>();

                var data = PersonQuery.TableNoTracking
                                      .Include(x => x.SalesMan)
                                      .Include(x => x.PersonBranch)
                                      .Where(x => request.IsSupplier ? x.IsSupplier : x.IsCustomer)
                                      .Where(e => (!request.isReport ? e.PersonBranch.Select(x => x.BranchId).ToArray().Contains(userInformation.CurrentbranchId) :
                                                  e.PersonBranch.Select(x => x.BranchId).ToArray().Any(c=> userInformation.employeeBranches.Contains(c)))
                                                  || (e.Id == 2 || e.Id == 1))
                                      .Where(e => request.invoiceTypeId == 0 ? true : e.Status == (int)Status.Active)
                                      .Where(e => request.SearchCriteria != null ? e.ArabicName.Contains(request.SearchCriteria) || e.LatinName.Contains(request.SearchCriteria) || e.Code.ToString().Contains(request.SearchCriteria)
                                      || e.Phone.Equals(request.SearchCriteria) : true)
                                      .Where(a => !userInformation.otherSettings.showCustomersOfOtherUsers ? (a.InvEmployeesId == userInformation.employeeId || (a.Id == 2 || a.Id == 1)) : true)
                                      .Select(a => (new personsForBalanceDto()
                                      {
                                          Id = a.Id,
                                          Code = a.Code,
                                          ArabicName =string.Concat(a.ArabicName," - كود ",a.Code),// $"{(a.Code)} - كود "+ a.ArabicName ,
                                          LatinName = string.Concat(a.LatinName, " - code ", a.Code),// a.LatinName+" - code " + (a.Code).ToString(),
                                          Status = a.Status
                                      ,
                                          Phone = a.Phone,
                                          SalesManId = a.SalesManId,
                                          salesManNameAr = a.SalesMan.ArabicName,
                                          salesManNameEn = a.SalesMan.LatinName,
                                          CodeT = a.CodeT,
                                          Discount = a.DiscountRatio,
                                          balance = 0
                                      })).OrderBy(a=>a.Id).ToList();
                //            true) || (e.Id == 2 || e.Id == 1))
                //)

                if (!string.IsNullOrEmpty(request.Code) && request.Code != "undefined")
                    data = data.Where(x => x.Code.ToString() == request.Code || (x.CodeT + "-" + x.Code.ToString()) == request.Code).ToList();
                responseResult.DataCount = data.Count();
                if (request.personId != null)
                {
                    data = data.Where(x => x.Id == request.personId).ToList();
                }
                double MaxPageNumber = data.ToList().Count() / Convert.ToDouble(request.PageSize);
                var countofFilter = Math.Ceiling(MaxPageNumber);

                if (!string.IsNullOrEmpty(request.SearchCriteria))
                    data = data.OrderByDescending(a => a.ArabicName == request.SearchCriteria)
                    .ThenByDescending(a => a.ArabicName.StartsWith(request.SearchCriteria)).ToList();

                if (request.PageSize > 0 && request.PageNumber > 0)
                {
                    data = data.ToList().Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
                }
                //    Mapping.Mapper.Map(data, person);
                if (setting.Other_ShowBalanceOfPerson && (((request.invoiceTypeId == (int)DocumentType.Purchase || request.invoiceTypeId == (int)DocumentType.wov_purchase)
                      && userInformation.otherSettings.purchaseShowBalanceOfPerson)
                      || ((request.invoiceTypeId == (int)DocumentType.Sales)
                      && userInformation.otherSettings.salesShowBalanceOfPerson)))
                {
                    int authory = (int)AuthorityTypes.suppliers;

                    if (request.invoiceTypeId == (int)DocumentType.Sales)
                        authory = (int)AuthorityTypes.customers;

                    var res = await mediator.Send(new GetReceiptBalanceForBenifitForInvoicesRequest() { AuthorityId = authory, persons = data,BranchId= userInformation.CurrentbranchId });
                    
                    responseResult.Data = res.Data;
                    //var peresonData = (List<personsForBalanceDto>)res.Data;

                    ////sales settings
                    //if (authory == (int)AuthorityTypes.customers)
                    //{
                    //    userInformation.otherSettings.salesShowAllPersonBalance = false;
                    //    userInformation.otherSettings.salesShowDebitorPersonBalance = true;


                    //    if (!userInformation.otherSettings.salesShowAllPersonBalance)
                    //    {
                    //        foreach (var personBalance in peresonData)
                    //        {
                    //            if (userInformation.otherSettings.salesShowDebitorPersonBalance)
                    //            {
                    //                personBalance.balance = personBalance.Debtor;
                    //                personBalance.isCreditor = false;
                    //            }
                    //            else if (userInformation.otherSettings.salesShowCreditorPersonBaalance)
                    //            {
                    //                personBalance.balance = personBalance.Creditor;
                    //                personBalance.isCreditor = true;
                    //            }
                    //        }

                    //        responseResult.Data = peresonData;
                    //    }

                    //}
                    ////purhases settings
                    //else if (authory == (int)AuthorityTypes.suppliers)
                    //{
                    //    if (!userInformation.otherSettings.purchaseShowAllPersonBalance)
                    //    {
                    //        foreach (var personBalance in peresonData)
                    //        {
                    //            if (userInformation.otherSettings.purchaseShowDebitorPersonBalance)
                    //            {
                    //                personBalance.balance = personBalance.Debtor;
                    //                personBalance.isCreditor = false;
                    //            }
                    //            else if (userInformation.otherSettings.purchaseShowCreditorPersonBaalance)
                    //            {
                    //                personBalance.balance = personBalance.Creditor;
                    //                personBalance.isCreditor = true;
                    //            }
                    //        }

                    //        responseResult.Data = peresonData;
                    //    }
                    //}

                }
                else
                    responseResult.Data = data;

                var totalCount = PersonQuery.TableNoTracking.Where(e => e.Status == (int)Status.Active).Count();


                responseResult.Result = data.Any() ? Result.Success : Result.Failed;
                responseResult.TotalCount = totalCount;
                responseResult.Note = (countofFilter == request.PageNumber ? Actions.EndOfData : "");
            });
            return responseResult;
        }
    }
}
