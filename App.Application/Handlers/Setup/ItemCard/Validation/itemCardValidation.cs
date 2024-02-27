using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.ItemCard.Validation
{
    public static class itemCardValidation
    {
        /// <summary>
        /// This function is validation method to check the next values:
        ///     1- ConversionFactor
        ///     2- ArabicName
        ///     3- LatinName
        ///     4- ItemCode
        ///     5- National Barcode
        ///     6- Item Units
        ///     7- Units Action
        ///     8- Item Type Id
        ///     9- Units Of Serial Item
        ///     10- Composite Item Parts
        ///     11- Item Statues
        ///     12- Item Group
        ///     13- Item Units
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async static Task<ResponseResult>CheckRequestData(itemCardRequestData parm)
        {
            ResponseResult response = null;
            response = checkConversionFactor(parm.TypeId, parm.ConversionFactor);
            if (response != null)
                return response;
            response = checkArabicName(parm.ArabicName);
            if (response != null)
                return response;
            response = checkLatinName(parm.LatinName);
            if (response != null)
                return response;
            response = await checkItemCode(parm.ItemCode, parm.itemId, parm.invGeneralSettingsRepositoryQuery, parm._balanceBarcodeProcs,
                                      parm.itemCardRepositoryQuery, parm.itemUnitRepositoryQuery, parm.itemSerialRepositoryQuery);
            if (response != null)
                return response;
            response = await checkNationalBarcode(parm.NationalBarcode, parm.itemId, parm.invGeneralSettingsRepositoryQuery, parm._balanceBarcodeProcs,
                                      parm.itemCardRepositoryQuery, parm.itemUnitRepositoryQuery, parm.itemSerialRepositoryQuery);
            if (response != null)
                return response;
            response = await checkUnits(parm.Units,parm.itemId, parm.invGeneralSettingsRepositoryQuery, parm._balanceBarcodeProcs,
                                      parm.itemCardRepositoryQuery, parm.itemUnitRepositoryQuery, parm.itemSerialRepositoryQuery);
            if (response != null)
                return response;
            response = CheckUnitsAction(parm.TypeId, parm.Units, parm.WithdrawUnit, parm.DepositeUnit, parm.ReportUnit);
            if (response != null)
                return response;
            response = checkItemTypeId(parm.TypeId);
            if (response != null)
                return response;
            response = checkUnitsOfSerialItem(parm.TypeId, parm.Units);
            if (response != null)
                return response;
            response = checkCompositeItemParts(parm.TypeId, parm.Parts);
            if (response != null)
                return response;
            response = checkItemStatues(parm.Status);
            if (response != null)
                return response;
            response = checkItemGroup(parm.GroupId);
            if (response != null)
                return response;
            response = checkItemUnits(parm.TypeId, parm.Units);
            if (response != null)
                return response;
            response = checkitemUnitsUsed(parm.invoiceDetails, parm.TypeId);
            if (response != null)
                return response;
            response = checkAllUnitsExist(parm.invoiceDetails, parm.Units,parm.TypeId);
            if (response != null)
                return response;

            response = checkConversionFactorForUpdate(parm.TypeId,parm._itemUnits,parm.Units);
            if (response != null)
                return response;
            return response;
        }
        #region Validation Methods
        private static ResponseResult checkItemUnits(int? TypeId, ICollection<ItemUnitVM> Units)
        {
            if (TypeId != (int)ItemTypes.Note)
                if (Units.Count < 1)
                    return new ResponseResult()
                    {
                        Note = "This item should have one unit at less",
                        Result = Result.Failed,
                        ErrorMessageAr = "هذا الصنف يجب ان يحتوي علي وحده واحده علي الاقل",
                        ErrorMessageEn = "This item should have one unit at less"
                    };
            return null;
        }
        private static ResponseResult checkConversionFactor(int? TypeId, double ConversionFactor)
        {
            if (TypeId != 6)
                if (ConversionFactor != 1)
                    return new ResponseResult()
                    {
                        Note = Actions.FirstUnitConversionFactor,
                        Result = Result.Failed,
                        ErrorMessageAr = "الوحده الاولي للصنف يجب ان يكون معامل التحويل الخاص بها يساوي 1",
                        ErrorMessageEn = "First Unit Conversion Factor should be 1"
                    };
            return null;
        }
        private static ResponseResult checkArabicName(string ArabicName)
        {
            if (ArabicName.Length > 250)
                return new ResponseResult()
                {
                    Note = Actions.ArabicNameLengthIsMoreThanMaxmum,
                    Result = Result.Failed,
                    ErrorMessageEn = "Arabic Name Length Is More Than Maxmum",
                    ErrorMessageAr = "عدد حروف الاسم عربي اكبر من المسموح به"
                };
            return null;
        }
        private static ResponseResult checkLatinName(string LatinName)
        {
            if (LatinName != null)
                if (LatinName.Length > 250)
                    return new ResponseResult()
                    {
                        Note = Actions.LatinNameLengthIsMoreThanMaxmum,
                        Result = Result.Failed,
                        ErrorMessageAr = "عدد حروف الاسم انجليزي اكبر من العدد المسموح به",
                        ErrorMessageEn = "Latin Name Length Is More Than Maxmum"
                    };
            return null;
        }
        private static ResponseResult checkItemTypeId(int? TypeId)
        {
            if (TypeId < 1 || TypeId > 6)
                return new ResponseResult()
                {
                    Note = "Type Id is wrong",
                    Result = Result.Failed,
                    ErrorMessageEn = "Type Id is wrong",
                    ErrorMessageAr = "خطأ نوع الصنف"
                };
            return null;
        }
        private async static Task<ResponseResult> checkItemCode(string ItemCode,int? itemId, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery, IBalanceBarcodeProcs _balanceBarcodeProcs,
                                                        IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery, IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery,
                                                         IRepositoryQuery<InvSerialTransaction> itemSerialRepositoryQuery)
        {
            var CheckCode = await itemCardHelpers.CheckCode(ItemCode, itemId, invGeneralSettingsRepositoryQuery, _balanceBarcodeProcs, itemCardRepositoryQuery, itemUnitRepositoryQuery, itemSerialRepositoryQuery);
            if (CheckCode != null)
                return CheckCode;
            return null;
        }
        private async static Task<ResponseResult> checkNationalBarcode(string NationalBarcode,
                                    int? ItemCode,      
                                                         IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery,
                                                         IBalanceBarcodeProcs _balanceBarcodeProcs,
                                                         IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery,
                                                         IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery,
                                                         IRepositoryQuery<InvSerialTransaction> itemSerialRepositoryQuery)
        {
            if (!string.IsNullOrEmpty(NationalBarcode))
            {
                var CheckCode = await itemCardHelpers.CheckCode(NationalBarcode, ItemCode, invGeneralSettingsRepositoryQuery, _balanceBarcodeProcs, itemCardRepositoryQuery, itemUnitRepositoryQuery, itemSerialRepositoryQuery);
                if (CheckCode != null)
                    return CheckCode;
            }
            return null;
        }
        private async static Task<ResponseResult> checkUnits(ICollection<ItemUnitVM> Units,int? itemId,
                                                     IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery,
                                                     IBalanceBarcodeProcs _balanceBarcodeProcs,
                                                     IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery,
                                                     IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery,
                                                     IRepositoryQuery<InvSerialTransaction> itemSerialRepositoryQuery)
        {
            foreach (var item in Units)
            {
                if (!string.IsNullOrEmpty(item.Barcode))
                {
                    var CheckCode = await itemCardHelpers.CheckCode(item.Barcode, itemId, invGeneralSettingsRepositoryQuery, _balanceBarcodeProcs, itemCardRepositoryQuery, itemUnitRepositoryQuery, itemSerialRepositoryQuery);
                    if (CheckCode != null)
                        return CheckCode;
                }
            }
            return null;
        }
        private static ResponseResult CheckUnitsAction(int? TypeId, ICollection<ItemUnitVM> Units, int? WithdrawUnit, int? DepositeUnit, int? ReportUnit)
        {
            if (TypeId != (int)ItemTypes.Note)
            {
                if (Units == null)
                    return null;
                if (!Units.Where(x => x.UnitId == WithdrawUnit).Any())
                    return new ResponseResult()
                    {
                        Note = "Withdraw Unit Id is wrong",
                        Result = Result.Failed,
                        ErrorMessageEn = "Withdraw Unit Id is wrong",
                        ErrorMessageAr = "خطأ وحده الصرف"
                    };
                if (!Units.Where(x => x.UnitId == DepositeUnit).Any())
                    return new ResponseResult()
                    {
                        Note = "Deposite Unit Id is wrong",
                        Result = Result.Failed,
                        ErrorMessageEn = "Deposite Unit Id is wrong",
                        ErrorMessageAr = "خطأ وحده الاضافة"
                    };
                if (!Units.Where(x => x.UnitId == ReportUnit).Any())
                    return new ResponseResult()
                    {
                        Note = "Report Unit Id is wrong",
                        Result = Result.Failed,
                        ErrorMessageEn = "Report Unit Id is wrong",
                        ErrorMessageAr = "خطأ وحده التقارير"
                    };
            }
            return null;
        }
        private static ResponseResult checkUnitsOfSerialItem(int? TypeId, ICollection<ItemUnitVM> Units)
        {
            if (TypeId == (int)ItemTypes.Serial)
            {
                if (Units == null)
                    return null;
                if (Units.Count() > 1)
                    return new ResponseResult()
                    {
                        Note = "serial item can not have more than one unit",
                        Result = Result.Failed,
                        ErrorMessageAr = "الصنف المسلسل لا يمكن ان يحتوي علي اكتر من وحده واحده",
                        ErrorMessageEn = "serial item can not have more than one unit"
                    };
            }
            return null;
        }
        private static ResponseResult checkCompositeItemParts(int? TypeId, ICollection<ItemPartVM> Parts)
        {
            if (TypeId == (int)ItemTypes.Composite)
                if (Parts.Count() < 1)
                    return new ResponseResult()
                    {
                        Note = "Composite item should have parts",
                        Result = Result.Failed,
                        ErrorMessageAr = "الصنف المجمع يجب ان يحتوي علي المكونات",
                        ErrorMessageEn = "Composite item should have parts"
                    };
            return null;
        }
        private static ResponseResult checkItemStatues(int Status)
        {
            if (Status < 1 || Status > 2)
            {
                return new ResponseResult()
                {
                    Note = "item statues is wrong,you shuold use 1 for active and 2 for deactivate",
                    Result = Result.Failed,
                    ErrorMessageAr = "خطأ حالة الصنف, يجب ان تكون حاله الصنف 1 للنشط و 2 للغير نشط",
                    ErrorMessageEn = "item statues is wrong,you shuold use 1 for active and 2 for deactivate"
                };
            }
            return null;
        }
        private static ResponseResult checkItemGroup(int GroupId)
        {
            if (GroupId == null)
            {
                return new ResponseResult()
                {
                    Note = "Group Id is Required",
                    Result = Result.Failed,
                    ErrorMessageAr = "يجب اختيار مجموعه للصنف",
                    ErrorMessageEn = "Group Id is Required"
                };
            }
            return null;
        }
        private static ResponseResult checkitemUnitsUsed(List<InvoiceDetails> invoiceDetails, int? TypeId)
        {
            if (TypeId != (int)ItemTypes.Note)
            {
                if (invoiceDetails == null)
                    return null;
                if (invoiceDetails.Any())
                    if (invoiceDetails.FirstOrDefault().Items.TypeId != TypeId)
                        return new ResponseResult()
                        {
                            Note = "Cant Change item type because this item is used in invices"
                        };
            }
            return null;
        }
        private static ResponseResult checkAllUnitsExist(List<InvoiceDetails> invoiceDetails, ICollection<ItemUnitVM> Units, int? TypeId)
        {
            if (TypeId != (int)ItemTypes.Note)
            {
                if (invoiceDetails == null)
                    return null;
                var checkAllUnitsExist = invoiceDetails.Where(x => !Units.Select(d => d.UnitId).ToArray().Contains(x.UnitId ?? 0)).Any();
                if (checkAllUnitsExist)
                    return new ResponseResult()
                    {
                        Note = "the used Units Not Exist"
                    };
            }
            return null;
        }
        private static ResponseResult checkConversionFactorForUpdate(int? TypeId, IQueryable<InvStpItemCardUnit> _itemUnits, ICollection<ItemUnitVM> Units)
        {
            if (TypeId != (int)ItemTypes.Note)
            {
                if (_itemUnits == null)
                    return null;
                bool stopAPI = false;
                _itemUnits.ToList().ForEach(x =>
                {
                    if (x.ConversionFactor != Units.Where(c => c.UnitId == x.UnitId).FirstOrDefault().ConversionFactor)
                        stopAPI = true;

                });
                if (stopAPI)
                    return new ResponseResult()
                    {
                        Note = "you cant change conversion factor after unit used in invices"
                    };
            }
            return null;
        }

        #endregion
        public static async Task<ResponseResult> CheckIfItemExist(int itemId,IRepositoryQuery<InvStpItemCardMaster> itemCardQueryRepository)
        {
            ResponseResult res = null;
            var isItemExist = itemCardQueryRepository.TableNoTracking.Where(x=> x.Id== itemId).Any();
            if (!isItemExist)
                res = new ResponseResult()
                {
                    Result = Result.Failed,
                    Note = "Item is Not Exist",
                    ErrorMessageAr = "هذا الصنف غير موجود",
                    ErrorMessageEn = "This Item is not Exist"
                };
            return res;
        }
        public static async Task<ResponseResult> CheckIfItemUsed(int itemId, IRepositoryQuery<InvStpItemCardMaster> itemCardQueryRepository, IRepositoryQuery<InvoiceDetails> InvoiceDetailsrepository, IRepositoryQuery<OfferPriceDetails> OfferPriceDetailsQuery)
        {
            ResponseResult res = null;
            res = await CheckIfItemExist(itemId, itemCardQueryRepository);
            if(res == null)
            {
                var isItemUsed = InvoiceDetailsrepository.TableNoTracking.Where(x => x.ItemId == itemId).Any();
                isItemUsed = OfferPriceDetailsQuery.TableNoTracking.Where(x => x.ItemId == itemId).Any();
                if (isItemUsed)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        Note = "Item is used in invoice",
                        ErrorMessageAr = "هذا الصنف مستخدم في الحسابات",
                        ErrorMessageEn = "Item is used in invoice"
                    };
            }
            return res;
        }


    }
    public class itemCardRequestData
    {
        public int? itemId { get; set; }
        public int? TypeId { get; set; }
        public int? WithdrawUnit { get; set; }
        public int? DepositeUnit { get; set; }
        public int? ReportUnit { get; set; }
        public double ConversionFactor { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string ItemCode { get; set; }
        public string NationalBarcode { get; set; }
        public int Status { get; set; }
        public int GroupId { get; set; }
        public IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery { get; set; }
        public IBalanceBarcodeProcs _balanceBarcodeProcs { get; set; }
        public IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery { get; set; }
        public IRepositoryQuery<InvStpItemCardUnit> itemUnitRepositoryQuery { get; set; }
        public IRepositoryQuery<InvSerialTransaction> itemSerialRepositoryQuery { get; set; }
        public ICollection<ItemUnitVM> Units { get; set; }
        public ICollection<ItemPartVM> Parts { get; set; }
        public List<InvoiceDetails> invoiceDetails { get; set; }
        public IQueryable<InvStpItemCardUnit> _itemUnits { get; set; }

    }
}
