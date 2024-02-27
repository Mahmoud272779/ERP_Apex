using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.General;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class DeletePersonsHandler : IRequestHandler<DeletePersonsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IRepositoryCommand<InvPersons> PersonCommand;
        private readonly IRepositoryQuery<InvFundsCustomerSupplier> FundsCustomerSupplierQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterQuery;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IRepositoryCommand<InvFundsCustomerSupplier> FundsCustomerSupplierCommand;
        private readonly IRepositoryCommand<InvPersons_Branches> PersonBranchCommand;
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public DeletePersonsHandler(ISystemHistoryLogsService systemHistoryLogsService,
            IFinancialAccountBusiness financialAccountBusiness,
            IRepositoryCommand<InvPersons_Branches> personBranchCommand,
            IRepositoryCommand<InvFundsCustomerSupplier> fundsCustomerSupplierCommand,
            IRepositoryQuery<InvFundsCustomerSupplier> fundsCustomerSupplierQuery,
            IRepositoryCommand<InvPersons> personCommand,
            iAuthorizationService iAuthorizationService,
            IRepositoryQuery<InvPersons> personQuery,
            IRepositoryQuery<InvoiceMaster> invoiceMasterQuery,
            IRepositoryQuery<GlReciepts> glRecieptsQuery,
            IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery,
            IDeletedRecordsServices deletedRecords)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            _financialAccountBusiness = financialAccountBusiness;
            PersonBranchCommand = personBranchCommand;
            FundsCustomerSupplierCommand = fundsCustomerSupplierCommand;
            FundsCustomerSupplierQuery = fundsCustomerSupplierQuery;
            PersonCommand = personCommand;
            _iAuthorizationService = iAuthorizationService;
            PersonQuery = personQuery;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _GlRecieptsQuery = glRecieptsQuery;
            _OfferPriceMasterQuery = offerPriceMasterQuery;
            _deletedRecords = deletedRecords;
        }

        public async Task<ResponseResult> Handle(DeletePersonsRequest request, CancellationToken cancellationToken)
        {
            List<int> deletedList = new List<int>();

            var allPersons = PersonQuery.TableNoTracking;
            var persons = await PersonQuery.GetAllIncludingAsync(0, 0,
                e => request.Ids.Contains(e.Id) && e.Id != 1 && e.Id != 2,
                im => im.InvoiceMaster,
                pb => pb.PersonBranch,
                dap => dap.Discount_A_P,
                r => r.reciept,
                fcs => fcs.FundsCustomerSuppliers);

            if (persons.Where(x => x.IsSupplier == true).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Suppliers_Purchases, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (persons.Where(x => x.IsCustomer == true).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Customers_Sales, Opretion.Delete);
                if (isAuthorized != null)
                    return isAuthorized;
            }


            var listOfCantDelete = new List<InvPersons>();
            foreach (var item in persons)
            {
                if (item.IsSupplier == true && item.IsCustomer == true)
                    listOfCantDelete.Add(item);
            }


            var listOfChangedElements = new List<InvPersons>();
            if (listOfCantDelete.Any())
            {
                foreach (var item in listOfCantDelete)
                {
                    if (request.isSupplierPage)
                    {
                        item.IsSupplier = false;
                        item.CodeT = "C";
                        item.AddToAnotherList = false;
                        item.Code = allPersons.Where(x => x.CodeT == "S").OrderByDescending(x => x.Code).FirstOrDefault().Code + 1;
                    }
                    else
                    {
                        item.IsCustomer = false;
                        item.CodeT = "S";
                        item.AddToAnotherList = false;
                        item.Code = allPersons.Where(x => x.CodeT == "C").OrderByDescending(x => x.Code).FirstOrDefault().Code + 1;
                    }
                    listOfChangedElements.Add(item);
                    deletedList.Add(item.Id);

                }
                await PersonCommand.UpdateAsyn(listOfChangedElements);
            }
            var listOfCanDelete = new List<InvPersons>();
            foreach (var item in persons)
            {
                if (item.IsSupplier == false || item.IsCustomer == false && !listOfCantDelete.Where(x => x.Id == item.Id).Any())
                {
                    var canDelete = await PersonHelper.canDelete(allPersons.ToList(), item.Id, item.IsSupplier, _InvoiceMasterQuery.TableNoTracking, _GlRecieptsQuery.TableNoTracking, _OfferPriceMasterQuery);
                    if (!canDelete)
                        continue;
                    listOfCanDelete.Add(item);
                }
            }

            if (listOfCanDelete != null)
            {
                var FA_Ids = new List<int>();
                foreach (var person in listOfCanDelete)
                {
                    if (person.InvoiceMaster.Count == 0
                        && person.Discount_A_P.Count == 0
                        && person.reciept.Count == 0)
                    {
                        var funds = (await FundsCustomerSupplierQuery.GetAllAsyn(e => e.PersonId == person.Id)).FirstOrDefault();
                        if (funds.Credit == 0 && funds.Debit == 0)
                        {
                            await FundsCustomerSupplierCommand.DeleteAsync(e => e.Id == funds.Id);
                            await PersonBranchCommand.DeleteAsync(e => e.PersonId == person.Id && person.PersonBranch.Select(s => s.BranchId).ToArray().Contains(e.BranchId));
                            await PersonCommand.DeleteAsync(person.Id);
                            deletedList.Add(person.Id);
                            if (person.FinancialAccountId != null && person.FinancialAccountId > 0)
                                if (!FA_Ids.Where(x => x == person.FinancialAccountId).Any())
                                    FA_Ids.Add((int)person.FinancialAccountId);
                        }

                    }
                }
                //if (FA_Ids.Any())
                //{
                //    foreach (var item in FA_Ids)
                //    {
                        var deletedAccount = await _financialAccountBusiness.DeleteFinancialAccountAsync(new SharedRequestDTOs.Delete()
                        {
                            //userId = userInfo.userId,
                            Ids = FA_Ids.ToArray()
                           // Ids = new int[] { item }
                        });
                //    }
                //}
            }
            var count = listOfCanDelete.Count();
            if (deletedList.Any())
            {
                if (listOfChangedElements.Where(x => x.IsCustomer).Any() || listOfCanDelete.Where(x => x.IsCustomer).Any())
                    await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.deleteCustomer, count);
                else if (listOfChangedElements.Where(x => x.IsSupplier).Any() || listOfCanDelete.Where(x => x.IsSupplier).Any())
                    await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.deleteSupplier,count);
            }

            //Fill The DeletedRecordTable

            _deletedRecords.SetDeletedRecord(deletedList, 3);
            /*var listRecords = new List<DeletedRecords>();
            foreach (var item in deletedList)
            {
                var deleted = new DeletedRecords
                {
                    Type = 3,
                    DTime = DateTime.Now,
                    RecordID = item
                };
                listRecords.Add(deleted);
            }
             _deletedRecordCommand.AddRangeAsync(listRecords);*/

            //await _deletedRecordCommand.SaveChanges();
            return new ResponseResult() { Data = deletedList, Id = null, Result = deletedList.Any() ? Result.Success : Result.NotFound };

        }
    }
}
