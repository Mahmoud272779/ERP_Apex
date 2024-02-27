using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store.Reports.Sales;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Helpers.ReportsHelper
{
    public static class itemsTransaction
    {
        public static async Task<List<itemsSalesData>> ItemsData(UserInformationModel userInfo,IRoundNumbers _roundNumbers, List<InvStpUnits> units, IIncludableQueryable<InvoiceDetails, ICollection<InvStpItemCardUnit>> invoices, int[] branches, bool showDeleted, int? itemId, int[] invoicesTypes, DateTime dateFrom, DateTime dateTo, int? catId, int storeId,int personId, int employeeId = 0, PaymentType paymentType = PaymentType.all, int salesSignal = 1,bool isPurchases = true)
        {
            var res = invoices
                            .Where(x=> isPurchases ? (!userInfo.otherSettings.purchasesShowOtherPersonsInv ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true ) : x.InvoicesMaster.InvoiceTypeId == (int)Enums.DocumentType.POS || x.InvoicesMaster.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS  ? (!userInfo.otherSettings.posShowOtherPersonsInv ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : (!userInfo.otherSettings.salesShowOtherPersonsInv ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true))
                            .Where(x => branches.Contains(x.InvoicesMaster.BranchId))
                            .Where(x => !x.InvoicesMaster.IsDeleted)
                            .Where(x => itemId != 0 ? x.ItemId == itemId : true)
                            .Where(x => invoicesTypes.Contains(x.InvoicesMaster.InvoiceTypeId))
                            .Where(x => catId != 0 ? x.Items.GroupId == catId : true)
                            .Where(x => paymentType != PaymentType.all ? x.InvoicesMaster.PaymentType == (int)paymentType : true)
                            .Where(x => employeeId != 0 ? x.InvoicesMaster.EmployeeId == employeeId : true)
                            .Where(x => storeId != 0 ? x.InvoicesMaster.StoreId == storeId : true)
                            .Where(x=> x.parentItemId == null)
                            .Where(x =>  x.ItemTypeId != (int)ItemTypes.Note)
                            .Where(x => x.InvoicesMaster.InvoiceDate.Date >= dateFrom.Date && x.InvoicesMaster.InvoiceDate.Date <= dateTo.Date)
                            .Where(x=> personId != 0 ? x.InvoicesMaster.PersonId == personId : true)
                            .ToList()
                            .GroupBy(c => new { c.ItemId })
                            .Select(x => new itemsSalesData
                            {
                                itemCode = x.First().Items.ItemCode,
                                arabicName = x.First().Items.ArabicName,
                                latinName = x.First().Items.LatinName,
                                unitArabicName = units.Where(c => c.Id == x.First().Items.ReportUnit).FirstOrDefault()?.ArabicName ?? "",
                                unitLatinName = units.Where(c => c.Id == x.First().Items.ReportUnit).FirstOrDefault()?.LatinName ?? "",

                                qyt = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Quantity(x, x.First().Items.Units.Where(c => c.UnitId == x.First().Items.ReportUnit).First().ConversionFactor, _roundNumbers,isPurchases)),
                                totalPrice = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Total(x,isPurchases)),
                                discount = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Discount(x)) * salesSignal,
                                net = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Total(x) - ReportData<InvoiceDetails>.Discount(x)) * salesSignal,

                                avgOfPrice = Math.Abs(_roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.avgPrice(x, x.First().Items.Units.Where(c => c.UnitId == x.First().Items.ReportUnit).First().ConversionFactor))),
                            })
                            .Where(x => x.qyt != 0)
                            .ToList();
            return res;
        }
    }
}
