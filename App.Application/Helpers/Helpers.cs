using App.Domain.Entities.Setup;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Remoting;

namespace App.Application.Helpers
{
    public static class Helpers
    {
        //public static object Helpers { get; internal set; }

        public static Image Resize(this Image image, int width, int height)
        {
            var res = new Bitmap(width, height);
            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, width, height);
            }
            return res;
        }
        public static int? ToNullableInt(this string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }
        public static string IsNullString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Trim();
        }
        public static byte[] FileToByteArray(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static object GetInstance(string strFullyQualifiedName)
        {

            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }
        public static double ConvertQyt(int itemId, int ReportUnitId, double qyt, List<InvStpItemCardMaster> items)
        {
            var item = items.Where(x => x.Id == itemId);
            InvStpItemCardUnit ItemUnit = new InvStpItemCardUnit();
            if (!item.Any())
                return 0;

            var FirstUnitOfitem = item.FirstOrDefault().Units.FirstOrDefault().ConversionFactor;
            var QytWithFirstUnit = qyt * FirstUnitOfitem;
            var ConversionFactorForReportUnit = item.FirstOrDefault().Units.Where(x => x.UnitId == ReportUnitId).FirstOrDefault().ConversionFactor;
            var FinalQyt = QytWithFirstUnit / ConversionFactorForReportUnit;

            return FinalQyt;
        }

        public static string ConnectionString(IConfiguration _configuration, string dbName)
        {
            string connection = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                $"Initial Catalog={dbName};" +
                                $"user id={_configuration["ApplicationSetting:UID"]};" +
                                $"password={_configuration["ApplicationSetting:Password"]};" +
                                $"MultipleActiveResultSets=true;";
            return connection;
        }

        public static string generateSecretCode()
        {
            var key = StringEncryption.EncryptString(defultData.userManagmentApplicationSecurityKey + DateTime.Now.ToString("yyyyMM-dd"));
            return key;
        }
    }
}
