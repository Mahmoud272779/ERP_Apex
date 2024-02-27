using App.Application.Handlers.Invoices.POS.GetItemUnitsForPOS;
using App.Application.Handlers.Units;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities.Setup;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.ItemCard.GetItemsByDate
{
    public class GetItemsByDateHandler : IRequestHandler<GetItemsByDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemCardRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemCardUnitsQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetItemsByDateHandler(IRepositoryQuery<InvStpItemCardMaster> ItemCardRepositoryQuery 
            , IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitsQuery
            , IGeneralAPIsService generalAPIsService)
        {
            itemCardRepositoryQuery = ItemCardRepositoryQuery;
            itemCardUnitsQuery = ItemCardUnitsQuery;
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<ResponseResult> Handle(GetItemsByDateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var resData = await itemCardRepositoryQuery.TableNoTracking
                    .Include(a => a.Units)
                    .Where(a => a.UTime >= request._date)
                .ToListAsync();

                return await generalAPIsService.Pagination(resData, request.PageNumber, request.PageSize);


            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}
