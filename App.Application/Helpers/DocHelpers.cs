using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public static class DocHelpers
    {
        public static int GetPageIdOfDoc(int invoiceTypeId)
        {
            int pageId = 0;
            if (invoiceTypeId == (int)Enums.DocumentType.Sales) pageId = (int)SubFormsIds.Sales;
            else if (invoiceTypeId == (int)Enums.DocumentType.ReturnSales) pageId = (int)SubFormsIds.SalesReturn_Sales;
            else if (invoiceTypeId == (int)Enums.DocumentType.Purchase) pageId = (int)SubFormsIds.Purchases;
            else if (invoiceTypeId == (int)Enums.DocumentType.ReturnPurchase) pageId = (int)SubFormsIds.PurchasesReturn_Purchases;
            else if (invoiceTypeId == (int)Enums.DocumentType.POS) pageId = (int)SubFormsIds.POS;
            else if (invoiceTypeId == (int)Enums.DocumentType.ReturnPOS) pageId = (int)SubFormsIds.returnPOS;
            else if (invoiceTypeId == (int)Enums.DocumentType.OfferPrice) pageId = (int)SubFormsIds.offerPrice_Sales;
            else if (invoiceTypeId == (int)Enums.DocumentType.OutgoingTransfer) pageId = (int)SubFormsIds.OutgoingTransfer;
            else if (invoiceTypeId == (int)Enums.DocumentType.IncomingTransfer) pageId = (int)SubFormsIds.IncomingTransfer;
            return pageId;
        }
        public static int GetPageEnum(int invoiceTypeId)
        {
            int pageEnum = 0;

            if (invoiceTypeId == (int)Enums.DocumentType.Sales || invoiceTypeId == (int)Enums.DocumentType.ReturnSales) pageEnum = 1;
            //else if (invoiceTypeId == (int)Enums.DocumentType.ReturnSales) pageEnum = 1;
            else if (invoiceTypeId == (int)Enums.DocumentType.Purchase || invoiceTypeId == (int)Enums.DocumentType.ReturnPurchase) pageEnum = 2;
            //else if (invoiceTypeId == (int)Enums.DocumentType.ReturnPurchase) pageEnum = 2;

            else if (invoiceTypeId == (int)Enums.DocumentType.OfferPrice) pageEnum = 3;

            //else if (invoiceTypeId == (int)Enums.DocumentType.DetailedTransactoinsOfItem) pageEnum = 4;
            //else if (invoiceTypeId == (int)Enums.DocumentType.ReviewWarehouseTransfers) pageEnum = 5;
            //else if (invoiceTypeId == (int)Enums.DocumentType.DetailedTransferReport) pageEnum = 6;
            //else if (invoiceTypeId == (int)Enums.DocumentType.DetailsOfSerialTransactions) pageEnum = 7;
            //else if (invoiceTypeId == (int)Enums.DocumentType.GetVatStatmentData) pageEnum = 8;
            return pageEnum;
        }

        public static int GetDocTypeId(int invoiceTypeId) 
        
        { 
            int docTypeId = (int)Enums.PersonAccountDataDocTypeId.invoices;

            if (invoiceTypeId == (int)Enums.DocumentType.AddPermission) docTypeId = (int)Enums.PersonAccountDataDocTypeId.AddPermission;
           else if (invoiceTypeId == (int)Enums.DocumentType.ExtractPermission) docTypeId = (int)Enums.PersonAccountDataDocTypeId.ExtractPermission;
            else if (invoiceTypeId == (int)Enums.DocumentType.itemsFund) docTypeId = (int)Enums.PersonAccountDataDocTypeId.itemsFund;
            else if (invoiceTypeId == (int)Enums.DocumentType.OutgoingTransfer) docTypeId = (int)Enums.PersonAccountDataDocTypeId.OutgoingTransfer;
            else if (invoiceTypeId == (int)Enums.DocumentType.IncomingTransfer) docTypeId = (int)Enums.PersonAccountDataDocTypeId.IncomingTransfer;

            return docTypeId;



        }
    }
}
