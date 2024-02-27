using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.OfferPrice.OfferPriceService
{
    public class TransferToSalesService : BaseClass, ITransferToSalesService
    {
        private readonly IRepositoryQuery<OfferPriceMaster> offerPriceMasterRepositoryQuery;
        private readonly IRepositoryCommand<OfferPriceMaster> offerPriceMasterRepositoryCommand;
        public TransferToSalesService(
            IHttpContextAccessor httpContextAccessor,
            IRepositoryQuery<OfferPriceMaster> offerPriceMasterRepositoryQuery,
            IRepositoryCommand<OfferPriceMaster> offerPriceMasterRepositoryCommand) : base(httpContextAccessor)
        {
            this.offerPriceMasterRepositoryQuery = offerPriceMasterRepositoryQuery;
            this.offerPriceMasterRepositoryCommand = offerPriceMasterRepositoryCommand;
        }
        public async  Task<ResponseResult> transferTosales(int id)
        {
            ResponseResult result = null;
            var status = offerPriceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == id).Select(a => a.InvoiceSubTypesId);
            if (status.Any())
            {
                if (status.First() == (int)SubType.OfferPriceDeleted)
                {
                    result = new ResponseResult()
                    {
                        Result = Result.InvoiceDeletedOrReturned,
                        ErrorMessageAr = ErrorMessagesAr.InvoiceDeleted,
                        ErrorMessageEn = ErrorMessagesEn.InvoiceDeleted
                    };
                }
                else if (status.First() == (int)SubType.OfferPriceAccridited)
                {
                    result = new ResponseResult()
                    {
                        Result = Result.InvoiceDeletedOrReturned,
                        ErrorMessageAr = ErrorMessagesAr.Invoicetransfered  ,
                        ErrorMessageEn = ErrorMessagesEn.Invoicetransfered
                    };
                }
                    
                else
                    result = new ResponseResult() { Result = Result.Success };


            }
            else
                result = new ResponseResult() { Result = Result.NoDataFound, ErrorMessageAr = ErrorMessagesAr.InvoiceNotExist
                    , ErrorMessageEn = ErrorMessagesEn.InvoiceNotExist };
            return result;
        }

        public async Task<ResponseResult> updateStatus(int id)
        {
            var data = offerPriceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == id).First();
            data.InvoiceSubTypesId = (int)SubType.OfferPriceAccridited;
         await      offerPriceMasterRepositoryCommand.UpdateAsyn(data);
           //  offerPriceMasterRepositoryCommand.SaveChanges();
            return new ResponseResult() { Result = Result.Success };
        }
    }
}
