using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Models.Response.Barcode;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace App.Application.Handlers.Barcode.GetBarcodeItemsFromInvoices
{
    public class GetBarcodeItemsFromInvoicesHandler : IRequestHandler<GetBarcodeItemsFromInvoicesRequest, List<PrintingResponseDTO>>
    {
        private readonly IRepositoryQuery<InvoiceDetails> _InvoiceDetailsQuery;
        private readonly IRoundNumbers _roundNumbers;
        private readonly IConfiguration _configuration;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        public GetBarcodeItemsFromInvoicesHandler(IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery, IRoundNumbers roundNumbers, IConfiguration configuration, ISecurityIntegrationService securityIntegrationService)
        {
            _InvoiceDetailsQuery = invoiceDetailsQuery;
            _roundNumbers = roundNumbers;
            _configuration = configuration;
            _securityIntegrationService = securityIntegrationService;
        }
        public async Task<List<PrintingResponseDTO>> Handle(GetBarcodeItemsFromInvoicesRequest request, CancellationToken cancellationToken)
        {
            string timeStartAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss").Replace(":", string.Empty);
            var companyInfo = await _securityIntegrationService.getCompanyInformation();
            var items = _InvoiceDetailsQuery.TableNoTracking
                .Where(x => request.PrintItemsBarcodeRequestDetalies.Select(c => c.ivoiceDetaliesId).ToArray().Contains(x.Id))
                .Include(x=> x.Items)
                .ThenInclude(x=> x.Category)
                .Include(x=> x.Items.Units);
            if (!items.Any())
                return null;
            List<PrintingResponseDTO> listOfPrintingItems = new List<PrintingResponseDTO>();
            foreach (var item in request.PrintItemsBarcodeRequestDetalies)
            {
               
                var currentItem = items.Where(c => c.ItemId == item.itemId).FirstOrDefault();
                if (currentItem == null)
                    continue;
                var price = currentItem.Items.Units.Where(c => c.UnitId == item.unitId).FirstOrDefault().SalePrice1;
                var itemUnit = currentItem.Items.Units.Where(c => c.UnitId == item.unitId).FirstOrDefault();
                var unitBarcode = string.Empty;
                if (itemUnit != null)
                {
                    if (!string.IsNullOrEmpty(itemUnit.Barcode))
                        unitBarcode = itemUnit.Barcode;
                    else
                        unitBarcode = currentItem.Items.ItemCode;
                }
                for (int i = 0; i < item.count; i++)
                {
                    var expairDate = items.Where(c => c.Id == item.ivoiceDetaliesId).FirstOrDefault()?.ExpireDate;
                    listOfPrintingItems.Add(new PrintingResponseDTO
                    {
                        itemCode = unitBarcode,
                        arabicName = currentItem.Items.ArabicName,
                        latinName = currentItem.Items.LatinName,
                        categoryAr = currentItem.Items.Category.ArabicName,
                        categoryEn = currentItem.Items.Category.LatinName,
                        expairDate = currentItem.Items.TypeId == (int)ItemTypes.Expiary ? expairDate.Value.ToString("dd/MM/yyyy") : "",
                        price = price,
                        priceWithVat = _roundNumbers.GetRoundNumber((price * (currentItem.Items.VAT / 100)) + price),
                        BarcodeURL = QRCode.GenerateBarcode(unitBarcode, Barcodestander_BarcodeType.Code128,_configuration, companyInfo.companyLogin+"_"+ timeStartAt + "_"+unitBarcode)
                    });
                }
            }
            return listOfPrintingItems;
        }
    }
}
