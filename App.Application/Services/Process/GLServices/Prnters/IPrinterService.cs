using App.Domain.Models.Request.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.GLServices.Prnters
{
    public interface IPrinterService
    {
        Task<ResponseResult> AddPrinter(PrinterRequestsDTOs.Add parameter);
        Task<ResponseResult> GetPrinterById(int Id);
        Task<ResponseResult> UpdatePrinter(PrinterRequestsDTOs.Update parameter);
        Task<ResponseResult> DeletePrinter(SharedRequestDTOs.Delete parameter);
        Task<ResponseResult> GetAllPrinterData(PrinterRequestsDTOs.Search paramters, bool isPrint = false);
        Task<ResponseResult> GetAllPrinterHistory(int PrinterId);
        Task<ResponseResult> GetPrinterByDate(DateTime date, int PageNumber, int PageSize);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter);
        Task<ResponseResult> GetAllPrinterByBranchDataDropDown(int BranchId);
        //Task<ResponseResult> GetAllPrinterDataDropDown();

    }
}
