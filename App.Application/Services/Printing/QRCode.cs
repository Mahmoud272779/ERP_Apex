using BarcodeStandard;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.IO;
using BarcodeLib;
using SkiaSharp;

namespace App.Application.Services.Printing
{
    public static class QRCode
    {

        public static string GetQRCodeAsString(string CompanyName, string VatNumber, string CreationDate, string Net, string VatTotal)
        {
            string dateFormated = CreationDate;
            string dtmFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
            try
            {
                DateTime dtm = DateTime.Parse(CreationDate);
                dateFormated = dtm.ToString(dtmFormat);

            }
            catch (Exception)
            {
                try
                {
                    DateTime dtm = DateTime.Now;
                    dateFormated = dtm.ToString(dtmFormat);
                }
                catch (Exception)
                {
                }
            }
            string res = "";
            if (CompanyName != null)
                res += GetTLVCode(CompanyName, TLVTags.CompanyName);
            if (VatNumber != null)
                res += GetTLVCode(VatNumber, TLVTags.VatNumber);
            if (dateFormated != null)

                res += GetTLVCode(dateFormated, TLVTags.CreationDate);
            if (Net != null)
                res += GetTLVCode(Net, TLVTags.Net);
            if (VatTotal != null)
                res += GetTLVCode(VatTotal, TLVTags.VatTotal);
            string x = convertToBase64(res);
            return x;
        }
        public static QRCodeReturning GetQRCode(string CompanyName, string VatNumber, string CreationDate, string Net, string VatTotal, IConfiguration _configuration)
        {
            string x = GetQRCodeAsString(CompanyName, VatNumber, CreationDate, Net, VatTotal);

            Bitmap img = GenerateMyQRCode(x);
            MemoryStream ms = new MemoryStream();

            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //save image at path
            var fileName = $"QRCode-{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss").Replace(":", string.Empty)}.png";
            var ImagePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], fileName);
            FileStream fs = new FileStream(ImagePath, FileMode.Create);
            fs.Write(ms.ToArray(), 0, ms.ToArray().Length);
            fs.Close();
            PrintHelper.DeleteFileBackground(ImagePath, 300);
            return new QRCodeReturning { URL = _configuration["ApplicationSetting:FileURL"] + "/" + fileName };

        }

        static string GetTLVCode(string Value, string tag)
        {
            int valLength = Value.Length;
            string result = tag;

            string hexVal = getValueInHex(Value);
            int length = hexVal.Length / 2;
            string hexLength = length.ToString("X2");
            result += hexLength.ToString() + hexVal.ToString();
            return result.ToLower();
        }
        static string getValueInHex(string value)
        {
            string s = "";

            var bytes = Encoding.UTF8.GetBytes(value);
            s = BitConverter.ToString(bytes);
            s = s.Replace("-", "");
            return s;

        }
        static string getLengthOfValue(string value)
        {
            int length = value.Length;
            string res = length.ToString("X");
            if (res.Length == 1)
                res = "0" + res;
            return res;
        }

        static string convertToBase64(string value)
        {
            //string encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            //return encodedStr;
            byte[] arr = Enumerable.Range(0, value.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                .ToArray();
            string str = Convert.ToBase64String(arr);
            return str;
        }
        public static Bitmap GenerateMyQRCode(string QRText)
        {
            MessagingToolkit.QRCode.Codec.QRCodeEncoder encoder = new MessagingToolkit.QRCode.Codec.QRCodeEncoder();
            Bitmap res = encoder.Encode(QRText);
            return res;

        }
        public static string GenerateBarcode(string value, Barcodestander_BarcodeType type, IConfiguration _configuration, string FileName)
        {
            FileName = FileName
                .Replace("*", "`")
                .Replace("/", "`")
                .Replace(@"\", "`")
                .Replace("<", "`")
                .Replace("|", "`")
                .Replace(":", "`")
                .Replace("?", "`")
                .Replace("\"", "`")
                + ".png";
            var ImagePath = Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], FileName);
            if (!File.Exists(ImagePath))
            {
                Barcode b = new Barcode();
                b.IncludeLabel = false;
                b.DisableEan13CountryException = true;
                b.ImageFormat = SKEncodedImageFormat.Png;
                b.ImageFormat = SkiaSharp.SKEncodedImageFormat.Png;
                var img = b.Encode(Enums.BarcodeTypes(type), value, 440, 1);
                var bytes = b.EncodedImageBytes;
                FileStream fs = new FileStream(ImagePath, FileMode.Create);
                fs.Write(bytes.ToArray(), 0, bytes.ToArray().Length);
                fs.Close();
                PrintHelper.DeleteFileBackground(ImagePath, 1000);
            }
            return _configuration["ApplicationSetting:FileURL"] + "/" + FileName;
        }
    }
    class TLVTags
    {
        public static string CompanyName = "01";

        public static string VatNumber = "02";
        public static string CreationDate = "03";
        public static string Net = "04";
        public static string VatTotal = "05";

    }
    public class QRCodeReturning
    {
        public string URL { get; set; }
    }
}
