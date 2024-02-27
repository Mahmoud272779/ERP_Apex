using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace App.Application.Helpers.Service_helper.FileHandler
{
    public class FileHandler:IFileHandler
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        private static IHttpContextAccessor httpContextAccessor;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly iUserInformation Userinformation;

        public FileHandler(
                        IWebHostEnvironment hostingEnvironment, 
                        IConfiguration configuration,
                        IHttpContextAccessor _httpContextAccessor,
                        ISecurityIntegrationService securityIntegrationService,
                        iUserInformation Userinformation)
        {
            //Injecting the configuration  and the webhostenvironment objects
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            httpContextAccessor = _httpContextAccessor;
            _securityIntegrationService = securityIntegrationService;
            this.Userinformation = Userinformation;
        }

        public string DeleteImage(string path)
        {
            var imageList = new List<string>();
            imageList.Add(path);
            return DeleteImage(imageList);
        }
        public string DeleteImage(List<string> FilesList)
        {
            var pathOfFile = "";
            var URL = configuration["ApplicationSetting:FileURL"];
            string serverURL = URL;
            foreach(var path in FilesList)
            {
                if (path != null)
                {
                    var fileName = path.Replace(serverURL, "");
                    string webRootPath = configuration["ApplicationSetting:FilesRootPath"];
                    //var fileName = file;
                    var fullPath = Path.Combine(webRootPath, fileName);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        pathOfFile = null;
                    }

                }
                else
                    pathOfFile = path;
            }
          
            return pathOfFile;
        }

        public string SaveImage(IFormFile image, string folderName, bool resize )
        {
            var URL = configuration["ApplicationSetting:FileURL"];
            string serverURL = URL;
            var path = configuration["ApplicationSetting:FilesRootPath"];
            var databaseName = contextHelper.GetDBName(httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result);
            if (!string.IsNullOrEmpty(databaseName))
                databaseName = StringEncryption.DecryptString(databaseName);
            path = Path.Combine(path, databaseName);
            if (!Directory.Exists(Path.Combine(path, folderName)))
            {
                Directory.CreateDirectory(Path.Combine(path, folderName));
            }
            path = Path.Combine(path, folderName);

            var imageName = DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + image.FileName.Replace(" ", "");
            string filePath = Path.Combine(path, imageName);
            string actulePath = Path.Combine(path, filePath);
            using (MemoryStream ms = new MemoryStream())
            {
                image.CopyTo(ms);
                if (resize)
                {
                    Image imgResized = Helpers.Resize(Image.FromStream(ms), 250, 250);
                    imgResized.Save(actulePath);
                }
                else
                {
                    Image img = Image.FromStream(ms);
                    img.Save(actulePath);
                }
                var folderPath = Path.Combine(databaseName, folderName);
                var imagePath = Path.Combine(folderPath, imageName);
                string imagePathCloud =Path.Combine(serverURL, imagePath);
                return imagePathCloud;
            }
        }

        public string UploadFile(IFormFile file, string folderName )
        {
            return UploadFile(file, folderName, "");
        }
        public string UploadFile(IFormFile file, string folderName, string fileName)
        {
            string serverURL = configuration["ApplicationSetting:FileURL"];

            var path = configuration["ApplicationSetting:FilesRootPath"];
            if (!Directory.Exists(Path.Combine(path, folderName)))
            {
                Directory.CreateDirectory(Path.Combine(path, folderName));
            }
            if (fileName == "")
                fileName = DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + file.FileName.Replace(" ", "");
            string filePath = Path.Combine(folderName, fileName);
            string actulePath = Path.Combine(path, filePath);
            using (FileStream fs = File.Create(actulePath))
            {
                file.CopyTo(fs);
                fs.Flush();
                string imagePath = Path.Combine(serverURL, filePath);
                return imagePath;
            }

        }

        public  Tuple<string,string>  folderExist(string path)
        {
            var webRoot = configuration["ApplicationSetting:FilesRootPath"]  /*hostingEnvironment.WebRootPath*/;
            UserInformationModel userInfo = Userinformation.GetUserInformation().Result;
            var token = httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;

         
            var dbName = StringEncryption.DecryptString( contextHelper.GetDBName(token.ToString()));
          //  string[] Paths = { webRoot, dbName, path };
            var fullPath = Path.Combine(dbName, path);
            var pathArr = fullPath.Split('\\');

            var subPath = webRoot;
            var pathName = "";
            foreach (var folder in pathArr)
            {
                subPath = Path.Combine(subPath, folder);
                pathName = Path.Combine(pathName, folder);
                if (!Directory.Exists(subPath))
                    Directory.CreateDirectory(subPath);

              
            }
            return new  Tuple<string,string> (webRoot, pathName) ;
        }

        public Tuple<string, string,string> CreateInvoiceForPrint(byte[] file,string fileName,string fileExt)
        {
            
            var webRoot = configuration["ApplicationSetting:FilesRootPath"]  /*hostingEnvironment.WebRootPath*/;
            var GetFileName = $"{fileName}.{fileExt}";
            var fullPath = Path.Combine(webRoot, GetFileName);
            var pathName = "";
            FileStream fs = new FileStream(fullPath,FileMode.Create);
            fs.Write(file, 0, file.Length);
            fs.Close();
            var webRootFile = Path.Combine(configuration["ApplicationSetting:FileURL"], GetFileName);
            return new Tuple<string, string,string>(webRootFile, fullPath, $"{fileName}.{fileExt}");
        }
    }
}
