using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Infrastructure.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public static class itemCardHelpers
    {
        public static async Task<ResponseResult> CheckCode
            (
            string code,
            int? itemId,
            IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery,
            IBalanceBarcodeProcs balanceBarcodeProcs,
            IRepositoryQuery<InvStpItemCardMaster> ItemCardMasterQuery,
            IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitQuery,
            IRepositoryQuery<InvSerialTransaction> serialQuery
            )
        {
            if (string.IsNullOrEmpty(code))
                return null;
            var res = new ResponseResult();

            var ItemCodestart = invGeneralSettingsQuery.TableNoTracking.FirstOrDefault().Barcode_ItemCodestart ? 1 : 0;
            var checkCode = balanceBarcodeProcs.getItem(new BalanceBarcodeInput
            {
                FullCode = code,
                ItemCodestart = ItemCodestart
            });

            //check in itemcard table
            var findInItemCardTable = ItemCardMasterQuery.TableNoTracking
                .Where(x=> itemId != null ? x.Id != itemId : true)
                .Where(x => checkCode == null ? (x.ItemCode == code || x.NationalBarcode == code) : (x.ItemCode == checkCode.ItemCode || x.NationalBarcode == checkCode.ItemCode)).Any();
            if (findInItemCardTable) 
            {
                res.Result = Result.Exist;
                res.ErrorMessageAr = checkCode == null ? "الكود موجود بالفعل" : "باركود ميزان مكرر";
                res.ErrorMessageEn = checkCode == null ? "Code is already exist" : "Scale barcode Exist";
                return res;
            }
            //check in units table
            var findInUnitsTable = ItemCardUnitQuery.TableNoTracking.Where(x => itemId != null ? x.ItemId != itemId : true).Where(x => x.Barcode == code).Any();
            if (findInUnitsTable)
            {
                res.Result = Result.Exist;
                res.ErrorMessageAr = "الكود مستخدم في وحدة";
                res.ErrorMessageEn = "Code is is used in unit";
                return res;
            }
            //check in serials table
            var findInSerialsTable = serialQuery.TableNoTracking.Where(x => x.SerialNumber == code).Any();
            if (findInSerialsTable)
            {
                res.Result = Result.Exist;
                res.ErrorMessageAr = "الكود مستخدم في صنف مسلسل";
                res.ErrorMessageEn = "Code is is used in serial item";
                return res;
            }
            //return
            return null;
        }
    }
}
