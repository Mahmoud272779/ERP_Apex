using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public class PosService: IPosService
    {
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;

        public PosService(IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            this.itemUnitsQuery = itemUnitsQuery;
        }
        public async Task<ResponseResult> getItemUnitsForPOS(List<int> itemIds)
        {
            if(itemIds.Count()==0)
            {
                return new ResponseResult() { Result=Result.RequiredData,ErrorMessageAr=ErrorMessagesAr.enterItemId,ErrorMessageEn=ErrorMessagesEn.enterItemId};
            }

            var unitsDto = itemUnitsQuery.TableNoTracking.Include(a => a.Unit).Where(a => itemIds.Contains( a.ItemId))
                       .Select(a => new ItemUnitsDto() {ItemId=a.ItemId, UnitId= a.UnitId,ArabicName= a.Unit.ArabicName,
                                   LatinName=a.Unit.LatinName,ConversionFactor= a.ConversionFactor,SalePrice1 =a.SalePrice1 });

            return new ResponseResult() { Data = unitsDto, Result = (unitsDto.Count() > 0 ? Result.Success : Result.NotExist),
                ErrorMessageAr = (unitsDto.Count() > 0 ? "": ErrorMessagesAr.ItemNotExist),
                ErrorMessageEn= (unitsDto.Count() > 0 ? "": ErrorMessagesEn.ItemNotExist)};
        }
    }
}
