using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Service_helper.FileHandler
{
    public interface IFileHandler
    {
        string SaveImage(IFormFile image, string folderName, bool resize);
        string DeleteImage(List<string> FilesList);
        string DeleteImage(string path);
        string UploadFile(IFormFile file,string folderName);
        string UploadFile(IFormFile file,string folderName, string fileName);
        Tuple<string, string,string> CreateInvoiceForPrint(byte[] file, string fileName, string fileExt);
        Tuple<string,string> folderExist(string path);
    }
}
