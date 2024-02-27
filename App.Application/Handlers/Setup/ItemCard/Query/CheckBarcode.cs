using App.Application.Helpers;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Setup.ItemCard.Query
{
    public class CheckBarcode : IRequestHandler<CheckBarcodeRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> _ItemCardMasterQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> _ItemCardUnitQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> _serialQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IBalanceBarcodeProcs _balanceBarcodeProcs;
        private readonly IServiceScopeFactory serviceScopeFactory;
        public CheckBarcode(IRepositoryQuery<InvStpItemCardMaster> ItemCardMasterQuery,
                          IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitQuery,
                          IRepositoryQuery<InvSerialTransaction> serialQuery,
                          IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                          IBalanceBarcodeProcs balanceBarcodeProcs,
                          IServiceScopeFactory serviceScopeFactory)
        {

            this.serviceScopeFactory = serviceScopeFactory;
            _ItemCardMasterQuery = ItemCardMasterQuery;
            _ItemCardUnitQuery = ItemCardUnitQuery;
            _serialQuery = serialQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _balanceBarcodeProcs = balanceBarcodeProcs;
        }


        public async Task<ResponseResult> Handle(CheckBarcodeRequest request, CancellationToken cancellationToken)
        {

            var res = new ResponseResult();

            ICollection<InvStpItemCardMaster> itemExists = new List<InvStpItemCardMaster>();
            ICollection<InvStpItemCardUnit> unitValidation = new List<InvStpItemCardUnit>();
            ICollection<InvSerialTransaction> serialValidation = new List<InvSerialTransaction>();
            var sendCode = request.Barcode != null ? request.Barcode.Trim() : "";
            var code = string.Empty;
            if(request.Barcode != null ){ code = request.Barcode; }
            else if (request.NationalBarcode != null ) { code = request.NationalBarcode; }
            else if (request.ItemCode != null) { code = request.ItemCode; }

            if (code == "")
                return res;
                     
            var checkingCode = await itemCardHelpers.CheckCode(code,request.ItemId, _invGeneralSettingsQuery, _balanceBarcodeProcs, _ItemCardMasterQuery, _ItemCardUnitQuery, _serialQuery);
            if (checkingCode != null)
            {
                return checkingCode;
            }


            if (request.ItemCode != null)
                sendCode = request.ItemCode.Trim();
            else if (request.NationalBarcode != null)
                sendCode = request.NationalBarcode.Trim();

            if (sendCode == "")
            {
                res.Result = Result.RequiredData;
                return res;
            }
            sendCode = sendCode.ToUpper();


            itemExists = await _ItemCardMasterQuery.FindAllAsync(e => (e.ItemCode == sendCode || e.NationalBarcode == sendCode) && !string.IsNullOrEmpty(sendCode) && (request.ItemId > 0 ? e.Id != request.ItemId : 1 == 1));

            var units = _ItemCardUnitQuery.TableNoTracking.Where(a => a.ItemId == request.ItemId && a.UnitId == request.UnitId).Select(a => a.UnitId).ToList();
            unitValidation = await _ItemCardUnitQuery.FindAllAsync(e => e.Barcode == sendCode && e.WillDelete == false && !string.IsNullOrEmpty(sendCode) && (request.ItemId > 0 ? e.ItemId != request.ItemId : 1 == 1) && (units.Count > 0 ? e.UnitId != request.UnitId : 1 == 1));
            // unitValidation = unitValidation.Where(e => e.Barcode == sendCode).ToList();
            // && (request.ItemId > 0 ? e.ItemId != request.ItemId : 1 == 1) && (units.Count>0 ? e.UnitId != request.UnitId : 1==1)

            serialValidation = await _serialQuery.FindAllAsync(e => e.SerialNumber == sendCode && !string.IsNullOrEmpty(sendCode) && (request.ItemId > 0 ? e.ItemId != request.ItemId : 1 == 1));

            if (unitValidation.Any() || itemExists.Any() || serialValidation.Any())
                res.Result = Result.Exist;
            else
                res.Result = Result.NoDataFound;

            return res;

        }

        
    }

}
