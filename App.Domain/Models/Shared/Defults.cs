using App.Application.Handlers.GeneralAPIsHandler.GetMobileAppVersion;
using App.Domain.Models.Setup.ItemCard.Response;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Shared
{
    public static class Defults
    { 
        public static string defultUpdateNumber = "16";
        private static int GetUpdateNumber(bool isPrint = false)
        {
            //check if File Exist
            
            var path = Path.Combine(Environment.CurrentDirectory, "wwwroot",isPrint? "UpdateFilesNumber.txt" : "updateNumber.txt");
            var isFileExist = File.Exists(path);
            if (!isFileExist)
            {
                File.Create(path).Close();
                File.WriteAllText(path, defultUpdateNumber);
            }
            var fileValue = File.ReadAllText(path);
            var tryParse = int.TryParse(fileValue, out var value);
            if (!tryParse)
            {
                File.WriteAllText(path, defultUpdateNumber);
                return int.Parse(defultUpdateNumber);
            }
            return int.Parse(fileValue);

        }
        private static int GetisUpdate(bool isPrint = false)
        {
            //check if File Exist
            
            var path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "isUpdate.txt");
            var isFileExist = File.Exists(path);
            if (!isFileExist)
            {
                File.Create(path).Close();
                File.WriteAllText(path, "1");
            }
            var fileValue = File.ReadAllText(path);
            var tryParse = int.TryParse(fileValue, out var value);
            if (!tryParse)
            {
                File.WriteAllText(path, "1");
                return int.Parse(defultUpdateNumber);
            }
            return int.Parse(fileValue);

        }
        public static string GetOfflineVersion()
        {
            string versionNumber = "";
            var path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "offlineVersion.txt");
            if(File.Exists(path))
            {
                versionNumber = File.ReadAllText(path);
            }
            else { 
                File.Create(path).Close();
                File.WriteAllText(path, "1.0.0");
                versionNumber = File.ReadAllText(path);
            }
            return versionNumber;
        }
        public static ResponseResult GetMobileAppVersion()
        {

            MobileAppVersionDTO data = new MobileAppVersionDTO();

            var path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "MobileAppVersion.txt");
            if (File.Exists(path))
            {
                string[] existingLines = File.ReadAllLines(path);
                if (existingLines.Length == 0)
                {
                    data.VersionNumber = "1.0.0";
                    data.AndroidPath = "https://www.google.com/";
                    data.iOSPath = "https://outlook.live.com/mail/0/";

                    List<string> lines = new List<string>();
                    lines.Insert(0, data.VersionNumber);
                    lines.Insert(1, data.AndroidPath);
                    lines.Insert(2, data.iOSPath);


                    File.WriteAllLines(path, lines);
                }
                else
                {
                    data.VersionNumber = File.ReadLines(path).First();
                    data.AndroidPath = File.ReadLines(path).ElementAt(1);
                    data.iOSPath = File.ReadLines(path).ElementAt(2);
                }   
            }
            else
            {
                File.Create(path).Close();

                data.VersionNumber = "1.0.0";
                data.AndroidPath = "https://www.google.com/";
                data.iOSPath = "https://outlook.live.com/mail/0/";

                List<string> lines = new List<string>();
                lines.Insert(0, data.VersionNumber);
                lines.Insert(1, data.AndroidPath);
                lines.Insert(2, data.iOSPath);


                File.WriteAllLines(path, lines);

            }
            return new ResponseResult()
            {
                Data = data,
                Result = Result.Success,
            };
        }
        public static int updateNumber
        {
            get { return GetUpdateNumber(); }
        }
        public static int isUpdate
        {
            get { return GetisUpdate(); }
        }
        public static int UpdateFilesNumber
        {
            get { return GetUpdateNumber(true); }
        }
        public static string ChangeUpdateNumberValuePassword = "(*^&*^*^HGJBBjhvdashdvashRTYRt1r231tr21KVJBVBJ";
    }
}
