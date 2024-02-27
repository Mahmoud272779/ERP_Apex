using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.GetItemData
{
    public class GetItemDataHandler : IRequestHandler<GetItemDataRequest, ItemAllData>
    {
        public async Task<ItemAllData> Handle(GetItemDataRequest request, CancellationToken cancellationToken)
        {
            var itemMaster = request.itemCardMasterRepository.TableNoTracking.Include(a => a.Serials).Where(a => (a.ItemCode == request.ItemCode
                                            || a.NationalBarcode == request.ItemCode) || a.Serials.Select(e => e.SerialNumber).Contains(request.ItemCode));// || a.Serials.Where(e=>e.ExtractInvoice==null).Select(e=>e.SerialNumber).Contains(request.ItemCode) ).ToList();


            //    var itemFromSerial = serialTransactionQuery.TableNoTracking.Where(a => a.SerialNumber == request.ItemCode)
            //      .Select(a => a.ItemId).ToList().First();
            var ItemData = request.itemUnitsQuery.TableNoTracking
             .Include(a => a.Item)
             .Include(a => a.Unit)
             .Where(a => ((a.Item.ItemCode == request.ItemCode
                                          || a.Barcode == request.ItemCode || a.Item.NationalBarcode == request.ItemCode
                                           || a.Item.Serials.Where(s => s.ExtractInvoice == null || s.TransferStatus == TransferStatus.Binded).Select(e => e.SerialNumber).Contains(request.ItemCode)) && // بجيب السيريال متاح ف انهي صنف
                                          (request.UnitId > 0 ? a.UnitId == request.UnitId :
                                                    (a.Barcode == request.ItemCode ? (1 == 1) : (request.IsAdd == true ? a.Item.DepositeUnit == a.UnitId : a.Item.WithdrawUnit == a.UnitId)))
                                          && a.Item.Status == (int)Status.Active)).ToList();

            return new ItemAllData()
            {
                itemData = ItemData,
                itemMaster = itemMaster
            };
        }
    }
}
