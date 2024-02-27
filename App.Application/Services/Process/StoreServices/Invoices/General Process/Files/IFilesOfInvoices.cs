using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IFilesOfInvoices
    {
         Task<bool> saveFilesOfInvoices(IFormFile[] AttachedFile, int BranchId, string fileDirectory, int InvoiceId, bool isUpdate ,List<int>? FileId, bool isReceipt);
        void RemoveOldFilesForUpdate( int InvoiceId, List<int> FileId);
    }
}
