using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public static class PrintHelper
    {
        public static async Task DeleteFileBackground(string path,int DeleteAfterSeconds)
        {
            await Task.Delay((int)TimeSpan.FromSeconds(DeleteAfterSeconds).TotalMilliseconds);
            if (path != null)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
