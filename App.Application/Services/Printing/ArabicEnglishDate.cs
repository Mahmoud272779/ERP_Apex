using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing
{
     public static class ArabicEnglishDate
    {

        public static   ReportOtherData OtherDataWithDatesArEn(bool isArabic,DateTime dateFrom,DateTime dateTo)
        {
            ReportOtherData reportOtherData = new ReportOtherData();

            if (isArabic)
            {
                string arFormat = "yyyy/MM/dd";
                reportOtherData.Date = DateTime.Now.ToString(arFormat + " HH:mm:ss");
                reportOtherData.DateFrom = dateFrom.ToString(arFormat);
                reportOtherData.DateTo = dateTo.ToString(arFormat);
            }
            else
            {
                string enFormat = "dd/MM/yyyy";
                reportOtherData.Date = DateTime.Now.ToString(enFormat + " HH:mm:ss");
                reportOtherData.DateFrom = dateFrom.ToString(enFormat);
                reportOtherData.DateTo = dateTo.ToString(enFormat);
            }
            return reportOtherData;
        }
    }
}
