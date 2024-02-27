
using App.Application.Handlers.Invoices.PriceLists;
using App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery;
using App.Application.Services.Process.StoreServices.Invoices.ItemsCard;
using App.Application.Services.Process.StoreServices.Invoices.POS;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Domain.Models;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Response.Store;
using MediatR;
using Org.BouncyCastle.Math;
using System.Linq;
using static App.Domain.POSTouchResponse;

namespace App.Application
{
    public class POSTouchService: IPOSTouchService
    {
        
        private readonly IRepositoryQuery<InvCategories> categoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemQuery;
        private readonly IRepositoryQuery<POSTouchSettings> posTouchSettingsQuery;
        private readonly IRepositoryCommand<POSTouchSettings> posTouchSettingsCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvPersonLastPrice> personLastPriveQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> settings;
        private readonly IMediator _mediator;
        private readonly IPosService posService;

        public POSTouchService(IRepositoryQuery<InvCategories> categoryQuery,
                               IRepositoryQuery<POSTouchSettings> _posTouchSettingsQuery,
                               IRepositoryCommand<POSTouchSettings> _posTouchSettingsCommand,
                               iUserInformation iUserInformation,
                               IRepositoryQuery<InvStpItemCardMaster> itemQuery,
                               IRepositoryQuery<InvPersonLastPrice> personLastPriveQuery,
                               IRepositoryQuery<InvGeneralSettings> settings,
                               IMediator mediator,
                               IPosService posService,
                               IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            this.categoryQuery = categoryQuery;
            this.itemQuery = itemQuery;
            posTouchSettingsQuery = _posTouchSettingsQuery;
            posTouchSettingsCommand = _posTouchSettingsCommand;
            _iUserInformation = iUserInformation;
            this.personLastPriveQuery = personLastPriveQuery;
            this.settings = settings;
            _mediator = mediator;
            this.posService = posService;
            this.itemUnitsQuery = itemUnitsQuery;
        }
        public async Task<ResponseResult> getCategoriesOfPOS()
        {
            var categories = categoryQuery.TableNoTracking
                .Where(a => a.Status == (int)Status.Active && a.UsedInSales == (int)UsedInSales.Used).OrderBy(a => a.ArabicName).ToList();//&& a.Items.Where(e => e.UsedInSales).Select(e => e.GroupId).Contains(a.Id)).OrderBy(a=>a.ArabicName).ToList();
            var res= Mapping.Mapper.Map<List<InvCategories>,List<CategoriesPOSTouchResponse>>(categories);

            return new ResponseResult() { Data = res };
        }

        public async Task<int> GetPriceListId(int? PersonId, int? PriceListId)
        {
            int salePriceId = 1;
            var Settings_ = await settings.SingleOrDefault(a => a.Id == 1);
            if (Settings_.other_ActivatePriceLists)
            {
                var userInfo = await _iUserInformation.GetUserInformation();

                salePriceId = await _mediator.Send(new salePriceListsRequest()
                {
                    branchId = userInfo.CurrentbranchId,
                    personId = PersonId.Value,
                    salesManId = 0,
                    employeeId = userInfo.employeeId, // hello shora
                    PriceListId = PriceListId.Value,
                    invoiceTypeId = (int)DocumentType.POS,
                    setting = Settings_
                });
                // Alaa
            }
            return salePriceId;
        }
        public async Task<ResponseResult> getItemsOfPOS(int categoryID , int PageNumber, int PageSize , string? itemName, int? PersonId,int? PriceListId)
        {
            int salePriceId = await GetPriceListId(PersonId,PriceListId);

           

            var items = itemQuery.TableNoTracking.Include(a=>a.Units).ThenInclude(a=>a.Unit)
                .Where(a =>(categoryID>0? a.GroupId==categoryID :(a.ArabicName.Contains(itemName)|| a.LatinName.Contains(itemName)))
                && a.TypeId!= (int)ItemTypes.Note &&
                  a.Status == (int)Status.Active && a.UsedInSales ).Select(a => new {
                    a.Id,
                    itemTypeId=a.TypeId,
                    a.ArabicName,
                    a.LatinName,
                    a.ImagePath,
                    units = a.Units.Where(e => e.UnitId == a.WithdrawUnit)
                              .Select(s=> new { unitId=s.UnitId , arabicName=s.Unit.ArabicName, latinName=s.Unit.LatinName,
                                  conversionFactor=s.ConversionFactor,
                                  salePrice1=(salePriceId==(int)SalePricesList.SalePrice1 ? s.SalePrice1:
                                              (salePriceId == (int)SalePricesList.SalePrice2 ? s.SalePrice2 :
                                              (salePriceId == (int)SalePricesList.SalePrice3 ? s.SalePrice3 : s.SalePrice4)))
                              }),
                    a.ApplyVAT,
                    vatRatio=a.VAT, 
                      a.ItemCode
                })
                 .OrderByDescending(a=> itemName == null ? a.ArabicName == a.ArabicName : a.ArabicName==itemName)
                  .ThenByDescending(a => itemName==null ? a.ArabicName== a.ArabicName : a.ArabicName.StartsWith(itemName))
                  .Skip((PageNumber - 1) * PageSize).Take(PageSize);
            //if (!string.IsNullOrEmpty(itemName))
            //    items = items.OrderByDescending(a => a.ArabicName.StartsWith(itemName));

           
            //var setting = settings.TableNoTracking.Where(a=>1 == 1).First();
            //if (setting.Pos_UseLastPrice)
            //{
            //    var lastPrices = personLastPriveQuery.TableNoTracking.Where(a => a.invoiceTypeId == (int)DocumentType.POS &&
            //               a.personId == 2
            //        && items.Select(e=>e.Id).Contains(a.itemId));
            //    foreach(var item in items)
            //    {
            //        var lastPrice = lastPrices.Where(a => a.itemId == item.Id && item.units.Select(e => e.unitId).Contains(a.unitId));
            //       foreach(var unit in item.units)
            //        {
            //            unit.salePrice1 = 1;
            //        }
            //        if(lastPrice.Count()>0)
            //        {
            //            item.units.Select(a => a.salePrice1 = lastPrice.Where(e => e.itemId == item.Id && e.unitId == a.unitId).First().price);
            //        }
            //    }

            //}


            var count = items.Count();
            var totalCount = itemQuery.TableNoTracking.Where(a => (categoryID > 0 ? a.GroupId == categoryID : (a.ArabicName.Contains(itemName) || a.LatinName.Contains(itemName))) 
            && a.TypeId != (int)ItemTypes.Note &&
                  a.Status == (int)Status.Active && a.UsedInSales).Count();
            //if (PageSize > 0 && PageNumber > 0)
            //{
            //    items = items.ToList().Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            //}
            //else
            //{
            //    return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            //}

            return new ResponseResult() { Data = items,DataCount=count,TotalCount= totalCount };
        }

        public async Task<ResponseResult> getItemsOfPOSIOS(int categoryID,string itemName, int PageNumber,int lastId, int PageSize, int? PriceListId,int? PersonId)
        {
            int salePriceId = await GetPriceListId(PersonId, PriceListId);

            var items = itemQuery.TableNoTracking.Include(a => a.Units).ThenInclude(a => a.Unit)
                 .Where(a => (categoryID!=0? a.GroupId == categoryID :1==1)&&
                 a.Status == (int)Status.Active && a.UsedInSales
                 && (itemName !=null? (a.ArabicName.Contains(itemName)|| a.LatinName.Contains(itemName)) :1==1)
                 && a.TypeId != (int)ItemTypes.Note ).Select(a => new {
                       a.Id,
                       itemTypeId = a.TypeId,
                       a.ArabicName,
                       a.LatinName,
                       a.ImagePath,
                       a.WithdrawUnit,
                       CatNameAr=a.Category.ArabicName,
                       CatId=a.Category.Id,
                       units = a.Units.OrderByDescending(u=>u.UnitId==a.WithdrawUnit)
                               .Select(s => new {
                                   unitId = s.UnitId,
                                   arabicName = s.Unit.ArabicName,
                                   latinName = s.Unit.LatinName,
                                   conversionFactor = s.ConversionFactor,
                                   salePrice1 = (salePriceId == (int)SalePricesList.SalePrice1 ? s.SalePrice1 :
                                              (salePriceId == (int)SalePricesList.SalePrice2 ? s.SalePrice2 :
                                              (salePriceId == (int)SalePricesList.SalePrice3 ? s.SalePrice3 : s.SalePrice4)))
                               }),
                       a.ApplyVAT,
                       vatRatio = a.VAT,
                       a.ItemCode
                   }).OrderByDescending(a => itemName == null ? a.ArabicName == a.ArabicName : a.ArabicName == itemName)
                  .ThenByDescending(a => itemName == null ? a.ArabicName == a.ArabicName : a.ArabicName.StartsWith(itemName))
                  .Where(a=>a.Id>lastId).Take(PageSize);

           var count = items.Count();
            var totalCount = itemQuery.TableNoTracking.Where(a => categoryID != 0 ? a.GroupId == categoryID : 1 == 1 && a.TypeId != (int)ItemTypes.Note &&
                  a.Status == (int)Status.Active && a.UsedInSales && (a.ArabicName.Contains(itemName) || a.LatinName.Contains(itemName))).Count();
           

            return new ResponseResult() { Data = items, DataCount = count, TotalCount = totalCount };
        }

        public async Task<ResponseResult> GetPOSTouchSettings()
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            var data = posTouchSettingsQuery.TableNoTracking.FirstOrDefault(a => a.UserId == userinfo.userId);

            

            if (data != null)
            {
                var response = new PosTouch()
                {
                    categoryImageHeight = data.PosTouch_CategoryImgHeight,
                    categoryImageWidth = data.PosTouch_CategoryImgWidth,
                    displayItemPrice = data.PosTouch_DisplayItemPrice,
                    fontSize = data.PosTouch_FontSize,
                    itemsImageHeight = data.PosTouch_ItemsImgHeight,
                    itemsImageWidth = data.PosTouch_ItemsImgWidth
                };
                return new ResponseResult() { Data = response, Result = Result.Success };
            }

            POSTouchSettings settings = new POSTouchSettings()
            {
                DeviceId = null,
                PosTouch_FontSize = 11,
                PosTouch_CategoryImgWidth = 140,
                PosTouch_CategoryImgHeight = 80,
                PosTouch_ItemsImgWidth = 144,
                PosTouch_ItemsImgHeight = 80,
                PosTouch_DisplayItemPrice = true,
                UserId = userinfo.userId,
            };

            var saved = await posTouchSettingsCommand.AddAsync(settings);

            var res = new PosTouch()
            {
                categoryImageHeight = settings.PosTouch_CategoryImgHeight,
                categoryImageWidth = settings.PosTouch_CategoryImgWidth,
                displayItemPrice = settings.PosTouch_DisplayItemPrice,
                fontSize = settings.PosTouch_FontSize,
                itemsImageHeight = settings.PosTouch_ItemsImgHeight,
                itemsImageWidth = settings.PosTouch_ItemsImgWidth
            };
            return new ResponseResult() { Data = res, Result = Result.Success };
        }

        public async Task<ResponseResult> UpdatePOSTouchSettings(POSTouchRequest request)
        {
            var userinfo = await _iUserInformation.GetUserInformation();

            var data = posTouchSettingsQuery.TableNoTracking.FirstOrDefault(a => a.UserId == userinfo.userId);

            if(data == null)
            {
                POSTouchSettings settings = new POSTouchSettings()
                {
                    DeviceId = null,
                    PosTouch_FontSize = request.fontSize,
                    PosTouch_CategoryImgWidth = request.categoryImageWidth,
                    PosTouch_CategoryImgHeight = request.categoryImageHeight,
                    PosTouch_ItemsImgWidth = request.itemsImageWidth,
                    PosTouch_ItemsImgHeight = request.itemsImageHeight,
                    PosTouch_DisplayItemPrice = request.displayItemPrice,
                    UserId = userinfo.userId,
                };

                await posTouchSettingsCommand.AddAsync(settings);

                return new ResponseResult() { Result = Result.Success };
            }

            data.PosTouch_FontSize = request.fontSize   ;
            data.PosTouch_CategoryImgWidth = request.categoryImageWidth   ;
            data.PosTouch_CategoryImgHeight = request.categoryImageHeight   ;
            data.PosTouch_ItemsImgWidth = request.itemsImageWidth   ;
            data.PosTouch_ItemsImgHeight = request.itemsImageHeight   ;
            data.PosTouch_DisplayItemPrice = request.displayItemPrice   ;

            var saved = await posTouchSettingsCommand.UpdateAsyn(data);

            return new ResponseResult() {  Result = Result.Success };
        }

        public async Task<ResponseResult> FillItemForPOSIOS(FillItemCardQueryRequest request)
        {
            int salePriceId = await GetPriceListId(request.PersonId,request.PriceListId);

            var res = await _mediator.Send(request);
            var itemData = (FillItemCardResponse)res.Data;
            if (res.Result != Result.Success)
                return res;
            var itemForIOSResponse = new ItemForIOSResponse()
            {
                ApplyVAT = itemData.ApplyVat,
                ArabicName = itemData.Item.ArabicName,
                LatinName = itemData.Item.LatinName,
                Id = itemData.ItemId,
                ItemCode = itemData.Item.ItemCode,
                itemTypeId = itemData.Item.TypeId,
                vatRatio = itemData.Vat,
                ImagePath = itemData.ImagePath,
               expiaryData=itemData.expiaryData,
               ExtractedSerials = itemData.ExtractedSerials,
               existedSerials = itemData.existedSerials,
               listSerials = itemData.listSerials,
               isBalanceBarcode=itemData.isBalanceBarcode,
               itemQuantity= !itemData.isBalanceBarcode?1: itemData.itemQuantity,
               balanceBarcode=itemData.balanceBarcode,
              
            };

            var unitsDto = itemUnitsQuery.TableNoTracking.Include(a => a.Unit).Where(a => a.ItemId == itemData.ItemId)
                          .Select(a => new { UnitId=a.UnitId, ArabicName= a.Unit.ArabicName, LatinName=a.Unit.LatinName,
                              ConversionFactor=a.ConversionFactor, SalePrice1= (salePriceId == (int)SalePricesList.SalePrice1 ? a.SalePrice1 :
                                              (salePriceId == (int)SalePricesList.SalePrice2 ? a.SalePrice2 :
                                              (salePriceId == (int)SalePricesList.SalePrice3 ? a.SalePrice3 : a.SalePrice4)))
                          }).ToList();
            itemForIOSResponse.units=unitsDto;
            

            res.Data = itemForIOSResponse;

            return res;
           
        }

    }
}
