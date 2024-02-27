using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Application.Handlers.Setup.ItemCard.Validation;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper;
using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Entities.Setup.Services;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.EmailService;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Command
{
    public class AddItemCommand : BaseClass, IRequestHandler<AddItemRequest, ResponseResult>
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IRepositoryCommand<InvStpItemCardMaster> itemCardRepositoryCommand;
        private readonly IRepositoryCommand<InvStpItemCardParts> partsRepositoryCommand;
        private readonly IRepositoryCommand<InvStpItemCardUnit> UnitsRepositoryCommand;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> itemSerialRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly IFileHandler fileHandler;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IHistory<InvStpItemCardHistory> history;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IBalanceBarcodeProcs _balanceBarcodeProcs;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        public AddItemCommand(IHttpContextAccessor httpContextAccessor,
            IRepositoryCommand<InvStpItemCardMaster> ItemCardRepository,
            IHttpContextAccessor _httpContext
           , IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery,
            IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery,
            IRepositoryCommand<InvStpItemCardParts> PartsRepositoryCommand,
            IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery1,
            IRepositoryCommand<InvStpItemCardUnit> unitsRepositoryCommand,
            IRepositoryQuery<InvSerialTransaction> itemSerialRepositoryQuery,
            IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery,
            IHistory<InvStpItemCardHistory> history, IFileHandler fileHandler,
            ISystemHistoryLogsService systemHistoryLogsService,
            IHostingEnvironment hostingEnvironment,
            IBalanceBarcodeProcs balanceBarcodeProcs,
            IServiceScopeFactory serviceScopeFactory, 
            ISecurityIntegrationService securityIntegrationService) : base(httpContextAccessor)
        {
            itemCardRepositoryCommand = ItemCardRepository;
            httpContext = _httpContext;
            this.itemCardRepositoryQuery = itemCardRepositoryQuery;
            this.itemUnitRepositoryQuery = itemUnitRepositoryQuery;
            this.serviceScopeFactory = serviceScopeFactory;
            UnitsRepositoryCommand = unitsRepositoryCommand;
            this.itemSerialRepositoryQuery = itemSerialRepositoryQuery;
            this.invGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery;
            partsRepositoryCommand = PartsRepositoryCommand;
            this.fileHandler = fileHandler;
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            this._hostingEnvironment = hostingEnvironment;
            _balanceBarcodeProcs = balanceBarcodeProcs;
            _securityIntegrationService = securityIntegrationService;
        }



        public async Task<ResponseResult> Handle(AddItemRequest request, CancellationToken cancellationToken)
        {
            var security = await _securityIntegrationService.getCompanyInformation();
            if (!security.isInfinityItems)
            {
                var itemsCount = itemCardRepositoryQuery.TableNoTracking.Count();
                if(itemsCount >= security.AllowedNumberOfItems)
                {
                    return new ResponseResult()
                    {
                        Note = Actions.YouHaveTheMaxmumOfSuppliers,
                        Result = Result.MaximumLength,
                        ErrorMessageAr = "تجاوزت الحد الاقصي من عدد الاصناف",
                        ErrorMessageEn = "You Cant add a new Item because you have the maximum of items for your bunlde"
                    };
                }
            }
            var isValid = await itemCardValidation.CheckRequestData(new itemCardRequestData()
            {
                ArabicName = request.ArabicName,
                LatinName = request.LatinName,
                ConversionFactor = request.Units.FirstOrDefault()?.ConversionFactor??0,
                TypeId = request.TypeId,
                ItemCode = request.ItemCode,
                invGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery,
                _balanceBarcodeProcs = _balanceBarcodeProcs,
                itemCardRepositoryQuery = itemCardRepositoryQuery,
                itemUnitRepositoryQuery = itemUnitRepositoryQuery,
                itemSerialRepositoryQuery = itemSerialRepositoryQuery,
                NationalBarcode = request.NationalBarcode,
                Units = request.Units,
                DepositeUnit = request.DepositeUnit,
                ReportUnit = request.ReportUnit,
                WithdrawUnit = request.WithdrawUnit,
                Parts = request.Parts,
                Status = request.Status
            });
            if (isValid != null)
                return isValid;

            var res = new ResponseResult();

            request.ArabicName = request.ArabicName != null ? request.ArabicName.Trim() : "";

            request.LatinName = request.LatinName != null ? request.LatinName.Trim() : request.ArabicName;
            request.Model = request.Model != null ? request.Model.Trim() : "";
            request.Description = request.Description != null ? request.Description.Trim() : "";
            var settings = await invGeneralSettingsRepositoryQuery.GetAsync(1);
            if (settings.Other_ItemsAutoCoding)
            {
                request.ItemCode = await GetMaxItemCode();
            }
            else
            {
                request.ItemCode = request.ItemCode != null ? request.ItemCode.Trim().ToUpper() : "";
            }
            
            request.NationalBarcode = request.NationalBarcode != null ? request.NationalBarcode.Trim().ToUpper() : "";

            var listOfUnits = request.Units.Where(e => !string.IsNullOrEmpty(e.Barcode)).Select(e => e.Barcode).ToList();
            List<Task> tasks = new();

            List<string> listOfExistBarcodes = new();
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
            
            #region comments
            //foreach (var barcode in request.Units.Select(a => a.Barcode))
            //{
            //    var check = await itemUnitRepositoryQuery.GetByAsync(e => (e.Barcode == barcode) && !string.IsNullOrEmpty(barcode));
            //    if (check != null)
            //        BarCodeList.Add(check);
            //}

            //foreach (var barcode in request.Units.Select(a => a.Barcode))
            //{
            //    var check = await itemCardRepositoryQuery.GetByAsync(e => (e.ItemCode == barcode || e.NationalBarcode == barcode) && !string.IsNullOrEmpty(barcode));
            //    if (check != null)
            //        unitItemValidation.Add(check);
            //}  

            //tasks.Add(Task.Run(async () =>
            //{
            //    using var scope = serviceScopeFactory.CreateScope();
            //    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();

            //    serialValidation = await repo.FindAllAsync(e => request.Units.Select(s => s.Barcode).Contains(e.SerialNo) && !string.IsNullOrEmpty(e.SerialNo));
            //}));

            //foreach (var barcode in request.Units.Select(a => a.Barcode))
            //{
            //    var check = await itemSerialRepositoryQuery.GetByAsync(e => (e.SerialNumber == barcode) && !string.IsNullOrEmpty(barcode));
            //    if (check != null)
            //        serialValidation.Add(check);
            //}

            #endregion
           
            
            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                NationalItemCode_Exist = await repo.FindAllAsync(e => e.ItemCode == request.NationalBarcode);
            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                ItemCodeNational_Exist = await repo.FindAllAsync(e => (e.NationalBarcode == request.ItemCode));
            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();
                National_Exist = await repo.FindAllAsync(e => (e.NationalBarcode == request.NationalBarcode));
            }));

            if (request.TypeId != (int)ItemTypes.Note)
            {
                tasks.Add(Task.Run(async () =>
                          {
                              using var scope = serviceScopeFactory.CreateScope();
                              var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardUnit>>();

                              // Added By Ahmed Mohsen For Color Requirement 2021-11-06 Replace Of Foreach
                              BarCode_Exist.AddRange(await repo.FindAllAsync(e => listOfUnits.Contains(e.Barcode)));

                          }));
                // Added By Ahmed Mohsen For Color Requirement 2021-11-06
                //tasks.Add(Task.Run(async () =>
                //{
                //    using var scope = serviceScopeFactory.CreateScope();
                //    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemColorSize>>();
                //    ColorBarCode_Exist.AddRange(await repo.FindAllAsync(e => request.Units.ItemColorsSizes.Select(a => a.Barcode).Contains(e.Barcode) ));
                //}));


                tasks.Add(Task.Run(async () =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();

                    BarcodeNationalItemCode_Exist.AddRange(await repo.FindAllAsync(e => listOfUnits.Contains(e.ItemCode) || listOfUnits.Contains(e.NationalBarcode)));


                }));

                tasks.Add(Task.Run(async () =>
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();

                    BarcodeSerial_Exist.AddRange(await repo.FindAllAsync(e => listOfUnits.Contains(e.SerialNumber)));


                    //foreach (var barcode in request.Units.Select(a => a.Barcode))
                    //{
                    //    var check = await repo.GetByAsync(e => (e.SerialNumber == barcode) && !string.IsNullOrEmpty(barcode));
                    //    if (check != null)
                    //        BarcodeSerial_Exist.Add(check);
                    //}
                }));
            }


            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardMaster>>();

                ItemCode_Exist = await repo.FindAllAsync(e => (e.ItemCode == request.ItemCode));
            }));



            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardUnit>>();
                ItemCodeBarcode_Exist = await repo.FindAllAsync(a => (a.Barcode == request.ItemCode) && !string.IsNullOrEmpty(a.Barcode));

            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvStpItemCardUnit>>();
                NationalBarcode_Exist = await repo.FindAllAsync(a => (a.Barcode == request.NationalBarcode) && !string.IsNullOrEmpty(a.Barcode));

            }));



            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();
                ItemCodeSerial_Exist = await repo.FindAllAsync(e => (e.SerialNumber == request.ItemCode) && !string.IsNullOrEmpty(e.SerialNumber));
            }));

            tasks.Add(Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepositoryQuery<InvSerialTransaction>>();
                NationalSerial_Exist = await repo.FindAllAsync(e => (e.SerialNumber == request.NationalBarcode) && !string.IsNullOrEmpty(e.SerialNumber));
            }));

            await Task.WhenAll(tasks);

            if ((request.TypeId != (int)ItemTypes.Note))
            {
                if (request.Units.Count==0)
                {
                    return new ResponseResult() { Data = request, Result = Result.Failed, Note = Aliases.ItemCardMessages.UnitsAreRequired };
                }
                var unSavedItemUnitValidation = request.Units.Select(s => s.Barcode).Where(e => e == request.ItemCode).ToList();
                var unSavedNatUnitValidation = request.Units.Select(s => s.Barcode).Where(e => e == request.NationalBarcode && !string.IsNullOrEmpty(request.NationalBarcode) && request.NationalBarcode != null).ToList();
                var unSavedBarcodeValidation = new List<string>();
                if (unSavedItemUnitValidation.Any() && unSavedItemUnitValidation.Count > 1)
                {
                    res.Result = Result.BarcodeExist;
                    listOfExistBarcodes.AddRange(unSavedItemUnitValidation);
                }


                if (unSavedNatUnitValidation.Any() && unSavedNatUnitValidation.Count > 1)
                {
                    res.Result = Result.BarcodeExist;
                    listOfExistBarcodes.AddRange(unSavedNatUnitValidation);
                }

                var barcodeList = request.Units.Select(a => a.Barcode).ToList();

                unSavedBarcodeValidation.AddRange(listOfUnits.GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(y => y.Key)
                    .ToList());

                if (unSavedBarcodeValidation.Any())
                {
                    res.Result = Result.BarcodeExist;
                    listOfExistBarcodes.AddRange(unSavedBarcodeValidation);
                }
            }

            if (NationalItemCode_Exist.Any())
            {
                res.Result = Result.NationalExist;
                listOfExistBarcodes.Add(request.NationalBarcode);
            }
            if (ItemCodeNational_Exist.Any())
            {
                res.Result = Result.ItemCodeExist;
                
                listOfExistBarcodes.Add(request.ItemCode);
            }
            if (National_Exist.Any())
            {
                res.Result = Result.NationalExist;
                
                listOfExistBarcodes.Add(request.NationalBarcode);
            }

            if (BarCode_Exist.Any())
            {
                res.Result = Result.BarcodeExist;
                
                listOfExistBarcodes.Add(request.ItemCode);
            }
            // Added By Ahmed Mohsen For Color Requirement 2021-11-06
            //if (ColorBarCode_Exist.Any())
            //{
            //    res.Result = Result.ItemCodeExist;
            //    listOfExistBarcodes.AddRange(ColorBarCode_Exist.Select(e => e.Barcode));
            //}

            if (ItemCode_Exist.Any())
            {
                res.Result = Result.ItemCodeExist;
                listOfExistBarcodes.Add(request.ItemCode);
            }

            if (BarcodeNationalItemCode_Exist.Any())
            {
                res.Result = Result.BarcodeExist;
                listOfExistBarcodes.AddRange(BarcodeNationalItemCode_Exist.Select(e => e.ItemCode));
            }

            if (ItemCodeBarcode_Exist.Any())
            {
                listOfExistBarcodes.AddRange(ItemCodeBarcode_Exist.Select(e => e.Barcode));
                res.Result = Result.ItemCodeExist;

            }

            if (NationalBarcode_Exist.Any())
            {
                listOfExistBarcodes.AddRange(NationalBarcode_Exist.Select(e => e.Barcode));
                res.Result = Result.NationalExist;

            }

            if (BarcodeSerial_Exist.Any())
            {
                res.Result = Result.BarcodeExist;
                listOfExistBarcodes.Add(request.ItemCode);
            }

            if (ItemCodeSerial_Exist.Any())
            {
                res.Result = Result.ItemCodeExist;
                listOfExistBarcodes.Add(request.ItemCode);
            }
            if (NationalSerial_Exist.Any())
            {
                res.Result = Result.NationalExist;
                listOfExistBarcodes.Add(request.ItemCode);
            }

            if (National_Exist.Any() && ItemCode_Exist.Any())
            {
                res.Result = Result.NationalItemCodeExist;
                listOfExistBarcodes.Add(request.NationalBarcode);
            }

            listOfExistBarcodes = listOfExistBarcodes.Distinct().ToList();
            res.Data = listOfExistBarcodes;

            if (!listOfExistBarcodes.Any())
            {
                var item = new InvStpItemCardMaster();
                //if (item.Image != null)
                //{
                //    item.Image = Helpers.Helpers.FileToByteArray(request.Image1);
                //    item.ImageName = request.Image1.FileName;
                //}
                var img = request.Image1;
                if (img != null)
                {
                   
                     item.ImagePath = fileHandler.SaveImage(img, "ItemCards", true); //SaveImage.saveImage(img, "ItemCards", true, _hostingEnvironment);
                 
                }
                //request.VAT = request.VAT == 0 ? 15 : request.VAT;
                
                item.ItemCode = request.ItemCode.Trim();
                item.NationalBarcode = request.NationalBarcode.Trim();

                if (string.IsNullOrEmpty(item.ItemCode))
                {
                    return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };

                }
                Mapping.Mapper.Map(request, item, typeof(AddItemRequest), typeof(InvStpItemCardMaster));
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

                if (item.DefaultStoreId==0)
                {
                    item.DefaultStoreId = null;
                }

                //Set Time
                item.UTime = DateTime.Now; 
                
                var result = await itemCardRepositoryCommand.AddAsync(item);
                

                // To Check Space in Barcode
                if (request.Units != null)
                {
                    foreach (var unit in request.Units)
                    {
                        var unitTable = await itemUnitRepositoryQuery.GetByAsync(q => q.ItemId == item.Id && q.UnitId == unit.UnitId);
                        if (unitTable.Barcode != null)
                            unitTable.Barcode = unit.Barcode.Trim().ToUpper();
                        if (unitTable.Barcode == "")
                        {
                            unitTable.Barcode = null;
                        }
                        await UnitsRepositoryCommand.UpdateAsyn(unitTable);
                    }
                }

                res.Result = result ? Result.Success : Result.Failed;
                res.Id = item.Id;
                res.Data = item;

                history.AddHistory(item.Id, item.LatinName, item.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addItemCard);
            }
            return res;
        }
        public async Task<string> GetMaxItemCode()
        {
            var list = new List<int>();
            var barcodes = await itemUnitRepositoryQuery.Get(x=> new {x.UnitId,x.Barcode});
            list.Add(barcodes.Count());
            var itemSerials = await itemSerialRepositoryQuery.Get(x => new {x.Id, x.SerialNumber});
            list.Add(itemSerials.Count());
            var codes = await itemCardRepositoryQuery.Get(e => new { e.ItemCode ,e.NationalBarcode});
            list.Add(itemSerials.Count());

            List<long> intCodes = codes
                .Select(s => long.TryParse(s.ItemCode, out long n) ? n : 0)
                .ToList();
            var res = (intCodes.Max(e => e) + 1).ToString();
            for (int i = 0; i < list.Max(); i++)
            {
                bool endLoop = false;
                if(barcodes.Where(x => x.Barcode == res).Any())
                {
                    res = (int.Parse(res) + 1).ToString();
                    endLoop = false;
                }
                else
                    endLoop = true;
                if (itemSerials.Where(x => x.SerialNumber == res).Any())
                {
                    res = (int.Parse(res) + 1).ToString();
                    endLoop = false;
                }
                else
                    endLoop = true;

                if (codes.Where(x => x.NationalBarcode == res).Any())
                {
                    res = (int.Parse(res) + 1).ToString();
                    endLoop = false;
                }
                else
                    endLoop = true;
                if (codes.Where(x => x.ItemCode == res).Any())
                {
                    res = (int.Parse(res) + 1).ToString();
                    endLoop = false;
                }
                else
                    endLoop = true;

                if (endLoop)
                    break;
            }

            return res;

        }
    }
}
