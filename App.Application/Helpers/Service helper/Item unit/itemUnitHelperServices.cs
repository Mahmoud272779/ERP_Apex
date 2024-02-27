using App.Application.Helpers.Service_helper.Item_unit;
using App.Domain.Entities.Setup;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers
{
    public class itemUnitHelperServices: IitemUnitHelperServices
    {
        private readonly IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitQuery;

        public itemUnitHelperServices(IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitQuery)
        {
            this.ItemCardUnitQuery = ItemCardUnitQuery;
        }

        public  async Task<RptUnitDataResponse> getRptUnitData(int itemId)
        {
            var itemUnit = ItemCardUnitQuery.TableNoTracking.Where(a => a.ItemId == itemId && a.Item.ReportUnit == a.UnitId)
                                .Include(a => a.Item)
                            .Select(a => new { a.Item.ReportUnit, a.ConversionFactor, a.Unit.ArabicName, a.Unit.LatinName });

            var resData = new RptUnitDataResponse();
            resData.rptUnit = itemUnit.Select(a => a.ReportUnit).Sum();
            resData.rptFactor = itemUnit.Select(a => a.ConversionFactor).Sum();
           resData.rptUnitAR = itemUnit.Select(a => a.ArabicName).First();
           resData.rptUnitEn = itemUnit.Select(a => a.LatinName).First();
            return resData;
        }
    }

    public class RptUnitDataResponse
        {
        public int? rptUnit { get; set; }
        public double rptFactor { get; set; }
        public string rptUnitAR { get; set; }
        public string rptUnitEn { get; set; }
    }
}
