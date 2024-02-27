using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure
{
    public class Numbers
    {
        public static double Roundedvalues(double value, int MidpointRounding)
        {
            //return number.ToString("N" + NFP);
            int Num;
            if (MidpointRounding == 0)
                Num = MidpointRounding;
            else
                Num = MidpointRounding;


            string res = string.Format("{0:F7}", value);
            //string res =  Convert .ToString(number);

            if (Num == 0)
            {
                string[] arr = res.Split('.');
                res = arr[0];
                return double.Parse(res);
            }
            int index = res.IndexOf('.');
            if (index == -1)
                return double.Parse(res);
            if (res.Length > index + Num)
            {
                int start = index + Num + 1;
                int count = res.Length - start;
                res = res.Remove(start, count);
            }
            // res = Math.Round(number, NFP).ToString();
            if (res.Contains('.'))
            {
                string[] arr = res.Split('.');
                string floatstr = arr[1];
                if (floatstr.Length == 1)
                    floatstr += "0";
                res = arr[0] + "." + floatstr;
            }
            return double.Parse(res);
        }
        public static double RoundedSimplevalues(double value, int decemalNum)
        {
            int Number = 0;
            for (int i = 0; i < decemalNum; i++)
            {
               Number= Number * 10;
            }
            double ValueAfterMultiplicaion = value * Number;
            double valueTrancate = Math.Truncate(ValueAfterMultiplicaion);

            double result = valueTrancate / Number;
            return result;
        }
        public static double RoundedUp(double value, int decemalNum)
        {
            return Math.Round(value, decemalNum);
        }

        public static double RoundedDown(double value, int decemalNum)
        {
            double val= Math.Round(value,decemalNum,MidpointRounding.AwayFromZero);//up
            double val1= Math.Round(value,decemalNum,MidpointRounding.ToEven);//up
            double val2= Math.Round(value,decemalNum,MidpointRounding.ToZero);//
            double val3= Math.Round(value,decemalNum,MidpointRounding.ToNegativeInfinity);//
            double val4= Math.Round(value,decemalNum,MidpointRounding.ToPositiveInfinity);//up
            return 0.0;
        }


        public static double ConvertQyt(int itemId, int ReportUnitId, double qyt,double ConversionFactor, List<InvStpItemCardMaster> items)
        {
            var item = items.Where(x => x.Id == itemId);
            InvStpItemCardUnit ItemUnit = new InvStpItemCardUnit();
            if (!item.Any())
                return 0;

            //var FirstUnitOfitem = item.FirstOrDefault().Units.FirstOrDefault().ConversionFactor;
            var QytWithFirstUnit = qyt * ConversionFactor;
            var ConversionFactorForReportUnit = item.FirstOrDefault().Units.Where(x => x.UnitId == ReportUnitId).FirstOrDefault().ConversionFactor;
            var FinalQyt = QytWithFirstUnit / ConversionFactorForReportUnit;

            return FinalQyt;
        }
        public static double ConvertPrice(int itemId, int ReportUnitId, double price,double ConversionFactor, List<InvStpItemCardMaster> items)
        {
            var item = items.Where(x => x.Id == itemId);
            InvStpItemCardUnit ItemUnit = new InvStpItemCardUnit();
            if (!item.Any())
                return 0;

            return (price * ConversionFactor) / item.FirstOrDefault().Units.Where(c=> c.UnitId == ReportUnitId).FirstOrDefault().ConversionFactor;
            
        }
    }
}
