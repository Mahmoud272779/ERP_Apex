using App.Application.Handlers.MainData.Payment_Methods.GetPaymentMethodByDate;
using App.Application.Handlers.Persons.GetPersonBalance;
using App.Application.Handlers.Persons.GetPersonsByDate;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Payment_methods
{
    public class PaymentMethodsService : BaseClass, IPaymentMethodsService
    {
        private readonly IRepositoryQuery<InvPaymentMethods> PaymentMethodsQuery;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentMethodsQuery;
        private readonly IMediator _mediator;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IRepositoryQuery<GLBank> GLBankQuery;
        private readonly IRepositoryQuery<GLSafe> GLSafeQuery;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> paymentmethodid;
        private readonly IRepositoryCommand<InvPaymentMethods> PaymentMethodsCommand;
        private readonly IHistory<InvPaymentMethodsHistory> history;

        private readonly IHttpContextAccessor httpContext;
        public PaymentMethodsService(IRepositoryQuery<InvPaymentMethods> _PaymentMethodsQuery
                                     , IRepositoryQuery<GLBank> _GLBankQuery
                                     , IRepositoryCommand<InvPaymentMethods> _PaymentMethodsCommand
                                     , IHistory<InvPaymentMethodsHistory> history
                                     , IHttpContextAccessor _httpContext
                                     ,IRepositoryQuery<GLSafe> gLSafeQuery
                                     ,IRepositoryQuery<InvoicePaymentsMethods> invoicePaymentMethodsQuery
                                     ,IMediator mediator
                                    ,IDeletedRecordsServices deletedRecords) : base(_httpContext)
        {
            PaymentMethodsQuery = _PaymentMethodsQuery;
            GLBankQuery = _GLBankQuery;
            PaymentMethodsCommand = _PaymentMethodsCommand;
            httpContext = _httpContext;
            this.history = history;
            GLSafeQuery = gLSafeQuery;
            InvoicePaymentMethodsQuery = invoicePaymentMethodsQuery;
            _mediator = mediator;
            _deletedRecords = deletedRecords;
        }

        public async Task<ResponseResult> AddPaymentMethod(PaymentMethodsRequest parameter)
        {
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;

            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var ArabicMethodExist = await PaymentMethodsQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ArabicMethodExist != null)
                return new ResponseResult() { Data = null, Id = ArabicMethodExist.PaymentMethodId, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinMethodExist = await PaymentMethodsQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (LatinMethodExist != null)

                return new ResponseResult() { Data = null, Id = LatinMethodExist.PaymentMethodId, Result = Result.Exist, Note = Actions.EnglishNameExist };
            if (parameter.safeOrBankId == 0 || parameter.safeOrBankId == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var bankExist = await GLBankQuery.GetByAsync(a => a.Id == parameter.safeOrBankId);
            if (bankExist == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotExist, Note = Actions.NotFound };

            int NextCode = PaymentMethodsQuery.GetMaxCode(e => e.Code) + 1;
            var methodData = Mapping.Mapper.Map<PaymentMethodsRequest, InvPaymentMethods>(parameter);
            methodData.Code = NextCode;
            methodData.SafeId = null;
            methodData.BankId = parameter.safeOrBankId;
            methodData.UTime = DateTime.Now;

            PaymentMethodsCommand.Add(methodData);
            history.AddHistory(methodData.PaymentMethodId, methodData.LatinName, methodData.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);

            return new ResponseResult() { Data = null, Id = methodData.PaymentMethodId, Result = Result.Success };

        }
        public async Task<ResponseResult> GetPaymentMethodsDropdown(bool isReceipts)
        {
            var data = await PaymentMethodsQuery.Get(a => new { a.PaymentMethodId, a.LatinName, a.ArabicName }, a => a.Status == 1);
            data.RemoveAll(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance);

            if (!isReceipts)
                return new ResponseResult() { Data = data, Result = Result.Success };
            var RecieptsData = data.Where(h => h.PaymentMethodId == 1 || h.PaymentMethodId == 3).ToList();

            return new ResponseResult() { Data = RecieptsData, Result = Result.Success };
        }
        public async Task<ResponseResult> GetPaymentMethodsDropdowninvoic(List<int> paymentIds, int? invoiceTypeId = 0)
        {
            var data = PaymentMethodsQuery.TableNoTracking.Where(a => a.Status == (int)Status.Active || paymentIds.Contains(a.PaymentMethodId))
               .Select(a=>new { a.PaymentMethodId,  a.LatinName,  a.ArabicName}).ToList();

           

            if ( invoiceTypeId.Value != (int)DocumentType.Purchase && invoiceTypeId.Value!= (int)DocumentType.Sales)
                data.RemoveAll(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance);
       
           return new ResponseResult() { Data = data, Result = Result.Success };


        }

        public async Task<ResponseResult> GetListOfPaymentMethods(PaymentMethodsSearch parameters)
        {

            //var data = await PaymentMethodsQuery.TableNoTracking.Include(h=>h.bank)
            //    .Include(q=>q.safe)
            //    .Include(s=>s.InvoicesPaymentsMethods).Include(d=>d.) ThenInclude(h=>h.).Where(a => ((a.Code.ToString().Contains(parameters.Name) ||
            //      string.IsNullOrEmpty(parameters.Name) || a.ArabicName.Contains(parameters.Name) ||
            //      a.LatinName.Contains(parameters.Name)) &&
            //      (parameters.Status == 0 || a.Status == parameters.Status)))


            var data = await PaymentMethodsQuery.GetAllIncludingAsync(0, 0,
                  a => (a.PaymentMethodId!=(int)PaymentMethod.PersonBalance &&
                  a.PaymentMethodId != (int)PaymentMethod.Transfer &&
                  (a.Code.ToString().Contains(parameters.Name) ||
                  string.IsNullOrEmpty(parameters.Name) || a.ArabicName.Contains(parameters.Name) ||
                  a.LatinName.Contains(parameters.Name)) &&
                  (parameters.Status == 0 || a.Status == parameters.Status)),
                  e => (string.IsNullOrEmpty(parameters.Name) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => a.Code /*(a.Code.ToString().Contains(parameters.Name)) ? 0 : 1*/))
                  , a => a.safe, w => w.bank, q => q.InvoicesPaymentsMethods);

            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };    
            //فى حاله لو احتاج امنع الحذف طرق السداد فى الفواتير المحذوفه 
            var invpayment = InvoicePaymentMethodsQuery.TableNoTracking.Where(a => data.Select(s => s.PaymentMethodId).Contains(a.PaymentMethodId) ).ToList();

            data.Where(a =>!Lists.MainPaymentMehod.Contains( a.PaymentMethodId )&& a.PaymentMethodId!=(int)PaymentMethod.PersonBalance && a.PaymentMethodId != (int)PaymentMethod.Transfer)
                 .Select(a => a.CanDelete = true).ToList();

            var FinalData = data.Select(a => new PaymentMethodResponse
            {
                ArabicName = a.ArabicName,
                Code = a.Code,
                PaymentMethodId = a.PaymentMethodId,
                LatinName = a.LatinName,
                Status = a.Status,
                safeOrBankId =(a.PaymentMethodId==(int)PaymentMethod.PersonBalance?0: (a.SafeId == null ? a.BankId : a.SafeId)),
                safeOrBankNameAr = (a.PaymentMethodId == (int)PaymentMethod.PersonBalance ? "" : (a.SafeId == null ? a.bank.ArabicName : a.safe.ArabicName)),
                safeOrBankNameEn = (a.PaymentMethodId == (int)PaymentMethod.PersonBalance ? "" : (a.SafeId == null ? a.bank.LatinName : a.safe.LatinName)),
                CanDelete = invpayment.Select(h => h.PaymentMethodId).Contains(a.PaymentMethodId) ? false : a.CanDelete,

            }).ToList();
            var finalResult = Pagenation<PaymentMethodResponse>.pagenationList(parameters.PageSize, parameters.PageNumber, FinalData);

            //Pagenation<>.pagenationList()
            var count = data.Count();
            //if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            //{
            //    data = data.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            //}
            //else
            //{
            //    return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            //}

            return new ResponseResult() { Data = finalResult, Id = null, DataCount = count, Result = Result.Success };


        }


        public async Task<ResponseResult> UpdatePaymentMethods(UpdatePaymentMethodsRequest parameters)
        {

            ResponseResult valid = await validateData(parameters);
            if (valid.Result != Result.Success)
                return valid;



            var data = await PaymentMethodsQuery.GetByAsync(a => a.PaymentMethodId == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            if ((parameters.Id >= (int)PaymentMethod.paid && parameters.Id <= (int)PaymentMethod.Cheques) || parameters.Id==(int)PaymentMethod.PersonBalance)
            {
                parameters.ArabicName = data.ArabicName;
                parameters.Code = data.Code;
                parameters.LatinName = data.LatinName;
                if(parameters.Id >0)
                   parameters.Status = (int)Status.Active;
            }
            //mapping
            var table = Mapping.Mapper.Map<UpdatePaymentMethodsRequest, InvPaymentMethods>(parameters, data);

            //check if there is bank or safe or not
            if (parameters.Id == (int)PaymentMethod.paid || parameters.Id == (int)PaymentMethod.Cheques)
            {
                data.SafeId = parameters.safeOrBankId;
                var safeExist = await GLSafeQuery.GetByAsync(a => a.Id == parameters.safeOrBankId);
                if (safeExist == null && parameters.Id!=(int)PaymentMethod.PersonBalance)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.NotExist, Note = "Invalid SafeId" };
                if (parameters.Id == (int)PaymentMethod.PersonBalance)
                    data.SafeId = null;
            }
            else
            {
                data.BankId = parameters.safeOrBankId;
                var bankExist = await GLBankQuery.GetByAsync(a => a.Id == parameters.safeOrBankId);
                if (bankExist == null && parameters.Id != (int)PaymentMethod.PersonBalance)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.NotExist, Note = "Invalid BankId" };
                if (parameters.Id == (int)PaymentMethod.PersonBalance)
                    data.BankId = null;
            }


             // Hello shoraaaa
            table.UTime = DateTime.Now;


            await PaymentMethodsCommand.UpdateAsyn(table);

            history.AddHistory(table.PaymentMethodId, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            return new ResponseResult() { Data = null, Id = data.PaymentMethodId, Result = data == null ? Result.Failed : Result.Success };

        }

        private async Task<ResponseResult> validateData(UpdatePaymentMethodsRequest parameters)
        {
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
            //بشوف لو id موجود ولا لا
            var PaymentMethodsExist = await PaymentMethodsQuery.GetByAsync(a => a.PaymentMethodId == parameters.Id);
            if (PaymentMethodsExist == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound, Note = Actions.NotFound };

            //
            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.Id!=(int)PaymentMethod.PersonBalance&&(parameters.safeOrBankId == 0 || parameters.safeOrBankId == null))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            //var bankExist = await GLBankQuery.GetByAsync(a => a.Id == parameters.BankId);
            //if (bankExist == null)
            //    return new ResponseResult() { Data = null, Id = null, Result = Result.NotExist, Note = "Invalid BankId" };

            //الحاله 
            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var ArabicMethodExist = await PaymentMethodsQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.PaymentMethodId != parameters.Id);
            if (ArabicMethodExist != null)
                return new ResponseResult() { Data = null, Id = ArabicMethodExist.PaymentMethodId, Result = Result.Exist, Note = Actions.ArabicNameExist };

            //var LatinMethodExist = await PaymentMethodsQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.PaymentMethodId != parameters.Id);
            //if (LatinMethodExist != null)
            //    return new ResponseResult() { Data = null, Id = LatinMethodExist.PaymentMethodId, Result = Result.Exist, Note = Actions.EnglishNameExist };
            return new ResponseResult() { Data = null, Result = Result.Success };
        }

        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed, Note = Actions.InvalidStatus };

            var PaymentMethod_ = PaymentMethodsQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.PaymentMethodId));
            var PaymentMethodList = PaymentMethod_.ToList();

            PaymentMethodList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if ( parameters.Id.Contains((int)PaymentMethod.paid) || parameters.Id.Contains((int)PaymentMethod.Cheques) || parameters.Id.Contains((int)PaymentMethod.net))
                PaymentMethodList.Where(q => Lists.MainPaymentMehod.Contains( q.PaymentMethodId )).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            var result = await PaymentMethodsCommand.UpdateAsyn(PaymentMethodList);

            foreach (var method in PaymentMethodList)
            {
                history.AddHistory(method.PaymentMethodId, method.LatinName, method.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }

        public async Task<ResponseResult> DeletePaymentMethods(SharedRequestDTOs.Delete parameter)
        {
            var pay = PaymentMethodsQuery.TableNoTracking.Include(a=>a.InvoicesPaymentsMethods).Where(e => parameter.Ids.Contains(e.PaymentMethodId)
                   && e.PaymentMethodId > (int)PaymentMethod.Cheques && e.CanDelete == false).ToList();

            if (pay.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted, Note = "can not delete this item" };
          
            List<int> ids = new List<int>();
            foreach (var method in pay)
            {
                if (method.InvoicesPaymentsMethods.Count() > 0)
                {
                    ids.Add(method.PaymentMethodId);
                    continue;
                }
                PaymentMethodsCommand.Remove(method);

                List<int> deletedlist = new List<int>();
                deletedlist.Add(method.PaymentMethodId);
                //Fill The DeletedRecordTable

                _deletedRecords.SetDeletedRecord(deletedlist, 9);

                history.AddHistory(method.PaymentMethodId, method.LatinName, method.ArabicName, Aliases.HistoryActions.Delete, Aliases.TemporaryRequiredData.UserName);

            }

            await PaymentMethodsCommand.SaveAsync();

            

            if (ids.Count > 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted, Note = "can not delete some  items that used in invoices " };
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }


        public async Task<ResponseResult> GetPaymentMethodHistory(int Code)
        {
            return await history.GetHistory(a => a.EntityId == Code);
        }

        public async Task<ResponseResult> GetPaymentMethodsByDate(GetPaymentMethodsByDateRequest parameter)
        {
            return await _mediator.Send(parameter);
        }

       
 
    }
}
