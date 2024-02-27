using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Printing;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Barcode;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.EmailService;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.BarcodeEnums;
using static App.Domain.Enums.Enums;
using App.Domain.Models.Response.Barcode;
using App.Application.Handlers.Barcode.GetBarcodeItemsFromInvoices;
using MediatR;
using App.Application.Handlers.Barcode.GetBarcodeItemsFromItemCard;

namespace App.Application.Services.Process.BarCode
{
    public class BarCodeService : BaseClass, IBarCodeService
    {
        private readonly IRepositoryQuery<InvBarcodeItems> InvBarcodeItemsRepositoryQuery;
        private readonly IRepositoryCommand<InvBarcodeItems> InvBarcodeItemsRepositoryCommand;

        private readonly IRepositoryCommand<InvBarcodeTemplate> InvBarcodeTemplateRepositoryCommand;
        private readonly IRepositoryQuery<InvBarcodeTemplate> InvBarcodeTemplateRepositoryQuery;

        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<InvStpItemCardMaster> ItemCardQuery;
        private readonly IRepositoryQuery<InvCompanyData> companyQuery;
        private readonly IRepositoryQuery<InvCategories> categoryQuery;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHistory<InvBarcodeHistory> history;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IMediator _mediator;

        public BarCodeService(IRepositoryQuery<InvBarcodeItems> _InvBarcodeItemsRepositoryQuery,
                                   IRepositoryCommand<InvBarcodeItems> _InvBarcodeItemsRepositoryCommand,
                                   IRepositoryCommand<InvBarcodeTemplate> _InvBarcodeTemplateRepositoryCommand,
                                   IRepositoryQuery<InvBarcodeTemplate> _InvBarcodeTemplateyRepositoryQuery,
                                   IRepositoryQuery<InvStpItemCardMaster> _ItemCardQuery,
                                   IHostingEnvironment hostingEnvironment,
                                   IRepositoryQuery<InvCategories> _categoryQuery,
                                   IRepositoryQuery<InvCompanyData> _companyQuery, IRepositoryQuery<InvStpUnits> unitsQuery,
                                   IHttpContextAccessor _httpContext,
                                   IHistory<InvBarcodeHistory> history,
                                   IGeneralPrint iGeneralPrint,
                                   IMediator mediator
                                    ) : base(_httpContext)
        {
            InvBarcodeItemsRepositoryQuery = _InvBarcodeItemsRepositoryQuery;
            InvBarcodeItemsRepositoryCommand = _InvBarcodeItemsRepositoryCommand;
            InvBarcodeTemplateRepositoryCommand = _InvBarcodeTemplateRepositoryCommand;
            InvBarcodeTemplateRepositoryQuery = _InvBarcodeTemplateyRepositoryQuery;
            ItemCardQuery = _ItemCardQuery;
            companyQuery = _companyQuery;
            categoryQuery = _categoryQuery;
            _hostingEnvironment = hostingEnvironment;
            httpContext = _httpContext;
            this.history = history;
            _iGeneralPrint = iGeneralPrint;
            _mediator = mediator;
        }
        public async Task<ResponseResult> AddBarCode(AddBarCodeRequest parameter)
        {
            var barcodes = InvBarcodeTemplateRepositoryQuery.FindAll(q => q.LatinName == parameter.LatinName || q.ArabicName == parameter.ArabicName);
            if (barcodes.Count() > 0)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.Exist };

            }
            if (parameter.ArabicName == null || string.IsNullOrEmpty(parameter.ArabicName.Trim()) || parameter.addBarCodeItemsRequests.Count == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData };

            int NextCode = 1;
            if (InvBarcodeTemplateRepositoryQuery.FindAll(q => q.BarcodeId > 0).Count() != 0)
                NextCode = InvBarcodeTemplateRepositoryQuery.GetMaxCode(e => e.Code) + 1;
            var barCode = new InvBarcodeTemplate();
            barCode.ArabicName = parameter.ArabicName.Trim();

            if (parameter.LatinName == null || string.IsNullOrEmpty(parameter.LatinName.Trim()))
                barCode.LatinName = parameter.ArabicName.Trim();
            else
                barCode.LatinName = parameter.LatinName.Trim();

            //barCode.BranchId = parameter.BranchId;
            barCode.Code = NextCode;
            barCode.CanDelete = true;
            barCode.IsDefault = false;

            barCode.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
            InvBarcodeTemplateRepositoryCommand.Add(barCode);
            var BarcodeItemList = new List<InvBarcodeItems>();
            foreach (var item in parameter.addBarCodeItemsRequests)
            {
                var barCodeItems = new InvBarcodeItems();

                barCodeItems.FontSize = item.FontSize;
                barCodeItems.FontType = item.FontType;
                barCodeItems.BarcodeId = barCode.BarcodeId;
                barCodeItems.BeginSplitter = item.BeginSplitter;
                barCodeItems.EndSplitter = item.EndSplitter;
                barCodeItems.Bold = item.Bold;
                barCodeItems.Direction = item.Direction;
                barCodeItems.Height = item.Height;
                barCodeItems.Width = item.Width;
                barCodeItems.AlignX = item.AlignX;
                barCodeItems.AlignY = item.AlignY;
                barCodeItems.Dock = item.Dock;//يمين وشمال ووسط
                barCodeItems.Italic = item.Italic;
                barCodeItems.PositionX = item.PositionX;
                barCodeItems.PositionY = item.PositionY;
                barCodeItems.UnderLine = item.UnderLine;
                barCodeItems.BarcodeItemType = item.BarcodeItemType;
                barCodeItems.BarcodeType = item.BarcodeType;
                barCodeItems.TextType = item.TextType;
                barCodeItems.TextContent = item.TextContent;
                barCodeItems.VariableContent = item.VariableContent;
                barCodeItems.X = item.X;
                barCodeItems.Y = item.Y;
                //if (item.Image != null) 
                //{ 
                //    barCodeItems.Image = Convert.FromBase64String(item.Image);
                //}
                var img = item.Image;
                if (img != null)
                {
                    var path = _hostingEnvironment.WebRootPath;
                    string filePath = Path.Combine("Barcode\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + img.FileName.Replace(" ", ""));
                    string actulePath = Path.Combine(path, filePath);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        item.Image.CopyTo(ms);
                        Image imgResized = Helpers.Helpers.Resize(Image.FromStream(ms), 250, 250);
                        imgResized.Save(actulePath);
                        barCodeItems.ImagePath = Constants.LocalServer + filePath;
                    }
                }
                barCodeItems.IsLogo = false;
                if (item.BarcodeItemType == (int)BarcodeItemType.Logo)
                    barCodeItems.IsLogo = true;
                if (barCodeItems.BarcodeItemType == (int)BarcodeItemType.Barcode)
                {
                    barCodeItems.TextType = (int)TextType.Variable;
                    barCodeItems.TextContent = "";
                    barCodeItems.VariableContent = (int)VariableContent.ItemCode;
                }
                await InvBarcodeItemsRepositoryCommand.AddAsync(barCodeItems);
            }

            history.AddHistory(barCode.BarcodeId, barCode.LatinName, barCode.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }

        public async Task<ResponseResult> FillItemCardBarcode(string? itemCode, int? unitId, int? itemId, int? itemType, int? categoryId)
        {
            #region commOld
            //var ItemData = await ItemUnitQuery.GetAllIncludingAsync(0, 0, a => (((itemCode != null ? a.Item.ItemCode == itemCode
            //                                  || a.Barcode == itemCode : 1 == 1))
            //                                  && (itemId != null ? a.ItemId == itemId : 1 == 1)
            //                                  && (unitId != null ? a.UnitId == unitId : 1 == 1)
            //                                  && (itemType != null ? a.Item.TypeId == itemType : 1 == 1)
            //                                  && (categoryId != null ? a.Item.GroupId == categoryId : 1 == 1)
            //                                  && (a.Item.Active == (int)Status.Active)), a => a.Item, a => a.Unit);
            #endregion

            var ItemData = ItemCardQuery.TableNoTracking
            .Include(a => a.Units.Where(x => (itemCode != null ? x.Barcode.Contains(itemCode) : 1 == 1) && (unitId != null ? x.UnitId == unitId : 1 == 1)))
            .ThenInclude(a => a.Unit)
                                           .Where(a => (((itemCode != null ? a.ItemCode == itemCode ||
                                                a.Units.Select(x => x.Barcode).Contains(itemCode) : 1 == 1))
                                          && (itemId != null ? a.Id == itemId : 1 == 1)
                                          && (itemType != null ? a.TypeId == itemType : 1 == 1)
                                          && (categoryId != null ? a.GroupId == categoryId : 1 == 1)
                                          && (a.Status == (int)Status.Active)));


            #region commOld
            //var ItemData = ItemCardQuery.TableNoTracking.Include(a => a.Units).Where(a => (itemCode != null ? a.ItemCode == itemCode
            //                                   || a.Units.Select(x => x.Barcode).Contains(itemCode) : 1 == 1) && (a.Active == (int)Status.Active));
            //if (itemId != null)
            //      ItemData.Where(a=>  a.ItemId == itemId);
            //if (unitId != null)
            //    ItemData.Where(a => a.Units.Select(a=>a.UnitId).Contains(unitId.Value) );
            //if(itemType != null)
            //    ItemData.Where(a => a.TypeId == itemType);
            //if(categoryId != null)
            //    ItemData.Where(a => a.GroupId == categoryId);
            #endregion



            if (ItemData.ToList().Count > 0)
            {
                var resultList = new List<ItemCardDataOfBarcodeResponse>();

                var CompanyData = await companyQuery.GetByIdAsync(1);
                foreach (var item in ItemData)
                {

                    var result = new ItemCardDataOfBarcodeResponse();
                    result.ItemId = item.Id;
                    result.ItemCode = item.ItemCode;
                    result.ItemNameAr = item.ArabicName;
                    result.ItemNameEn = item.LatinName;
                    foreach (var Unit in item.Units)
                    {
                        var unitData = new unitsData();
                        unitData.unitId = Unit.UnitId;
                        unitData.ArabicName = Unit.Unit.ArabicName;
                        unitData.LatinName = Unit.Unit.LatinName;
                        unitData.Price = Unit.SalePrice1;
                        unitData.Barcode = Unit.Barcode;

                        result.unitsList.Add(unitData);
                    }
                    result.ItemType = item.TypeId;
                    result.ExpireDate = DateTime.Now;
                    result.Vat = item.VAT;
                    result.CategoryId = item.GroupId;
                    var categoryData = await categoryQuery.GetByIdAsync(result.CategoryId);
                    result.CategoryNameAr = categoryData.ArabicName;
                    result.CategoryNameEn = categoryData.ArabicName;
                    result.CompanyNameAr = CompanyData.ArabicName;
                    result.CompanyNameEn = CompanyData.LatinName;
                    result.Mobile = CompanyData.Phone1;
                    result.Phone = CompanyData.Phone2;
                    result.Website = CompanyData.Website;
                    resultList.Add(result);

                }
                return new ResponseResult() { Data = resultList, DataCount = ItemData.Count(), Id = null, Result = Result.Success };
            }

            return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

        }
        public async Task<ResponseResult> updateBarCode(UpdateBarCodeRequest parameter)
        {
            if (parameter.BarCodeId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var barcodes = InvBarcodeTemplateRepositoryQuery.FindAll(q => (q.LatinName == parameter.LatinName || q.ArabicName == parameter.ArabicName) && q.BarcodeId != parameter.BarCodeId);
            if (barcodes.Count() > 0)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.Exist };

            }
            if (parameter.ArabicName == null || string.IsNullOrEmpty(parameter.ArabicName.Trim()) || parameter.updateBarCodeItemsRequests.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData };

            var barCode = await InvBarcodeTemplateRepositoryQuery.GetByAsync(q => q.BarcodeId == parameter.BarCodeId);
            barCode.ArabicName = parameter.ArabicName.Trim();
            if (parameter.LatinName == null || string.IsNullOrEmpty(parameter.LatinName.Trim()))
                barCode.LatinName = parameter.ArabicName.Trim();
            else
                barCode.LatinName = parameter.LatinName.Trim();


            // barCode.BranchId = parameter.BranchId;
            barCode.Code = barCode.Code;
            barCode.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
            await InvBarcodeTemplateRepositoryCommand.UpdateAsyn(barCode);

            var barCodeItemsList = InvBarcodeItemsRepositoryQuery.FindAll(q => q.BarcodeId == barCode.BarcodeId);
            InvBarcodeItemsRepositoryCommand.RemoveRange(barCodeItemsList);
            await InvBarcodeItemsRepositoryCommand.SaveAsync();

            var BarCodeItemList = new List<InvBarcodeItems>();
            foreach (var item in parameter.updateBarCodeItemsRequests)
            {
                var barCodeItems = new InvBarcodeItems();
                barCodeItems.BarcodeId = barCode.BarcodeId;
                barCodeItems.FontSize = item.FontSize;
                barCodeItems.FontType = item.FontType;
                barCodeItems.BarcodeId = barCode.BarcodeId;
                barCodeItems.BeginSplitter = item.BeginSplitter;
                barCodeItems.EndSplitter = item.EndSplitter;
                barCodeItems.Bold = item.Bold;
                barCodeItems.Direction = item.Direction;
                barCodeItems.Height = item.Height;
                barCodeItems.Width = item.Width;
                barCodeItems.AlignX = item.AlignX;
                barCodeItems.AlignY = item.AlignY;

                barCodeItems.Dock = item.Dock;//يمين وشمال ووسط
                barCodeItems.Italic = item.Italic;
                barCodeItems.PositionX = item.PositionX;
                barCodeItems.PositionY = item.PositionY;
                barCodeItems.UnderLine = item.UnderLine;
                barCodeItems.BarcodeItemType = item.BarcodeItemType;
                barCodeItems.BarcodeType = item.BarcodeType;
                barCodeItems.TextType = item.TextType;
                barCodeItems.TextContent = item.TextContent;
                barCodeItems.VariableContent = item.VariableContent;
                barCodeItems.X = item.X;
                barCodeItems.Y = item.Y;
                //if (item.Image != null)
                //{
                //    barCodeItems.Image = Convert.FromBase64String(item.Image);
                //}
                if (item.ChangeImage == true)
                {
                    if (barCodeItems.ImagePath != null)
                    {
                        var file = barCodeItems.ImagePath.Replace(Constants.LocalServer, "");
                        string webRootPath = _hostingEnvironment.WebRootPath;
                        var fileName = "/" + file;
                        var hh = webRootPath;
                        var fullPath = webRootPath + fileName;

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                            barCodeItems.ImagePath = null;
                        }

                    }
                    var img = item.Image;
                    if (img != null)
                    {
                        var path = _hostingEnvironment.WebRootPath;
                        string filePath = Path.Combine("Categories\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + img.FileName.Replace(" ", ""));
                        string actulePath = Path.Combine(path, filePath);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            item.Image.CopyTo(ms);
                            Image imgResized = Helpers.Helpers.Resize(Image.FromStream(ms), 250, 250);
                            imgResized.Save(actulePath);
                            barCodeItems.ImagePath = Constants.LocalServer + filePath;
                        }
                    }
                }
                else
                {
                    barCodeItems.ImagePath = barCodeItems.ImagePath;
                }
                barCodeItems.IsLogo = false;
                if (item.BarcodeItemType == (int)BarcodeItemType.Logo)
                    barCodeItems.IsLogo = true;


                if (barCodeItems.BarcodeItemType == (int)BarcodeItemType.Barcode)
                {
                    barCodeItems.TextType = (int)TextType.Variable;
                    barCodeItems.TextContent = "";
                    barCodeItems.VariableContent = (int)VariableContent.ItemCode;
                }
                InvBarcodeItemsRepositoryCommand.Add(barCodeItems);
                //BarCodeItemList.Add(barCodeItems);
            }
            //InvBarcodeItemsRepositoryCommand.AddRange(barCodeItemsList);
            //await InvBarcodeItemsRepositoryCommand.SaveAsync();

            history.AddHistory(barCode.BarcodeId, barCode.LatinName, barCode.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
        public async Task<ResponseResult> GetAllBarCode(BarcodeSearch parameter)
        {
            var list = new List<BarCodeDto>();
            //var barCoddes = InvBarcodeTemplateRepositoryQuery.GetAllIncludingAsync(parameter.PageNumber,parameter.PageSize, a => ((a.Code.ToString().Contains(parameter.Name) ||
            //     string.IsNullOrEmpty(parameter.Name) || a.ArabicName.Contains(parameter.Name) ||
            //     a.LatinName.Contains(parameter.Name))) ,
            //     e => (parameter.Name == "" ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(parameter.Name)) ? 0 : 1)), a => a.BarcodeItems);

            var res = InvBarcodeTemplateRepositoryQuery.TableNoTracking.Include(e => e.BarcodeItems)
                .Where(a => (a.Code.ToString().Contains(parameter.Name) ||
                 string.IsNullOrEmpty(parameter.Name) || a.ArabicName.Contains(parameter.Name) ||
                 a.LatinName.Contains(parameter.Name))).ToList();

            res = (parameter.Name == "" || parameter.Name == null ? res.OrderByDescending(q => q.Code) : res.OrderBy(a => (a.Code.ToString().Contains(parameter.Name)) ? 0 : 1)).ToList();

            var result = res.ToList();
            var count = result.Count();


            if (parameter.PageSize > 0 && parameter.PageNumber > 0)
            {
                result = result.Skip((parameter.PageNumber - 1) * parameter.PageSize).Take(parameter.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }

            foreach (var item in result)
            {
                var container = item.BarcodeItems.Where(a => a.BarcodeItemType == (int)BarcodeItemType.Container);
                var barCodeDto = new BarCodeDto();
                barCodeDto.ArabicName = item.ArabicName;
                barCodeDto.BarCodeId = item.BarcodeId;
                barCodeDto.Code = item.Code;
                barCodeDto.CanDelete = item.CanDelete;
                barCodeDto.IsDefault = item.IsDefault;
                barCodeDto.LatinName = item.LatinName;
                if (container.Count() > 0)
                {
                    var width = container.Select(a => a.Width).First();
                    var height = container.Select(a => a.Height).First();
                    barCodeDto.Width = width;
                    barCodeDto.Height = height;
                }


                list.Add(barCodeDto);
                //}
            }
            return new ResponseResult() { Data = list, DataCount = count, Id = null, Result = Result.Success };

        }
        public async Task<ResponseResult> GetBarCodeById(int BarCodeId)
        {
            var barCoddes = await InvBarcodeTemplateRepositoryQuery.GetByAsync(q => q.BarcodeId == BarCodeId);
            var list = new List<BarCodeByIdDto>();

            var barCodeDto = new BarCodeByIdDto()
            {
                ArabicName = barCoddes.ArabicName,
                BarCodeId = barCoddes.BarcodeId,
                Code = barCoddes.Code,
                CanDelete = barCoddes.CanDelete,
                IsDefault = barCoddes.IsDefault,
                LatinName = barCoddes.LatinName,
            };
            var barCodeItems = InvBarcodeItemsRepositoryQuery.FindAll(q => q.BarcodeId == barCoddes.BarcodeId);
            foreach (var barItem in barCodeItems)
            {
                var barCodeitem = new BarCodeItemsDto()
                {
                    AlignX = barItem.AlignX,
                    AlignY = barItem.AlignY,
                    BarcodeId = barItem.BarcodeId,
                    BarcodeItemType = barItem.BarcodeItemType,
                    BarcodeType = barItem.BarcodeType,
                    BeginSplitter = barItem.BeginSplitter,
                    Bold = barItem.Bold,
                    Direction = barItem.Direction,
                    Dock = barItem.Dock,
                    EndSplitter = barItem.EndSplitter,
                    FontSize = barItem.FontSize,
                    FontType = barItem.FontType,
                    Height = barItem.Height,
                    Image = barItem.Image,
                    ImageName = barItem.ImageName,
                    IsLogo = barItem.IsLogo,
                    Italic = barItem.Italic,
                    PositionX = barItem.PositionX,
                    PositionY = barItem.PositionY,
                    TextContent = barItem.TextContent,
                    TextType = barItem.TextType,
                    UnderLine = barItem.UnderLine,
                    VariableContent = barItem.VariableContent,
                    Width = barItem.Width,
                    ImagePath = barItem.ImagePath,
                    X = barItem.X,
                    Y = barItem.Y,

                };
                if (barItem.IsLogo)
                {
                    // get logo of company
                    var companyLogo = await companyQuery.GetByAsync(q => q.Id > 0);
                    //barCodeitem.Image = companyLogo.Image;
                    // barCodeitem.ImageName = companyLogo.imageFile;
                }

                if (barItem.BarcodeItemType == (int)BarcodeItemType.Image)
                {
                    barCodeitem.Image = barItem.Image;
                    barCodeitem.ImageName = barItem.ImageName;
                }
                barCodeDto.barCodeItemsDtos.Add(barCodeitem);
            }

            return new ResponseResult() { Data = barCodeDto, Id = null, Result = Result.Success };

        }
        public async Task<ResponseResult> DeleteBarCode(SharedRequestDTOs.Delete parameter)
        {
            var barcode = InvBarcodeTemplateRepositoryQuery.FindAll(q => parameter.Ids.Contains(q.BarcodeId) && q.IsDefault == false);
            var barCodeCanDelete = InvBarcodeTemplateRepositoryQuery.FindAll(q => parameter.Ids.Contains(q.BarcodeId) && q.IsDefault == true).ToList();
            if (barcode.Count() > 0)
            {
                var barCodeDeatails = InvBarcodeItemsRepositoryQuery.FindAll(q => parameter.Ids.Contains(q.BarcodeId));
                InvBarcodeItemsRepositoryCommand.RemoveRange(barCodeDeatails);
                await InvBarcodeItemsRepositoryCommand.SaveAsync();
                InvBarcodeTemplateRepositoryCommand.RemoveRange(barcode);
                await InvBarcodeTemplateRepositoryCommand.SaveAsync();
                return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
            }
            else if (barCodeCanDelete.Count() > 0)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted };
            }
            else
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }

        }
        /* public async void HistoryBarCode( string browserName, string lastTransactionAction, string addTransactionUser, int BarcodeId, string ArabicName, string LatinName,
             bool IsDefault)
         {
             var history = new InvBarcodeHistory()
             {
                 LastDate = DateTime.Now,
                 LastAction = lastTransactionAction,
                 LastTransactionAction = lastTransactionAction,
                 AddTransactionUser = addTransactionUser,
                 BrowserName = browserName,
                 BarcodeId = BarcodeId,
                 ArabicName = ArabicName,
                 LatinName = LatinName,
                 IsDefault = IsDefault
             };
             InvBarcodeHistoryRepositoryCommand.Add(history);
         }*/
        public async Task<ResponseResult> GetAllBarCodeHistory(int BarCodeId)
        {
            return await history.GetHistory(a => a.EntityId == BarCodeId);
        }
        public async Task<ResponseResult> UpdateDefaultBarcode(DefaultBarCodeRequest barcodeId)
        {
            if (barcodeId.barcodeId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            var barcodes = await InvBarcodeTemplateRepositoryQuery.GetByAsync(q => q.IsDefault == true);
            barcodes.IsDefault = false;
            barcodes.CanDelete = true;
            await InvBarcodeTemplateRepositoryCommand.UpdateAsyn(barcodes);

            var data = await InvBarcodeTemplateRepositoryQuery.GetByIdAsync(barcodeId.barcodeId);

            data.IsDefault = true;
            data.CanDelete = false;
            await InvBarcodeTemplateRepositoryCommand.UpdateAsyn(data);
            string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
            history.AddHistory(data.BarcodeId, data.LatinName, data.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }

        public async Task<ResponseResult> GetBarcodeDropDown()
        {

            var BarcodeList = InvBarcodeTemplateRepositoryQuery.TableNoTracking.Select(a => new { a.BarcodeId, a.Code, a.ArabicName, a.LatinName });
            return new ResponseResult() { Data = BarcodeList, Id = null, Result = BarcodeList.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> InstalledPrinters()
        {

            var list = new List<Printers>();
            //var printerQuery = new ManagementObjectSearcher("SELECT * from Win32_Printer");
            foreach (var printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                // ClientPrintJob cpj = new ClientPrintJob();
                //cpj.ClientPrinter = new InstalledPrinter("");
                //   cpj. = new InstalledPrinter("MyLocalPrinter");
                var pri = new Printers();
                string printName = new PrintDocument().PrinterSettings.PrinterName.Trim();
                string BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                //  System.Management.ManagementObjectCollection printers = new System.Management.ManagementClass("Win32_Printer ").GetInstances();
                //var name = printer.GetPropertyValue("Name");
                //var status = printer.GetPropertyValue("Status");
                //var isDefault = printer.GetPropertyValue("Default");
                //var isNetworkPrinter = printer.GetPropertyValue("Network");
                pri.Name = printName.ToString();

                list.Add(pri);
            }
            return new ResponseResult() { Data = list, Id = null, Result = Result.Success };
        }

        public async Task<WebReport> BarcodeReport(PrintItemsBarcodeRequest parameter)
        {
            var data = new List<PrintingResponseDTO>();
            if (parameter.isInvoice)
            {
                data = await _mediator.Send(new GetBarcodeItemsFromInvoicesRequest
                {
                    DesignId = parameter.DesignId,
                    isInvoice = parameter.isInvoice,
                    PrintItemsBarcodeRequestDetalies = parameter.PrintItemsBarcodeRequestDetalies
                });


            }
            else
            {
                data = await _mediator.Send(new getBarcodeItemsFromItemCardRequest
                {
                    DesignId = parameter.DesignId,
                    isInvoice = parameter.isInvoice,
                    PrintItemsBarcodeRequestDetalies = parameter.PrintItemsBarcodeRequestDetalies
                });
            }

            var tablesNames = new TablesNames()
            {
                FirstListName = "itemsData",
            };


            var report = await _iGeneralPrint.PrintReport<object, PrintingResponseDTO, object>(null, data, null, tablesNames, null
             , parameter.DesignId, exportType.Print, true,0, 0, true);
            return report;
        }
    }
}


