using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class GetItemCardQuery : BaseClass, IRequestHandler<GetItemCardRequest, GetItemCardResponse>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepository;

        private readonly IRepositoryQuery<InvCategories> categoriesRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsRepositoryQuery;
        private readonly IRepositoryQuery<InvStorePlaces> storePlacesRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardStores> itemStoresRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardParts> itemPartsRepositoryQuery;

        public GetItemCardQuery(IHttpContextAccessor _httpContextAccessor, IRepositoryQuery<InvStpItemCardMaster> ItemCardRepository
            , IRepositoryQuery<InvCategories> categoriesRepositoryQuery, IRepositoryQuery<InvStpItemCardUnit> ItemUnitsRepositoryQuery
                  , IRepositoryQuery<InvStpItemCardStores> ItemStoresRepositoryQuery,
            IRepositoryQuery<InvStorePlaces> StorePlacesRepositoryQuery,
            IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
            IRepositoryQuery<InvStpItemCardParts> ItemPartsRepositoryQuery) : base(_httpContextAccessor)
        {
            itemCardRepository = ItemCardRepository;
            this.categoriesRepositoryQuery = categoriesRepositoryQuery;
            itemUnitsRepositoryQuery = ItemUnitsRepositoryQuery;
            itemStoresRepositoryQuery = ItemStoresRepositoryQuery;
            itemPartsRepositoryQuery = ItemPartsRepositoryQuery;
            storePlacesRepositoryQuery = StorePlacesRepositoryQuery;
            _invoiceDetailsQuery = InvoiceDetailsQuery;
        }

        public async Task<GetItemCardResponse> Handle(GetItemCardRequest request, CancellationToken cancellationToken)
        {
            var results = itemCardRepository.TableNoTracking.Where(a => a.Id == request.ItemId).FirstOrDefault();
            var isItemInvoicesDetaliesExist = _invoiceDetailsQuery.TableNoTracking.Where(x => x.ItemId == request.ItemId);
            var masterDto = new GetItemCardResponse();
            masterDto.CanEdit = true;
            //Check if the item exists in the itemparts table so the CanEdit will be false if it exists in the item part
            var itemPartExist = await itemPartsRepositoryQuery.FindAllAsync(e => e.PartId == request.ItemId);
            if (itemPartExist.Count > 0 || isItemInvoicesDetaliesExist.Any())
            {
                masterDto.CanEdit = false;
            }

            masterDto.Id = results.Id;
            masterDto.Status = results.Status;
            masterDto.ApplyVAT = results.ApplyVAT;
            masterDto.ArabicName = results.ArabicName;
            masterDto.VAT = results.VAT;
            masterDto.ImagePath = results.ImagePath;
            masterDto.StorePlace.DefaultStoreId = results.DefaultStoreId;
            var storeplace = await storePlacesRepositoryQuery.GetByAsync(q => q.Id == results.DefaultStoreId);
            if (storeplace != null)
            {
                masterDto.StorePlace.ArabicName = storeplace.ArabicName;
                masterDto.StorePlace.LatinName = storeplace.LatinName;
                masterDto.StorePlace.StorePlaceActive = storeplace.Status;
            }


            masterDto.DepositeUnit = results.DepositeUnit;
            masterDto.Description = results.Description;
            masterDto.GroupId = results.GroupId;
            var CategoryData = await categoriesRepositoryQuery.GetByAsync(q => q.Id == masterDto.GroupId);
            masterDto.Category.CategoryActive = CategoryData.Status;
            masterDto.Category.ArabicName = CategoryData.ArabicName;
            masterDto.Category.LatinName = CategoryData.LatinName;

            masterDto.Image = results.Image;

            masterDto.ItemCode = results.ItemCode;
            masterDto.LatinName = results.LatinName;
            masterDto.Model = results.Model;
            masterDto.NationalBarcode = results.NationalBarcode;
            masterDto.ReportUnit = results.ReportUnit;
            masterDto.TypeId = results.TypeId;
            masterDto.UsedInSales = results.UsedInSales;
            masterDto.WithdrawUnit = results.WithdrawUnit;

            var category = await categoriesRepositoryQuery.GetByAsync(q => q.Id == results.GroupId);
            if (category != null)
            {
                masterDto.Category.ArabicName = category.ArabicName;
                masterDto.Category.Id = category.Id;
                masterDto.Category.Code = category.Code;
                masterDto.Category.LatinName = category.LatinName;

            }
            var units = await itemUnitsRepositoryQuery.Find(a => a.ItemId == results.Id, a => a.Unit);
            units = units.OrderBy(a => a.ConversionFactor).ToList();
            foreach (var unit in units)
            {
                var unitExistInitemPart = itemPartsRepositoryQuery.TableNoTracking.Where(a => a.UnitId == unit.UnitId && a.PartId == unit.ItemId);
                var unitDto = new UnitsVm();
                unitDto.ItemId = masterDto.Id;
                unitDto.UnitId = unit.UnitId;
                unitDto.ArabicName = unit.Unit?.ArabicName;
                unitDto.LatinName = unit.Unit?.LatinName;
                unitDto.ConversionFactor = unit.ConversionFactor;
                unitDto.Barcode = unit.Barcode;
                unitDto.PurchasePrice = unit.PurchasePrice;
                unitDto.SalePrice1 = unit.SalePrice1;
                unitDto.SalePrice2 = unit.SalePrice2;
                unitDto.SalePrice3 = unit.SalePrice3;
                unitDto.SalePrice4 = unit.SalePrice4;
                unitDto.UnitActive = unit.Unit.Status;
                if (isItemInvoicesDetaliesExist.Any() && isItemInvoicesDetaliesExist.Where(x=> x.UnitId == unit.UnitId).Any() || unitExistInitemPart.Count() >0)
                {
                    unitDto.CanEditUnit = false;
                }
                else
                    unitDto.CanEditUnit = true;

                masterDto.Units.Add(unitDto);
            }

            var stores = await itemStoresRepositoryQuery.Find(a => a.ItemId == results.Id, a => a.Store);
            if (stores.Count() > 0)
            {
                foreach (var store in stores)
                {
                    var storeDto = new StoresVm();
                    storeDto.ItemId = store.ItemId;
                    storeDto.StoreId = store.StoreId;
                    storeDto.DemandLimit = store.DemandLimit;
                    storeDto.ArabicName = store.Store?.ArabicName;
                    storeDto.LatinName = store.Store?.LatinName;
                    storeDto.StoreCode = store.Store.Code;
                    storeDto.StoreActive = store.Store.Status;
                    masterDto.Stores.Add(storeDto);
                }
            }

            var itemPart = itemPartsRepositoryQuery.TableNoTracking.Include(x=> x.CardMaster).Include(a => a.PartDetails).Include(a => a.Unit).Where(a => a.ItemId == results.Id);
            if (itemPart != null)
            {
                foreach (var part in itemPart)
                {
                    var partDto = new PartsVm();
                    partDto.ItemId = part.PartDetails.Id;
                    partDto.ItemCode = part.PartDetails.ItemCode;
                    partDto.ArabicName = part.PartDetails.ArabicName;
                    partDto.LatinName = part.PartDetails.LatinName;
                    partDto.Quantity = part.Quantity;
                    partDto.UnitId = part.UnitId;
                    partDto.UnitAr = part.Unit.ArabicName;
                    partDto.UnitEn = part.Unit.LatinName;
                    partDto.ItemPartActive = part.PartDetails.Status;
                    partDto.itemType = part.CardMaster.TypeId;
                    masterDto.ItemParts.Add(partDto);


                }
            }

            //Mapping.Mapper.Map(results, responses, typeof(InvStpItemCardMaster), typeof(GetItemCardResponse));
            return masterDto;
        }
    }
}
