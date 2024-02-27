using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Barcode;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Barcode.GetBarcodeItemsFromItemCard
{
    public class getBarcodeItemsFromItemCardHandler : IRequestHandler<getBarcodeItemsFromItemCardRequest, List<PrintingResponseDTO>>
    {

        private readonly IRepositoryQuery<InvStpItemCardMaster> _itemCardQuery;
        private readonly IConfiguration _configuration;
        private readonly ISecurityIntegrationService _securityIntegrationService;


        public getBarcodeItemsFromItemCardHandler(IRepositoryQuery<InvStpItemCardMaster> itemCardQuery = null, IConfiguration configuration = null, ISecurityIntegrationService securityIntegrationService = null)
        {
            _itemCardQuery = itemCardQuery;
            _configuration = configuration;
            _securityIntegrationService = securityIntegrationService;
        }
        public async Task<List<PrintingResponseDTO>> Handle(getBarcodeItemsFromItemCardRequest request, CancellationToken cancellationToken)
        {
            string timeStartAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss").Replace(":", string.Empty);
            var companyInfo = await _securityIntegrationService.getCompanyInformation();
            var items = _itemCardQuery.TableNoTracking
                .Where(x => request.PrintItemsBarcodeRequestDetalies.Select(c => c.itemId).ToArray().Contains(x.Id))
                .Include(c=> c.Units)
                .Include(c => c.Category);
            List<PrintingResponseDTO> listOfPrintingItems = new List<PrintingResponseDTO>();
            foreach (var item in request.PrintItemsBarcodeRequestDetalies)
            {
                var currentItem = items.Where(c => c.Id == item.itemId).FirstOrDefault();
                var price = currentItem.Units.Where(c => c.UnitId == item.unitId).FirstOrDefault().SalePrice1;
                for (int i = 0; i < item.count; i++)
                {
                    var itemUnit = currentItem.Units.Where(c => c.UnitId == item.unitId).FirstOrDefault();
                    var unitBarcode = string.Empty;
                    if(itemUnit != null)
                    {
                        if (!string.IsNullOrEmpty(itemUnit.Barcode))
                            unitBarcode = itemUnit.Barcode;
                        else
                            unitBarcode = currentItem.ItemCode;
                    }
                    listOfPrintingItems.Add(new PrintingResponseDTO
                    {
                        itemCode = unitBarcode,
                        arabicName = currentItem.ArabicName,
                        latinName = currentItem.LatinName,
                        categoryAr = currentItem.Category.ArabicName,
                        categoryEn = currentItem.Category.LatinName,
                        expairDate = currentItem.TypeId == (int)ItemTypes.Expiary ?  item.expairDate.Value.Date.ToString("dd/MM/yyyy") : "",
                        price = price,
                        priceWithVat = (price * (currentItem.VAT / 100)) + price,
                        BarcodeURL = QRCode.GenerateBarcode(unitBarcode, Barcodestander_BarcodeType.Code128, _configuration, companyInfo.companyLogin + "_" + timeStartAt + "_" + unitBarcode)
                    });
                }
            }
            return listOfPrintingItems;
        }
    }
}
