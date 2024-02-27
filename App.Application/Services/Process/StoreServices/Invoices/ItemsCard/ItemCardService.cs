using App.Application.Handlers.Setup.ItemCard;
using App.Application.Handlers.Setup.ItemCard.Query;
using App.Application.Handlers.Units;
using App.Application.Services.Printing.InvoicePrint;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Setup.ItemCard.Request;
using AutoMapper;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Application.Services.Process.StoreServices.Invoices.ItemsCard
{

    public class ItemCardService : IItemCardService
    {
        private readonly IMediator _mediator;
        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IMapper _mapper;
        private readonly IGeneralPrint _iGeneralPrint;


        public ItemCardService(IMediator mediator, IPrintService iprintService, IFilesMangerService filesMangerService, ICompanyDataService companyDataService, iUserInformation iUserInformation, IMapper mapper, IGeneralPrint iGeneralPrint)
        {
            _mediator = mediator;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _mapper = mapper;
            _iGeneralPrint = iGeneralPrint;
        }
        public async Task<WebReport> ItemCardPrint(GetAllItemCardRequest parameter, exportType exportType, bool isArabic,int fileId=0)
        {
            parameter.isPrint = true;
            var data = await _mediator.Send(parameter);
           
            var userInfo = await _iUserInformation.GetUserInformation();



            var retunedData = (List<InvStpItemCardMaster>)data.Data;
            
            var mainData = _mapper.Map<List<ItemCardMainData>>(retunedData);
            
            
            var otherdata = new ReportOtherData()
            {
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };

            int screenId = (int)SubFormsIds.ItemCard_MainUnits;

            var tablesNames = new TablesNames()
            {
                FirstListName = "ItemCard"
            };
            var report = await _iGeneralPrint.PrintReport<object, ItemCardMainData, object>(null, mainData, null, tablesNames, otherdata
                , screenId, exportType, isArabic,fileId);
            return report;
            
        }

        public async Task<ResponseResult> GetItemsByDate(DateTime date, int PageNumber, int PageSize)
        {
            return await _mediator.Send(new GetItemsByDateRequest(date,  PageNumber,  PageSize ));
        }
    }
}
