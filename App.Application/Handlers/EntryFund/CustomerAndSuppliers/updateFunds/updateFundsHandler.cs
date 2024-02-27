using App.Application.Handlers.EntryFund.CustomerAndSuppliers.updateFunds.updateFundsGLRelation;
using App.Application.Services.Process.FundsCustomerSupplier;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.EntryFund.CustomerAndSuppliers.updateFunds
{
    public class updateFundsHandler : IRequestHandler<updateFundsRequest, ResponseResult>
    {
        private readonly iUserInformation _iUserInformation;

        private readonly IRepositoryQuery<InvFundsCustomerSupplier> _InvFundsCustomerSupplierQuery;
        private readonly IRepositoryCommand<InvFundsCustomerSupplier> _InvFundsCustomerSupplierCommand;
        private readonly IRepositoryQuery<GlReciepts> _receiptQuery;
        private readonly IReceiptsService _receiptsService;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IMediator _mediator;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;


        public updateFundsHandler(iUserInformation iUserInformation, IRepositoryCommand<InvFundsCustomerSupplier> invFundsCustomerSupplierCommand, IRepositoryQuery<InvFundsCustomerSupplier> invFundsCustomerSupplierQuery, IRepositoryQuery<GlReciepts> receiptQuery, IReceiptsService receiptsService, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery, IMediator mediator, ISystemHistoryLogsService systemHistoryLogsService)
        {
            _iUserInformation = iUserInformation;
            _InvFundsCustomerSupplierCommand = invFundsCustomerSupplierCommand;
            _InvFundsCustomerSupplierQuery = invFundsCustomerSupplierQuery;
            _receiptQuery = receiptQuery;
            _receiptsService = receiptsService;
            _invGeneralSettingsQuery = invGeneralSettingsQuery;
            _mediator = mediator;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<ResponseResult> Handle(updateFundsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var funds = _InvFundsCustomerSupplierQuery
                .TableNoTracking
                .Where(c => c.branchId == userInfo.CurrentbranchId)
                .Where(c => request.listOfPersonsFunds.Select(x => x.Id).Contains(c.PersonId))
                .ToList();

            funds.ForEach(c =>
            {
                c.Credit = request.listOfPersonsFunds.Find(x => x.Id == c.PersonId).Credit;
                c.Debit = request.listOfPersonsFunds.Find(x => x.Id == c.PersonId).Debit;
            });

            var saved = await _InvFundsCustomerSupplierCommand.UpdateAsyn(funds);
            if (saved)
            {
                DateTime dateCreation = request.date;
                var settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
                if (request.isCustomer)
                {
                    settings.Funds_Customers_Date = request.date;
                    settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
                }
                else
                {
                    settings.Funds_Supplires_Date = request.date;
                    settings = await _invGeneralSettingsQuery.GetByIdAsync(1);
                }
                var listOfupdateRecModel = new List<updateRecModel>();

                foreach (var item in funds)//parameters.listOfPersonsFunds)
                {
                    listOfupdateRecModel.Add(new updateRecModel()
                    {
                        creditor = item.Credit,
                        depitor = item.Debit,
                        personId = item.PersonId,
                        
                    });
                }
                await updateRec(listOfupdateRecModel, request.isCustomer, dateCreation,userInfo.CurrentbranchId);
                await _mediator.Send(new updateFundsGLRelationRequest
                {
                    supAndCustUpdateFunds = request.listOfPersonsFunds,
                    isCustomer = request.isCustomer,
                    date = request.isCustomer ? settings.Funds_Customers_Date.Value : settings.Funds_Supplires_Date.Value,
                    isUpdate  = true,
                    branchId = userInfo.CurrentbranchId
                });
                //await GLRelation(request.listOfPersonsFunds, request.isCustomer, request.isCustomer ?  settings.Funds_Customers_Date.Value : settings.Funds_Supplires_Date.Value, true,userInfo.CurrentbranchId);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(request.isCustomer ? SystemActionEnum.editCustomersFund : SystemActionEnum.editSuppliersFund);
            return new ResponseResult() { Data = null, Result = saved ? Result.Success : Result.Success, Note = saved ? Actions.Success : Actions.SaveFailed };
            throw new NotImplementedException();
        }

        
        public async Task updateRec(List<updateRecModel> updateRecModel, bool isCustomer, DateTime date,int branchId)
        {
            var personsRecs = _receiptQuery
                .TableNoTracking
                .Where(c=> c.BranchId == branchId)
                .Where(x => updateRecModel.Select(c => c.personId).ToArray().Contains(x.PersonId.Value) && x.ParentTypeId == (isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds));
            foreach (var item in updateRecModel)
            {
                var amount = item.creditor - item.depitor;

                if (personsRecs.Where(x => x.PersonId == item.personId).Any())
                {
                    var updated = await _receiptsService.UpdateReceipt(new Domain.UpdateRecieptsRequest()
                    {
                        Id = personsRecs.Where(x => x.PersonId == item.personId).FirstOrDefault().Id,
                        SafeID = 1,
                        Creditor = amount >= 0 ? amount : 0,
                        Debtor = amount < 0 ? (amount * -1) : 0,
                        Amount = amount >= 0 ? amount : (amount * -1),
                        Authority = isCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                        RecieptDate = date,
                        IsAccredit = false,
                        Notes = isCustomer ? "رصيد اول المدة عملاء" : "رصيد اول المدة موردين",
                        Deferre = true,
                        BenefitId = item.personId,
                        ParentType = InvoicesCode.SupplierFund + " - " + item.personId,
                        ParentId = 0,
                        RecieptTypeId = amount < 0 ? (int)DocumentType.SafeCash : (int)DocumentType.SafePayment,
                        ParentTypeId = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds,
                        PaymentMethodId = 1,
                        ReceiptOnly = true
                    }, true);
                }
                else
                {
                    var added = await _receiptsService.AddReceipt(new Domain.RecieptsRequest()
                    {
                        SafeID = 1,
                        Creditor = amount >= 0 ? amount : 0,
                        Debtor = amount < 0 ? amount : 0,
                        Amount = amount > 0 ? amount : (amount * -1),
                        Authority = isCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                        RecieptDate = date,
                        ParentId = 0,
                        IsAccredit = false,
                        Notes = isCustomer ? "رصيد اول المدة عملاء" : "رصيد اول المدة موردين",
                        Deferre = true,
                        BenefitId = item.personId,
                        RecieptTypeId = amount < 0 ? (int)DocumentType.SafeCash : (int)DocumentType.SafePayment,
                        ParentTypeId = isCustomer ? (int)DocumentType.CustomerFunds : (int)DocumentType.SuplierFunds,
                        ParentType = InvoicesCode.SupplierFund + " - " + item.personId,
                        PaymentMethodId = 1,
                        ReceiptOnly = true,
                        BranchId = branchId
                    });

                }
            }



        }
      
    }
}
