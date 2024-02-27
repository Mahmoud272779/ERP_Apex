using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.MergeItems
{
    public static class MergeItemsMethod
    {
        public static async Task<List<InvoiceDetailsRequest>> MergeItems(List<InvoiceDetailsRequest> list, int invoiceType)
        {
            var listDto = new List<InvoiceDetailsRequest>(); // list which will return after marge

            //    var SettingsOfOtherDecimal = (forSave?RoundNumbers.defaultDecimal:  GeneralSettings.TableNoTracking.First().Other_Decimals);
            foreach (var item in list)
            {
                var margeItemDto = new InvoiceDetailsRequest();
                if (listDto.Count() > 0)
                {
                    if (item.ItemTypeId == (int)ItemTypes.Serial)
                    {
                        AddMergeItem(listDto, item, margeItemDto);
                        continue;
                    }

                    var x = listDto.FirstOrDefault(q => q.ItemId == item.ItemId && q.UnitId == item.UnitId
                            && q.Price == item.Price);

                    if (x != null)
                    {
                        if (item.ItemTypeId == (int)ItemTypes.Expiary && item.ExpireDate != x.ExpireDate)
                        {
                            AddMergeItem(listDto, item, margeItemDto);
                        }
                        else
                        {
                            x.ExpireDate = item.ExpireDate;
                            x.ItemTypeId = item.ItemTypeId;
                            x.SplitedDiscountValue += item.SplitedDiscountValue;
                            x.SplitedDiscountRatio = item.SplitedDiscountRatio;
                            x.Quantity += item.Quantity;
                            x.Price = item.Price;
                            x.VatRatio = item.VatRatio;
                            x.VatValue += item.VatValue;
                            //  x.DiscountRatio += item.DiscountRatio;
                            x.DiscountValue += item.DiscountValue;
                            x.ConversionFactor = item.ConversionFactor;
                            x.Total += item.Total;
                            x.UnitId = item.UnitId;
                            x.SizeId = item.SizeId;
                            x.DiscountRatio = (x.DiscountValue / (x.Total + x.DiscountValue)) * 100;// Math.Round( (x.DiscountValue / (x.Total+x.DiscountValue)) * 100 , SettingsOfOtherDecimal);


                        }
                    }
                    else
                    {
                        AddMergeItem(listDto, item, margeItemDto);
                    }
                }
                else
                {
                    AddMergeItem(listDto, item, margeItemDto);
                }

            }
            return listDto;
        }
        public static void AddMergeItem(List<InvoiceDetailsRequest> listDto, InvoiceDetailsRequest item, InvoiceDetailsRequest mergeItemDto)
        {
            mergeItemDto.ItemId = item.ItemId;
            mergeItemDto.ItemCode = item.ItemCode;
            //  mergeItemDto.ItemNameAr = item.ItemNameAr;
            //  mergeItemDto.ItemNameEn = item.ItemNameEn;
            mergeItemDto.UnitId = item.UnitId;
            mergeItemDto.SizeId = item.SizeId;
            // mergeItemDto.UnitNameAr = item.UnitNameAr;
            //  mergeItemDto.UnitNameEn = item.UnitNameEn;
            mergeItemDto.Quantity = item.Quantity;
            mergeItemDto.Price = item.Price;
            mergeItemDto.ApplyVat = item.ApplyVat;
            mergeItemDto.VatRatio = item.VatRatio;
            mergeItemDto.VatValue = item.VatValue;
            mergeItemDto.DiscountRatio = item.DiscountRatio;
            mergeItemDto.DiscountValue = item.DiscountValue;
            mergeItemDto.TransQuantity = item.TransQuantity;
            mergeItemDto.TransStatus = item.TransStatus;
            mergeItemDto.ExpireDate = item.ExpireDate;
            mergeItemDto.ItemTypeId = item.ItemTypeId;
            mergeItemDto.AutoDiscount = item.AutoDiscount;
            mergeItemDto.SplitedDiscountValue = item.SplitedDiscountValue;
            mergeItemDto.SplitedDiscountRatio = item.SplitedDiscountRatio;
            mergeItemDto.ConversionFactor = item.ConversionFactor;
            mergeItemDto.Total += item.Total;
            mergeItemDto.parentItemId = item.parentItemId;
            mergeItemDto.TotalWithSplitedDiscount = item.TotalWithSplitedDiscount;
            mergeItemDto.isDiscountRatioItem = item.isDiscountRatioItem;
            mergeItemDto.VatValueForPrint = item.VatValueForPrint;
            if (item.ItemTypeId == (int)ItemTypes.Serial)
            {
                var serialList = new List<string>();
                foreach (var serials in item.ListSerials)
                {

                    //var serial = new ListOfSerials();
                    //serial.SerialNumber = serials ;

                    serialList.Add(serials);

                }
                mergeItemDto.ListSerials.AddRange(serialList);

            }
            listDto.Add(mergeItemDto);

        }
    }
}
