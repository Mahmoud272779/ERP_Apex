using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvSerialTransaction
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public int ItemId { get; set; }
        public string AddedInvoice { get; set; } // تم ادخالها الى المخزن من فاتورة مشتاريات او اذن اضافةاو مرتجع مبيعات او حذف مبيعات 
        public string ExtractInvoice { get; set; } // تم اخراجها من المخزن من فاتورة مبيعات او نقاط البيع او مرتجع مشتريات
        public int indexOfSerialForAdd { get; set; } // لو دخلت نفس الصنف ف نفس الفاتورة يحدد كل سيريال تبع اى ريكورد مثلا ف المشتريات
        public int indexOfSerialForExtract { get; set; } // لو دخلت نفس الصنف ف نفس الفاتورة يحدد كل سيريال تبع اى ريكورد مثلا في المبيعات مرتجع المشتريات
        public int StoreId { get; set; }
        public bool IsAccridited { get; set; }
        public bool IsDeleted { get; set; }
        public int TransferStatus { get; set; }
        public virtual InvStpItemCardMaster Items { get; set; }
      //  public virtual InvoiceMaster InvoicesMaster { get; set; }
        public virtual InvStpStores Stores { get; set; }
    }
}
