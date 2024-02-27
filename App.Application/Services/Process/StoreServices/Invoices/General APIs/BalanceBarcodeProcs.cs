using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public class BalanceBarcodeProcs: IBalanceBarcodeProcs
    {
        public IRoundNumbers RoundNumbers { get; }

        public BalanceBarcodeProcs(IRoundNumbers roundNumbers)
        {
            RoundNumbers = roundNumbers;
        }
     

        public BalanceBarcodeResult getItem(BalanceBarcodeInput input)
        {
            //double n;  
            //if (!double.TryParse(input.FullCode, out n))
            //    return null;
            if (input.FullCode.Length != input.SbarcodeLength)
                return null;
            BalanceBarcodeResult res = new BalanceBarcodeResult();
            res.ItemCode = getitemcode(input);
            var wightcode = getQty(input);
            if (!int.TryParse(wightcode, out int number))
                return null;
            res.Qty =double.Parse(wightcode);

            return res;
        }

        private string getitemcode(BalanceBarcodeInput input)
        {
            if (input.FullCode.Length < input.ItemCodestart + input.ItemCodeLength)
                return "";
            int extraLength = input.ItemCodestart == 0 ? 1 : 0;
            string code = input.FullCode.Substring(input.ItemCodestart, (input.ItemCodeLength + extraLength));
            return code;
        }
        private string getQty(BalanceBarcodeInput input)
        {
            string weightcode = "";
            int start = 1;
         
            int length = input.ItemCodeLength;
            int TotalLength = input.SbarcodeLength;
            for (var k = (start + length); k <= TotalLength - 2; k++)
                weightcode += input.FullCode[k];
            return weightcode;
        }
        public resultQuantity GetItemCost(double price, double num,string BarcodeType)
        {
            if (price < 0) return null; 
            resultQuantity result= new resultQuantity();
            if (BarcodeType== ScalbarcodeType.Weight)
            {
                result.QTY =Math.Round( (num / 1000),(int)generalEnum.BalanceBarcodeDecimal);
                result.ItemCost = RoundNumbers.GetRoundNumber((price * result.QTY));
            }
            else if(BarcodeType == ScalbarcodeType.Cost)
            {
                result.ItemCost = RoundNumbers.GetRoundNumber((num / 100));
                result.QTY =Math.Round ((result.ItemCost/ price), (int)generalEnum.BalanceBarcodeDecimal) ;
            }
            
            return result;
        }
    }
    public class BalanceBarcodeResult
    {
        public string ItemCode;

        public double Qty;
    }
    public class resultQuantity
    {
        public double QTY;

        public double ItemCost;
    }

    public  class BalanceBarcodeInput
    {
        public string FullCode; // باركود الميزان كامل 
        public int ItemCodestart; // بداية كود الصنف
        public int ItemCodeLength= barcodeLength.ItemcodeLength; // عدد حروف كود الصنف
        public int SbarcodeLength= barcodeLength.ScalBarcodeLength;
    }

}


 
