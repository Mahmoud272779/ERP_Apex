using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices
{
    public interface ICalculationSystemService
    {
        // Calaculation system called in controller
        
        Task<ResponseResult> CalculationOfInvoice(CalculationOfInvoiceParameter parameter);
        Task<InvoiceResultCalculateDto> StartCalculation(CalculationOfInvoiceParameter parameter, bool forReturn);
        Task<InvoiceResultCalculateDto> StartCalculation(CalculationOfInvoiceParameter parameter);
        InvoiceResultCalculateDto StartCalculation( CalculationOfInvoiceParameter parameter, bool AllowRecursive,InvGeneralSettings settings );
        Tuple<double, string, string> CalculateTotalPrice(ref CalculationOfInvoiceParameter parameter);
        ReturnNotes CalculateDiscount(ref CalculationOfInvoiceParameter parameter, double totalPrice, SettingsOfInvoice SettingsOfInvoice, InvGeneralSettings settings);
         bool InvoiceWithoutVAT(int recType);
        void CalculateVat(CalculationOfInvoiceParameter parameter, SettingsOfInvoice SettingsOfInvoice);
        Task<ResponseResult> checkItemPrice(checkItemPrice paramter );
        // ReturnNotes CalculateDiscount(ref PurchaseResultCalculateDto resultTotalDiscountItems,ref CalculationOfInvoiceParameter parameter, double totalPrice, List<InvGeneralSettings> Settings_ , ref ReturnNotes ReturnNotes);
    }
}
