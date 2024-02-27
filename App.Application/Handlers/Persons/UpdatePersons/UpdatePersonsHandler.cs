using App.Application.Helpers.Service_helper.History;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class UpdatePersonsHandler : IRequestHandler<UpdatePersonsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLBranch> branchesRepositoryQuery;
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly IRepositoryCommand<InvPersons> PersonCommand;
        private readonly IRepositoryQuery<InvPersons_Branches> _personBranchQuery;
        private readonly IRepositoryCommand<InvPersons_Branches> PersonBranchCommand;
        private readonly IHistory<InvPersonsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<GLIntegrationSettings> _GLIntegrationSettingsQuery;
        private readonly iUserInformation _iUserInformation;

        public UpdatePersonsHandler(ISystemHistoryLogsService systemHistoryLogsService, IRepositoryQuery<GLBranch> branchesRepositoryQuery, IRepositoryQuery<InvPersons> personQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<GLGeneralSetting> gLGeneralSettingQuery, IRepositoryCommand<InvPersons> personCommand, IRepositoryQuery<InvPersons_Branches> personBranchQuery, IRepositoryCommand<InvPersons_Branches> personBranchCommand, IHistory<InvPersonsHistory> history, IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery, iUserInformation iUserInformation)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.branchesRepositoryQuery = branchesRepositoryQuery;
            PersonQuery = personQuery;
            _invoiceMasterQuery = invoiceMasterQuery;
            _gLGeneralSettingQuery = gLGeneralSettingQuery;
            PersonCommand = personCommand;
            _personBranchQuery = personBranchQuery;
            PersonBranchCommand = personBranchCommand;
            this.history = history;
            _GLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
            _iUserInformation = iUserInformation;
        }

        public async Task<ResponseResult> Handle(UpdatePersonsRequest parameters, CancellationToken cancellationToken)
        {
            var checkBranch = await branchesHelper.CheckIsBranchExist(parameters.Branches, branchesRepositoryQuery);
            if (checkBranch != null)
                return checkBranch;
            //This To Remove Add To another List option
            parameters.AddToAnotherList = false;


            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            if (parameters.Type < (int)PersonType.Normal || parameters.Type > (int)PersonType.Midmost)
                return new ResponseResult { Result = Result.RequiredData, Note = Actions.TypeIsRequired };

            if (!parameters.IsSupplier)
                if (string.IsNullOrEmpty(parameters.SalesManId.ToString()) || parameters.SalesManId == 0)
                    return new ResponseResult()
                    {
                        Note = "sales man is required",
                        Result = Result.Failed
                    };
            var PhoneExist = await PersonQuery.GetByAsync(a => a.Phone == parameters.Phone
                         && !string.IsNullOrEmpty(parameters.Phone) && a.Id != parameters.Id
                         && ((parameters.IsSupplier) ?
                                 (parameters.AddToAnotherList ? true : a.IsSupplier == true) :
                                 (parameters.AddToAnotherList ? true : a.IsCustomer == true)));
            if (PhoneExist != null)
                return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };

            var EmailExist = await PersonQuery.GetByAsync(a => a.Email == parameters.Email
                         && !string.IsNullOrEmpty(parameters.Email) && a.Id != parameters.Id
                         && ((parameters.IsSupplier) ?
                                 (parameters.AddToAnotherList ? true : a.IsSupplier == true) :
                                 (parameters.AddToAnotherList ? true : a.IsCustomer == true)));

            if (EmailExist != null)
                return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };

            if (parameters.IsSupplier && parameters.Id == 1 && parameters.AddToAnotherList)
                return new ResponseResult() { Result = Result.CanNotBeUpdated, Note = Actions.CanNotMakeSupplierAsCustomer };

            if (!parameters.IsSupplier && parameters.Id == 2 && parameters.AddToAnotherList)
                return new ResponseResult() { Result = Result.CanNotBeUpdated, Note = Actions.CanNotMakeCustomerAsSupplier };
            if (!parameters.Branches.Any())
                return new ResponseResult() { Result = Result.CanNotBeUpdated, Note = Actions.BranchIdIsRequired };

            if (string.IsNullOrEmpty(parameters.LatinName.Trim()))
                parameters.LatinName = parameters.ArabicName.Trim();

            parameters.LatinName = parameters.LatinName.Trim();
            parameters.ArabicName = parameters.ArabicName.Trim();


            var data = await PersonQuery.GetByAsync(a => a.Id == parameters.Id);// && a.CodeT==parameters.CodeT);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            if (parameters.SalesPriceId > 0 && parameters.LessSalesPriceId == null)
                parameters.LessSalesPriceId = parameters.SalesPriceId;
            if (parameters.SalesPriceId == null && parameters.LessSalesPriceId == null)
            {
                parameters.LessSalesPriceId = (int)SalePricesList.SalePrice1;
                parameters.SalesPriceId = (int)SalePricesList.SalePrice1;
            }
            if (data.AddToAnotherList != parameters.AddToAnotherList)
            {
                if (parameters.IsSupplier)
                {
                    var custInvoices = _invoiceMasterQuery.TableNoTracking.Where(x => x.InvoiceSubTypesId == 8 && x.PersonId == data.Id).Any();
                    if (custInvoices)
                    {
                        return new ResponseResult()
                        {
                            Note = "Person is used in Sales invoices can not Remove from Customers list",
                            Result = Result.Failed
                        };
                    }
                }
                else
                {
                    var SupplierInvoices = _invoiceMasterQuery.TableNoTracking.Where(x => x.InvoiceTypeId == 5 && x.PersonId == data.Id);
                    if (SupplierInvoices.Any())
                    {
                        return new ResponseResult()
                        {
                            Note = "Person is used in Purchases invoices can not Remove from Suppliers list",
                            Result = Result.Failed
                        };
                    }
                }
            }
            var addToAnotherListBeforeMapping = data.AddToAnotherList;


            var FA_id = data.FinancialAccountId;
            var table = Mapping.Mapper.Map<UpdatePersonRequest, InvPersons>(parameters, data);
            table.FinancialAccountId = FA_id;

            if (table.Id == 1 || table.Id == 2)
                table.Status = (int)Status.Active;
            if (addToAnotherListBeforeMapping != parameters.AddToAnotherList)
            {
                if (parameters.IsSupplier)
                {
                    if (parameters.AddToAnotherList)
                        table.IsCustomer = true;
                    else
                    {

                        table.SalesManId = null;
                        table.IsCustomer = false;
                        //var codeExist = await PersonQuery.GetByAsync(a => a.Code == table.Code && a.CodeT == "S" && a.Id != table.Id);
                        //if(codeExist != null)
                        table.Code = PersonQuery.GetMaxCode(e => e.Code, a => a.IsSupplier == true && a.CodeT == "S") + 1;
                        table.CodeT = "S";

                    }
                    table.IsSupplier = true;

                }
                else
                {
                    if (parameters.AddToAnotherList)
                        table.IsSupplier = true;
                    else
                    {
                        // table.SalesManId = null;

                        table.IsSupplier = false;
                        //var codeExist = await PersonQuery.GetByAsync(a => a.Code == table.Code && a.CodeT == "C" && a.Id != table.Id);
                        //if (codeExist != null)
                        table.Code = PersonQuery.GetMaxCode(e => e.Code, a => a.IsCustomer == true && a.CodeT == "C") + 1;
                        table.CodeT = "C";


                    }
                    table.IsCustomer = true;

                }
            }
            var GLSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
            var userInfo =  await _iUserInformation.GetUserInformation();
            var linkingMethod = _GLIntegrationSettingsQuery
                   .TableNoTracking
                   .Where(c => c.GLBranchId == userInfo.CurrentbranchId && c.screenId == (parameters.IsSupplier? (int)SubFormsIds.Suppliers_Purchases:(int)SubFormsIds.Customers_Sales))
                   .FirstOrDefault().linkingMethodId;


            if(linkingMethod==1 && table.FinancialAccountId != parameters.FinancialAccountId)
                table.FinancialAccountId = parameters.FinancialAccountId;


            //if (parameters.IsSupplier)
            //{
            //    if (GLSettings.DefultAccSupplier == 1)
            //    {
            //        table.FinancialAccountId = parameters.FinancialAccountId;
            //    }
            //}
            //else
            //{
            //    if (GLSettings.DefultAccCustomer == 1)
            //    {
            //        table.FinancialAccountId = parameters.FinancialAccountId;
            //    }
            //}
            if (table.SalesManId == 0)
                table.SalesManId = null;

            //Set New Time to Personal ( Cutomer Or Suplier ) 

            table.UTime = DateTime.Now;


            var saved = await PersonCommand.UpdateAsyn(table);


            List<InvPersons_Branches> personBranchList = new List<InvPersons_Branches>();
            if (table.Id != 1 && table.Id != 2)
            {

                var personInvoices = _invoiceMasterQuery.TableNoTracking.Where(x => x.PersonId == parameters.Id).Select(x => x.BranchId).ToArray();
                foreach (var i in parameters.Branches)
                {
                    if (personInvoices.Where(c => c == i).Any())
                        continue;
                    var personBranchItem = new InvPersons_Branches();
                    personBranchItem.BranchId = i;
                    personBranchItem.PersonId = table.Id;
                    personBranchList.Add(personBranchItem);
                }
                var personBranches = _personBranchQuery.GetAll().Where(x => x.PersonId == parameters.Id && !personInvoices.Contains(x.BranchId)).Select(c => c.BranchId).ToArray();
                if (personBranches.Any())
                    await PersonBranchCommand.DeleteAsync(a => personBranches.Contains(a.BranchId) && a.PersonId == parameters.Id);

                PersonBranchCommand.AddRange(personBranchList);
                await PersonBranchCommand.SaveAsync();
            }
            //History 
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(parameters.IsSupplier ? SystemActionEnum.editSupplier : SystemActionEnum.editCustomer);

            return new ResponseResult() { Data = null, Id = table.Id, Result = table == null ? Result.Failed : Result.Success };

        }
    }
}
