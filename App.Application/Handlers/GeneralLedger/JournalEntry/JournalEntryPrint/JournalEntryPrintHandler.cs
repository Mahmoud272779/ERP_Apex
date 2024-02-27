using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryPrint;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class JournalEntryPrintHandler : IRequestHandler<JournalEntryPrintRequest, WebReport>
    {
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;

        public JournalEntryPrintHandler(IMediator mediator, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint)
        {
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }

        public async Task<WebReport> Handle(JournalEntryPrintRequest request, CancellationToken cancellationToken)
        {
            // var allData= getJournalEntryServ
            List<GetJournalEntryByID> MainData = new List<GetJournalEntryByID>();
            List<GetJournalEntryDetailsByID> Listdata = new List<GetJournalEntryDetailsByID>();
            string[] getIds = request.ids.Split(",");
            var data = new GetJournalEntryByID();
            int y = 1;
            foreach (var id in getIds)
            {
                data = await _mediator.Send(new JournalEntryByIdRequest { Id = (Convert.ToInt32(id))});
                data.GroupId = y;
                data.Date = data.FTDate?.ToString("yyyy/MM/dd");
                foreach (var item in data.JournalEntryDetailsDtos)
                {
                    item.GroupId = y;
                }
                MainData.Add(data);
                Listdata.AddRange(data.JournalEntryDetailsDtos);
                y++;
            }

            var userInfo = await _iUserInformation.GetUserInformation();

            var otherdata = new ReportOtherData()
            {


                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };
            var tableNames = new TablesNames()
            {
                FirstListName = "FinancialAccount",
                SecondListName = "FinancialAccountList"
            };
            var report = await _iGeneralPrint.PrintReport<object, GetJournalEntryByID, GetJournalEntryDetailsByID>(null, MainData, Listdata
                , tableNames, otherdata, (int)SubFormsIds.AccountingEntries_GL, request.exportType, request.isArabic,request.fileId);
            return report;
        }
    }
}
