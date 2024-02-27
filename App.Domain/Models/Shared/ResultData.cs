using System;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Shared
{
    public class ResponseResult
    {
        public Result Result { get; set; }
#nullable enable
        public object? DataCount { get; set; }
        public object? Data { get; set; }
        public Alart? Alart { get; set; }
        public int? Id { get; set; }
        public int? Code { get; set; }
        public string? Note { get; set; } // notes for front
        public int TotalCount { get; set; } // Count in Table
        public string ErrorMessageAr { get; set; }
        public string ErrorMessageEn { get; set; }
        public double Total { get; set; }
        public string dateTimeNow { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        public int updateNumber { get; set; } = Defults.updateNumber;
        public int isUpdate { get; set; } = Defults.isUpdate;
        public bool isPrint { get; set; }
        public int permissionListId { get; set; }
        public string? employyeNameAr { get; set; }
        public string? employyeNameEn { get; set; }
        public byte[] PosPrintFilesAr { get; set; }
        public byte[] PosPrintFilesEn { get; set; }
        public byte[] ReturnPosPrintFilesAr { get; set; }
        public byte[] ReturnPosPrintFilesEn { get; set; }


    }
    public class Alart
    {
        public AlartType AlartType { get; set; } // alart type is the type of the alart danger,info,worning and success 
        public AlartShow type { get; set; } // alart show for showing alart in notify or popup
        public string titleAr {get;set;}
        public string titleEn {get;set;}
        public string MessageAr {get;set;}
        public string MessageEn {get;set;}
    }
    //    public class ResponseResulttest
    //    {
    //        public Result Result { get; set; }
    //#nullable enable
    //        public byte[] DataFile { get; set; }
    //        public object? Data { get; set; }
    //    }
}
