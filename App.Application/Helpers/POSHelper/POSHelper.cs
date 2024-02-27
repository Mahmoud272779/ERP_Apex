using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.POSHelper
{
    public static class POSHelper
    {
        public static bool showOthorInv(int invoiceTypeId, UserInformationModel userInfo)
        {
            if (invoiceTypeId == (int)DocumentType.POS)
                return userInfo.otherSettings.posShowOtherPersonsInv;
            if (invoiceTypeId == (int)DocumentType.Sales)
                return userInfo.otherSettings.salesShowOtherPersonsInv;

            if (invoiceTypeId == (int)DocumentType.Purchase)
                return userInfo.otherSettings.purchasesShowOtherPersonsInv;

            return false;
        }

        public static int Autocode(int BranchId, int invoiceType, IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery)
        {
            var Code = 1;
            Code = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == invoiceType && a.BranchId == BranchId);

            if (Code != null)
                Code++;

            return Code;
        }
    }
}
