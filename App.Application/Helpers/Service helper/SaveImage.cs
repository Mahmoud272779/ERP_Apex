using App.Infrastructure.EmailService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Service_helper
{
   public  class SaveImage
    {
      
        public static   string saveImage(IFormFile img , string FolderName , bool Resize , IHostingEnvironment hostingEnvironment)
        {
            var path = hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(FolderName, DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + img.FileName.Replace(" ", ""));
            string actulePath = Path.Combine(path, filePath);
            using (MemoryStream ms = new MemoryStream())
            {
                img.CopyTo(ms);
                if(Resize)
                {
                    Image imgResized = Helpers.Resize(Image.FromStream(ms), 250, 250);
                    imgResized.Save(actulePath);
                }
                else
                {

                Image image = Image.FromStream(ms);
                image.Save(actulePath);
                }
               
                string imagePath = Constants.LocalServer + filePath;
               return imagePath;
            }
        }
    }
}
