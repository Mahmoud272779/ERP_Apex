using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Enums
{
   public class BarcodeEnums
    {
        public enum BarcodeItemType
        {
            Container,
            Barcode ,
            Text,
            Image,
            Logo
        }

        public enum TextType
        {
            Fixed=1,
            Variable
        }

        public enum BarcodeType
        {
            Code128 = 1,
            Code39,
            Interleaved2of5,
            EAN13,
            UPC_A
        }

        public enum VariableContent //المحتوي المتغير
        {
            CompanyName =1,
            Mobile =2,
            Phone=3,
            Website=4,
            ItemCode=5,
            ItemName_ar=6,
            ItemName_en=7,
            Price=8,
            PriceWithVat=9,
            Category=10,
            ExpireDate=11
        }
    }
}
