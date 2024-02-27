using App.Application.Handlers.Setup.ItemCard.Validation;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Command
{
    public class DeleteItemsCommand : BaseClass, IRequestHandler<DeleteItemsRequest, ResponseResult>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardCommandRepository;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardQueryRepository;
        private readonly IRepositoryQuery<OfferPriceDetails> _OfferPriceDetailsQuery;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<InvSerialTransaction> Serialrepository;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsrepository;
        private readonly IRepositoryCommand<DeletedRecords> _deletedRecordCommand;
        private readonly IRepositoryQuery<InvStpItemCardParts> Partsrepository;
        private readonly IRepositoryCommand<InvStpItemCardParts> PartsrepositoryCommand;

        List<int> listOfExistItems = new();
        public DeleteItemsCommand(IHttpContextAccessor httpContextAccessor, IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository
            , IRepositoryQuery<InvStpItemCardMaster> ItemCardQueryRepository,
              IRepositoryQuery<InvSerialTransaction> serialrepository,
              IRepositoryQuery<InvStpItemCardParts> partsrepository,
              IRepositoryCommand<InvStpItemCardParts> partsrepositoryCommand,
              IRepositoryQuery<InvoiceDetails> invoiceDetailsrepository,
              IRepositoryCommand<DeletedRecords> deletedRecordCommand,
              IRepositoryQuery<OfferPriceDetails> offerPriceDetailsQuery ,
              ISystemHistoryLogsService systemHistoryLogsService) : base(httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            itemCardCommandRepository = ItemCardRepository;
            itemCardQueryRepository = ItemCardQueryRepository;
            Serialrepository = serialrepository;
            InvoiceDetailsrepository = invoiceDetailsrepository;
            _deletedRecordCommand = deletedRecordCommand;
            Partsrepository = partsrepository;
            PartsrepositoryCommand = partsrepositoryCommand;
            _OfferPriceDetailsQuery = offerPriceDetailsQuery;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<ResponseResult> Handle(DeleteItemsRequest request, CancellationToken cancellationToken)
        {
            if (request.Items.Contains(1))
                return new ResponseResult()
                {
                    Note = Actions.DefultDataCanNotbeDeleted,
                    Result = Result.Failed
                };
            foreach (var item in request.Items)
            {
                var isValid = await itemCardValidation.CheckIfItemUsed(item, itemCardQueryRepository, InvoiceDetailsrepository, _OfferPriceDetailsQuery);
                if (isValid != null)
                    return isValid;
            }
            var invoiceDetails = await InvoiceDetailsrepository.FindAllAsync(e => request.Items.Contains(e.ItemId));
            var Serial = await Serialrepository.FindAllAsync(e => request.Items.Contains(e.ItemId));
            var parts = await Partsrepository.FindAllAsync(q => request.Items.Contains(q.PartId));
            
            DeleteResult deleteResult = new DeleteResult();
            if (request.Items.Contains(1))
            {
                deleteResult.DefaultListCount = 1;
            }

            if (invoiceDetails.Any())
            {
                listOfExistItems.AddRange(invoiceDetails.Select(e => e.ItemId));
                deleteResult.ExistedListCount += listOfExistItems.Count;

            }
            if (Serial.Any())
            {
                listOfExistItems.AddRange(Serial.Select(e => e.ItemId));
                deleteResult.ExistedListCount += listOfExistItems.Count;

            }
            var list = await itemCardQueryRepository.FindAllAsync(e => request.Items.Contains(e.Id) && !invoiceDetails.Select(a => a.ItemId).Contains(e.Id));
            
            var deletedListItems = list.Select(e => e.Id).ToList();
            var DeletedItems = deletedListItems.Count();
            //await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteItemCard, DeletedItems);

            if (parts.Any())
            {
                PartsrepositoryCommand.RemoveRange(parts);
                await PartsrepositoryCommand.SaveAsync();
            }


            if (list.Any())
            {
                itemCardCommandRepository.RemoveRange(list);
                await itemCardCommandRepository.SaveAsync();

                //Fill The DeletedRecordTable
                var listRecords = new List<DeletedRecords>();
                foreach (var item in list)
                {
                    var deleted = new DeletedRecords
                    {
                        Type = 5,
                        DTime = DateTime.Now,
                        RecordID = item.Id
                    };
                    listRecords.Add(deleted);
                }
                _deletedRecordCommand.AddRangeAsync(listRecords);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteItemCard, DeletedItems);

                return new ResponseResult() { Data = deletedListItems, Result = deletedListItems.Count > 0 ? Result.Success : Result.Exist };
            }
            else
            {
                return new ResponseResult() { Data = null, Result = list.Count > 0 ? Result.Success : Result.NoDataFound };
            }
            //result.Result = Result.Success;
            //return result;
        }
    }
    public class DeleteResult
    {
        public int DeletedListCount { get; set; }
        public int ExistedListCount { get; set; }
        public int DefaultListCount { get; set; } 
    }
}
