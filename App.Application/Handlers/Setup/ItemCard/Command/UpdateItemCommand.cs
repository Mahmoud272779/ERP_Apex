using App.Application.Handlers.Setup.ItemCard.Validation;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Entities.Setup.Services;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using App.Domain.Models.Shared;
using App.Infrastructure.EmailService;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.Persistence.Context;
using Castle.Components.DictionaryAdapter;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Command
{

    public class UpdateItemCommand : BaseClass, IRequestHandler<UpdateItemRequest, ResponseResult>
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepository;
        private readonly IRepositoryCommand<InvStpItemCardParts> partsRepositoryCommand;
        private readonly IRepositoryCommand<InvStpItemColorSize> colorSizeRepositoryCommand;
        private readonly IRepositoryQuery<InvStpItemCardParts> partsRepositoryQuery;
        //private readonly IRepositoryCommand<InvStpItemCardParts> partRepositoryCommand;
        private readonly IRepositoryCommand<InvStpItemCardStores> storeRepositoryCommand;
        private readonly IRepositoryQuery<InvStpItemCardStores> storeRepositoryQuery;
        private readonly IRepositoryCommand<InvStpItemCardUnit> unitRepositoryCommand;
        private readonly IRepositoryQuery<InvStpItemCardUnit> UnitRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> _invSerialTransactionQuery;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardQuery;
        private readonly IBalanceBarcodeProcs _balanceBarcodeProcs;
        private readonly ClientSqlDbContext _context = null;
        private readonly IFileHandler fileHandler;

        private readonly IHistory<InvStpItemCardHistory> history;

        public UpdateItemCommand(IHttpContextAccessor httpContextAccessor,
                                IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository,
                                IHistory<InvStpItemCardHistory> history,
                                IServiceScopeFactory serviceScopeFactory,
                                IRepositoryCommand<InvStpItemCardParts> PartsRepositoryCommand,
                                IRepositoryQuery<InvStpItemCardParts> PartsRepositoryQuery,
                                IRepositoryQuery<InvStpItemCardStores> storeRepositoryQuery,
                                IRepositoryCommand<InvStpItemCardStores> storeRepositoryCommand,
                                IRepositoryCommand<InvStpItemCardUnit> unitRepositoryCommand,
                                IRepositoryQuery<InvStpItemCardUnit> unitRepositoryQuery,
                                IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                                IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                                IRepositoryQuery<InvSerialTransaction> InvSerialTransactionQuery,
                                ISystemHistoryLogsService systemHistoryLogsService,
                                IFileHandler fileHandler,
                                IRepositoryQuery<InvStpItemCardMaster> itemCardQuery,
                                IBalanceBarcodeProcs balanceBarcodeProcs,
                                ClientSqlDbContext context,
                                IRepositoryCommand<InvStpItemColorSize> colorSizeRepositoryCommand) : base(httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            itemCardRepository = ItemCardRepository;
            this.serviceScopeFactory = serviceScopeFactory;
            partsRepositoryCommand = PartsRepositoryCommand;
            partsRepositoryQuery = PartsRepositoryQuery;
            this.storeRepositoryQuery = storeRepositoryQuery;
            this.storeRepositoryCommand = storeRepositoryCommand;
            this.unitRepositoryCommand = unitRepositoryCommand;
            this.itemCardQuery = itemCardQuery;
            _balanceBarcodeProcs = balanceBarcodeProcs;
            UnitRepositoryQuery = unitRepositoryQuery;
            _invoiceDetailsQuery = InvoiceDetailsQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _invSerialTransactionQuery = InvSerialTransactionQuery;
            _systemHistoryLogsService = systemHistoryLogsService;
            _context = context;
            this.colorSizeRepositoryCommand = colorSizeRepositoryCommand;
            this.fileHandler = fileHandler;
            this.history = history;
        }




        public async Task<ResponseResult> Handle(UpdateItemRequest request, CancellationToken cancellationToken)
        {
            var itemUnits = UnitRepositoryQuery.TableNoTracking.Where(x => x.ItemId == request.Id).Select(x => x.UnitId).ToArray();
            var itemUnitsUsed = _invoiceDetailsQuery.TableNoTracking
                                        .Include(x => x.Items)
                                        .Include(x => x.Items.Units)
                                        .Where(x => x.ItemId == request.Id && itemUnits.Contains(x.UnitId ?? 0))
                                        .GroupBy(x => x.UnitId)
                                        .Select(x => x.FirstOrDefault())
                                        .ToList();
            var _itemUnits = UnitRepositoryQuery.TableNoTracking.Where(x => x.ItemId == request.Id).Where(x => itemUnitsUsed.Select(c => c.UnitId).Contains(x.UnitId));
            var isValid = await itemCardValidation.CheckRequestData(new itemCardRequestData()
            {
                ArabicName = request.ArabicName,
                LatinName = request.LatinName,
                ConversionFactor =  request.Units.FirstOrDefault()?.ConversionFactor??0,
                TypeId = request.TypeId,
                ItemCode = request.ItemCode,
                invGeneralSettingsRepositoryQuery = _invGeneralSettingsQuery,
                _balanceBarcodeProcs = _balanceBarcodeProcs,
                itemCardRepositoryQuery = itemCardQuery,
                itemUnitRepositoryQuery = UnitRepositoryQuery,
                itemSerialRepositoryQuery = _invSerialTransactionQuery,
                NationalBarcode = request.NationalBarcode,
                Units = request.Units,
                DepositeUnit = request.DepositeUnit,
                ReportUnit = request.ReportUnit,
                WithdrawUnit = request.WithdrawUnit,
                Parts = request.Parts,
                Status = request.Status,
                GroupId = request.GroupId,
                invoiceDetails = itemUnitsUsed,
                _itemUnits = _itemUnits,
                itemId = request.Id
            });
            if (isValid != null)
                return isValid;


            request.ArabicName = request.ArabicName != null ? request.ArabicName.Trim() : "";
            request.LatinName = request.LatinName != null  ? request.LatinName.Trim() : request.ArabicName.Trim();
            request.Model = request.Model != null ? request.Model.Trim() : "";
            request.Description = request.Description != null ? request.Description.Trim() : "";
            request.ItemCode = request.ItemCode != null ? request.ItemCode.Trim().ToUpper() : "";
            request.NationalBarcode = request.NationalBarcode != null ? request.NationalBarcode.Trim().ToUpper() : "";

            

            


            


            List<Task> tasks = new();
            var res = new ResponseResult();
            List<string> listOfBarcodeExists = new();


            List<InvStpItemCardMaster> NationalItemCode_Exist = new List<InvStpItemCardMaster>();
            List<InvStpItemCardMaster> ItemCodeNational_Exist = new List<InvStpItemCardMaster>();

            List<InvStpItemCardMaster> ItemCode_Exist = new List<InvStpItemCardMaster>();
            List<InvStpItemCardMaster> National_Exist = new List<InvStpItemCardMaster>();
            List<InvStpItemCardMaster> BarcodeNationalItemCode_Exist = new List<InvStpItemCardMaster>();
            List<InvStpItemCardUnit> ItemCodeBarcode_Exist = new List<InvStpItemCardUnit>();
            List<InvStpItemCardUnit> NationalBarcode_Exist = new List<InvStpItemCardUnit>();
            List<InvStpItemCardUnit> BarCode_Exist = new List<InvStpItemCardUnit>();
            // Added By Ahmed Mohsen For Color Requirement 2021-11-06
            List<InvStpItemColorSize> ColorBarCode_Exist = new List<InvStpItemColorSize>();
            List<InvSerialTransaction> BarcodeSerial_Exist = new List<InvSerialTransaction>();
            List<InvSerialTransaction> ItemCodeSerial_Exist = new List<InvSerialTransaction>();
            List<InvSerialTransaction> NationalSerial_Exist = new List<InvSerialTransaction>();

            var listOfUnits = request.Units.Where(e => !string.IsNullOrEmpty(e.Barcode)).Select(e => e.Barcode).ToList();
            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                NationalItemCode_Exist = await repo.FindAllAsync(e => (e.ItemCode == request.NationalBarcode) && !string.IsNullOrEmpty(request.NationalBarcode) && e.Id != request.Id);
            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                ItemCodeNational_Exist = await repo.FindAllAsync(e => (e.NationalBarcode == request.ItemCode) && e.Id != request.Id);
            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                National_Exist = await repo.FindAllAsync(e => (e.NationalBarcode == request.NationalBarcode) && !string.IsNullOrEmpty(request.NationalBarcode) && (e.Id != request.Id));
            }));



            if (request.TypeId != (int)ItemTypes.Note)
            {
                if (request.Units.Count == 0)
                {
                    return new ResponseResult() { Data = request, Result = Result.Failed, Note = "Units are required" };
                }
                tasks.Add(Task.Run(async () =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                    BarcodeNationalItemCode_Exist.AddRange(await repo.FindAllAsync(e => (listOfUnits.Contains(e.ItemCode) || listOfUnits.Contains(e.NationalBarcode)) && e.Id != request.Id));

                }));


                tasks.Add(Task.Run(async () =>
               {
                   using var scope = serviceScopeFactory.CreateScope();
                   var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardUnit>>();

                   // Added By Ahmed Mohsen For Color Requirement 2021-11-06
                   // badl el foreach 
                   BarCode_Exist.AddRange(await repo.FindAllAsync(e => listOfUnits.Contains(e.Barcode) && e.ItemId != request.Id));


               }));



                // Added By Ahmed Mohsen For Color Requirement 2021-11-06
                //tasks.Add(Task.Run(async () =>
                //{
                //    using var scope = serviceScopeFactory.CreateScope();
                //    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemColorSize>>();
                //    ColorBarCode_Exist.AddRange(await repo.FindAllAsync(e => request.ItemColorsSizes.Where(e => e.Barcode != null && string.IsNullOrEmpty(e.Barcode.Trim())).Select(a => a.Barcode).Contains(e.Barcode) && e.ItemId != request.ItemId));
                //}));


                tasks.Add(Task.Run(async () =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardUnit>>();
                    ItemCodeBarcode_Exist = await repo.FindAllAsync(a => (a.Barcode == request.ItemCode) && !string.IsNullOrEmpty(a.Barcode) && (a.ItemId != request.Id));

                }));

                tasks.Add(Task.Run(async () =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardUnit>>();
                    if (request.NationalBarcode != null)
                    {
                        NationalBarcode_Exist = await repo.FindAllAsync(a => (a.Barcode == request.NationalBarcode) && !string.IsNullOrEmpty(request.NationalBarcode) && (a.ItemId != request.Id));
                    }
                }));

                tasks.Add(Task.Run(async () =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();
                    BarcodeSerial_Exist.AddRange(await repo.FindAllAsync(e => listOfUnits.Contains(e.SerialNumber) && e.ItemId != request.Id));

                }));
            }


            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();

                ItemCode_Exist = await repo.FindAllAsync(e => (e.ItemCode == request.ItemCode) && (e.Id != request.Id));
            }));




            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();
                ItemCodeSerial_Exist = await repo.FindAllAsync(e => (e.SerialNumber == request.ItemCode) && !string.IsNullOrEmpty(e.SerialNumber) && (e.ItemId != request.Id));
            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();
                NationalSerial_Exist = await repo.FindAllAsync(e => (e.SerialNumber == request.NationalBarcode) && !string.IsNullOrEmpty(e.SerialNumber) && (e.ItemId != request.Id));
            }));



            await Task.WhenAll(tasks);
            //var unSavedItemNatValidation = request.ItemCode == request.NationalBarcode;

            var unSavedItemUnitValidation = new List<string>(); // request.Units.Select(s => s.Barcode).Where(e => e == request.ItemCode).ToList();
            var unSavedNatUnitValidation = new List<string>(); // request.Units.Select(s => s.Barcode).Where(e => e == request.NationalBarcode && request.NationalBarcode != null).ToList();
            var unSavedBarcodeValidation = new List<string>();
            if (request.TypeId != (int)ItemTypes.Note)
            {
                unSavedItemUnitValidation = request.Units.Select(s => s.Barcode).Where(e => e == request.ItemCode).ToList();
                unSavedNatUnitValidation = request.Units.Select(s => s.Barcode).Where(e => e == request.NationalBarcode && !string.IsNullOrEmpty(request.NationalBarcode) && request.NationalBarcode != null).ToList();

                var barcodeList = request.Units.Select(a => a.Barcode).ToList();

                unSavedBarcodeValidation.AddRange(listOfUnits.GroupBy(x => x)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList());
            }
            if (NationalItemCode_Exist.Any())
            {
                res.Result = Result.NationalExist;
                listOfBarcodeExists.Add(request.NationalBarcode);
            }
            if (ItemCodeNational_Exist.Any())
            {
                res.Result = Result.ItemCodeExist;
                listOfBarcodeExists.Add(request.ItemCode);
            }
            if (National_Exist.Any())
            {
                res.Result = Result.NationalExist;
                listOfBarcodeExists.Add(request.NationalBarcode);
            }

            if (BarCode_Exist.Any())
            {
                res.Result = Result.BarcodeExist;
                listOfBarcodeExists.Add(request.ItemCode);
            }
            // Added By Ahmed Mohsen For Color Requirement 2021-11-06
            //if (ColorBarCode_Exist.Any())
            //{
            //    res.Result = Result.ItemCodeExist;
            //    listOfBarcodeExists.AddRange(ColorBarCode_Exist.Select(e=> e.Barcode));
            //}
            if (ItemCode_Exist.Any())
            {
                res.Result = Result.ItemCodeExist;
                listOfBarcodeExists.Add(request.ItemCode);
            }

            if (BarcodeNationalItemCode_Exist.Any())
            {
                res.Result = Result.BarcodeExist;
                listOfBarcodeExists.AddRange(BarcodeNationalItemCode_Exist.Select(e => e.ItemCode));
            }

            if (ItemCodeBarcode_Exist.Any())
            {
                listOfBarcodeExists.AddRange(ItemCodeBarcode_Exist.Select(e => e.Barcode));
                res.Result = Result.ItemCodeExist;

            }

            if (NationalBarcode_Exist.Any())
            {
                listOfBarcodeExists.AddRange(NationalBarcode_Exist.Select(e => e.Barcode));
                res.Result = Result.NationalExist;

            }

            if (BarcodeSerial_Exist.Any())
            {
                res.Result = Result.BarcodeExist;
                listOfBarcodeExists.Add(request.ItemCode);
            }

            if (ItemCodeSerial_Exist.Any())
            {
                res.Result = Result.ItemCodeExist;
                listOfBarcodeExists.Add(request.ItemCode);
            }
            if (NationalSerial_Exist.Any())
            {
                res.Result = Result.NationalExist;
                listOfBarcodeExists.Add(request.ItemCode);
            }

            if (National_Exist.Any() && ItemCode_Exist.Any())
            {
                res.Result = Result.NationalItemCodeExist;
                listOfBarcodeExists.Add(request.NationalBarcode);
            }

            if (unSavedNatUnitValidation.Any() && unSavedNatUnitValidation.Count > 1)
            {
                res.Result = Result.BarcodeExist;
                listOfBarcodeExists.AddRange(unSavedNatUnitValidation);
            }
            if (unSavedItemUnitValidation.Any() && unSavedItemUnitValidation.Count > 1)
            {
                res.Result = Result.BarcodeExist;
                listOfBarcodeExists.AddRange(unSavedItemUnitValidation);
            }
            if (unSavedBarcodeValidation.Any() && unSavedBarcodeValidation.Count > 1)
            {
                res.Result = Result.BarcodeExist;
                listOfBarcodeExists.AddRange(unSavedBarcodeValidation);
            }


            listOfBarcodeExists = listOfBarcodeExists.Distinct().ToList();
            res.Data = listOfBarcodeExists;
            if (!listOfBarcodeExists.Any())
            {
                var item = new InvStpItemCardMaster();
                item = await itemCardRepository.GetByAsync(e => e.Id == request.Id, p => p.Parts, i => i.InvoicesDetails, u => u.Units, s => s.Stores);
                if (request.TypeId == (int)ItemTypes.Composite)
                {
                    var isItemExistInInvoices = _invoiceDetailsQuery.TableNoTracking.Where(x => x.ItemId == request.Id).Any();
                    if (isItemExistInInvoices)
                    {
                        var parts = new List<ItemPartVM>();
                        foreach (var part in item.Parts)
                        {
                            parts.Add(new ItemPartVM()
                            {
                                PartId = part.PartId,
                                Quantity = part.Quantity,
                                UnitId = part.UnitId
                            });
                        }
                        request.Parts = parts;
                    }

                }
            
                if (item == null)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
                if (item.ItemCode == "")
                    return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };

                Mapping.Mapper.Map(request, item, typeof(UpdateItemRequest), typeof(InvStpItemCardMaster));
                if (request.ChangeImage == true)
                {
                    item.ImagePath = fileHandler.DeleteImage(item.ImagePath);

                    var img = request.Images;
                    if (img != null)
                    {
                        item.ImagePath = fileHandler.SaveImage(img, "ItemCards", true);

                    }
                }
                else
                {
                    item.ImagePath = item.ImagePath;
                }

                if (request.Id == 1)
                {
                    item.Status = (int)Status.Active;
                }

                if (item.NationalBarcode == "")
                    item.NationalBarcode = null;
                if (request.TypeId != (int)ItemTypes.Composite)
                    item.ItemParts = null;

                if (request.TypeId == (int)ItemTypes.Note)
                {
                    item.ItemParts = null;
                    item.Units = null;
                    item.Stores = null;
                    item.WithdrawUnit = 0;
                    item.ReportUnit = 0;
                    item.DepositeUnit = 0;
                    item.VAT = 0;
                    item.Model = "";
                }


                // update willdelete = true to prevent lossing data in case of update process is faild
                #region updateWillDelete
                //List<Task> willDeleteTasks = new();
                //var itemStore =   storeRepositoryQuery.FindAll( a => a.ItemId == item.ItemId).ToList() ;

                //if (itemStore != null)
                //{
                //    itemStore.ForEach(a => a.WillDelete = true);

                //    willDeleteTasks.Add(storeRepositoryCommand.UpdateAsyn(itemStore));
                //}

                //var itemUnit =  UnitRepositoryQuery.FindAll(a => a.ItemId == item.ItemId).ToList();

                //if (itemUnit != null)
                //{
                //    itemUnit.ForEach(a => a.WillDelete = 1);
                //   willDeleteTasks.Add(unitRepositoryCommand.UpdateAsyn(itemUnit));
                //}

                //var itemparts =  partsRepositoryQuery.FindAll(a => a.ItemId == item.ItemId).ToList();

                //if (itemparts != null)
                //{
                //    itemparts.ForEach(a => a.WillDelete = true);
                //    willDeleteTasks.Add(partsRepositoryCommand.UpdateAsyn(itemparts));
                //}
                //await Task.WhenAll(willDeleteTasks);

                #endregion
                if (request.TypeId != 6)//If the item is of type note there is no need to execute the following statements
                {
                    item.Units.Select(e => { e.Barcode = string.IsNullOrEmpty(e.Barcode) ? null : e.Barcode.Trim().ToUpper(); return e; }).ToList();
                    //await storeRepositoryCommand.UnSavedDeleteAsync(e => e.ItemId == item.ItemId);
                    //await unitRepositoryCommand.UnSavedDeleteAsync(e => e.ItemId == item.ItemId);
                    //await partsRepositoryCommand.UnSavedDeleteAsync(e => e.ItemId == item.ItemId);
                    //// Added By Ahmed Mohsen For Color Requirement 2021-11-06
                    //await colorSizeRepositoryCommand.UnSavedDeleteAsync(e => e.ItemId == item.ItemId);
                }



                if (item.DefaultStoreId == 0)
                {
                    item.DefaultStoreId = null;
                }

                //Set Time
                item.UTime = DateTime.Now;

                var result = await itemCardRepository.UpdateAsyn(item);


                res.Result = result ? Result.Success : Result.Failed;
                res.Id = item.Id;
                res.Data = item;
                history.AddHistory(item.Id, item.LatinName, item.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editItemCard);

            }
            return res;
        }
    }
}
