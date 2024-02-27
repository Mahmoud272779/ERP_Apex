using App.Application.Basic_Process;
using App.Application.Helpers.Service_helper.ApplicationSettingsHandler;
using App.Domain.Entities.Common;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.JournalEntryDrafts
{
    public class JournalEntryDraftBusiness : BusinessBase<GLJournalEntryDraft>, IJournalEntryDraftBusiness
    {
        private readonly IRepositoryQuery<GLJournalEntryDraft> journalEntryDraftRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntryDraft> journalEntryDraftRepositoryCommand;
        private readonly IRepositoryCommand<GLJournalEntryDraftDetails> journalEntryDetailsDraftRepositoryCommand;
        private readonly IRepositoryQuery<GLJournalEntryDraftDetails> journalEntryDetailsDraftRepositoryQuery;
        private readonly IPagedList<JournalEntryDto, JournalEntryDto> pagedListJournalEntryDto;
        private readonly IInvGeneralSettingsHandler invGeneralSettingsHandler;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntryFilesDraft> journalEntryFilesDraftRepositoryCommand;
        private readonly IRepositoryQuery<GLJournalEntryFilesDraft> journalEntryFilesDraftRepositoryQuery;

        public JournalEntryDraftBusiness(
            IRepositoryQuery<GLJournalEntryDraft> JournalEntryDraftRepositoryQuery,
            IRepositoryCommand<GLJournalEntryDraft> JournalEntryDraftRepositoryCommand,
            IRepositoryCommand<GLJournalEntryDraftDetails> JournalEntryDetailsDraftRepositoryCommand,
            IRepositoryQuery<GLJournalEntryDraftDetails> JournalEntryDetailsDraftRepositoryQuery,
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryCommand<GLJournalEntryFilesDraft> JournalEntryFilesDraftRepositoryCommand,
            IRepositoryQuery<GLJournalEntryFilesDraft> JournalEntryFilesDraftRepositoryQuery,
            IRepositoryQuery<GLCostCenter> CostCenterRepositoryQuery,
            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
            IRepositoryQuery<GLBranch> BranchRepositoryQuery,
            IHostingEnvironment hostingEnvironment,
             IPagedList<JournalEntryDto, JournalEntryDto> PagedListJournalEntryDto,
             IInvGeneralSettingsHandler invGeneralSettingsHandler,
            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            journalEntryDraftRepositoryQuery = JournalEntryDraftRepositoryQuery;
            _hostingEnvironment = hostingEnvironment;
            journalEntryDraftRepositoryCommand = JournalEntryDraftRepositoryCommand;
            journalEntryDetailsDraftRepositoryCommand = JournalEntryDetailsDraftRepositoryCommand;
            journalEntryDetailsDraftRepositoryQuery = JournalEntryDetailsDraftRepositoryQuery;
            pagedListJournalEntryDto = PagedListJournalEntryDto;
            this.invGeneralSettingsHandler = invGeneralSettingsHandler;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            journalEntryFilesDraftRepositoryCommand = JournalEntryFilesDraftRepositoryCommand;
            journalEntryFilesDraftRepositoryQuery = JournalEntryFilesDraftRepositoryQuery;
            currencyRepositoryQuery = CurrencyRepositoryQuery;
            costCenterRepositoryQuery = CostCenterRepositoryQuery;
            branchRepositoryQuery = BranchRepositoryQuery;
        }
        public async Task<bool> CheckIsValidCode(int Code)
        {
            //var loginUser = await _accountService.GetUser();
            var journalEntry = await journalEntryDraftRepositoryQuery.SingleOrDefault(
                   cust => cust.Code == Code);

            return journalEntry == null ? false : true;

        }
        public async Task<string> AddAutomaticCodeDraft()
        {
            var code = journalEntryDraftRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = journalEntryDraftRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
                int codee = (Convert.ToInt32(code2.Code));
                if (codee == 0)
                {
                }
                var NewCode = codee + 1;
                return NewCode.ToString();

            }
            else
            {
                var NewCode = 1;
                return NewCode.ToString();

            }
        }
        public async Task<IRepositoryActionResult> AddJournalEntryDraft(JournalEntryParameter parameter)
        {
            try
            {
                var list = new List<JournalEntryParameter>();
                var table = Mapping.Mapper.Map<JournalEntryParameter, GLJournalEntryDraft>(parameter);
                journalEntryDraftRepositoryCommand.Add(table);

                foreach (var item in parameter.JournalEntryDetails)
                {
                    var journalDetails = new GLJournalEntryDraftDetails()
                    {
                        CostCenterId = item.CostCenterId,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        DescriptionAr = item.DescriptionAr,
                        DescriptionEn = item.DescriptionEn,
                        FinancialAccountId = item.FinancialAccountId,
                        JournalEntryDraftId = table.Id
                    };
                    journalEntryDetailsDraftRepositoryCommand.Add(journalDetails);
                    table.CreditTotal += journalDetails.Credit;
                    table.DebitTotal += journalDetails.Debit;
                }
                if (table.CreditTotal != table.DebitTotal)
                {
                    await journalEntryDetailsDraftRepositoryCommand.DeleteAsync(q => q.JournalEntryDraftId == table.Id);
                    await journalEntryDraftRepositoryCommand.DeleteAsync(table.Id);
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant add this journal entry");
                }
                var img = parameter.AttachedFiles;
                var ListjournalDetailss = new List<GLJournalEntryFilesDraft>();
                if (img != null)
                foreach (var item in img)
                {
                    var journalDetailss = new GLJournalEntryFilesDraft();
                    journalDetailss.JournalEntryDraftId = table.Id;
                    var path = _hostingEnvironment.WebRootPath;
                    string filePath = Path.Combine("JournalEntry\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + item.FileName.Replace(" ", ""));
                    string actulePath = Path.Combine(path, filePath);
                    using (var fileStream = new FileStream(actulePath, FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    journalDetailss.File = filePath;
                    ListjournalDetailss.Add(journalDetailss);
                }
                journalEntryFilesDraftRepositoryCommand.AddRange(ListjournalDetailss);
                await journalEntryFilesDraftRepositoryCommand.SaveAsync();
                await journalEntryDraftRepositoryCommand.SaveAsync();
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
            //return  new Task<bool>(() => true);
        }
        public async Task<IRepositoryActionResult> GetJournalEntryDraft()
        {

            var doubleStringFormat = await invGeneralSettingsHandler.GetDoubleFormat();
            var list = new List<JournalEntryDto>();
            var journalEntry = journalEntryDraftRepositoryQuery.FindAll(q => q.Id > 0 && q.IsBlock == false);
            foreach (var item in journalEntry)
            {
                var entry = new JournalEntryDto()
                {
                   Id = item.Id,
                   Code = item.Code,
                   Name = item.Name,
                   CreditTotal = item.CreditTotal,
                   DebitTotal = item.DebitTotal,
                   CurrencyId = item.CurrencyId,
                   FTDate = item.FTDate,
                   Notes = item.Notes,
                   IsBlock = item.IsBlock,
                   IsDraft = true
                };
              
                var entryDetails = journalEntryDetailsDraftRepositoryQuery.FindAll(q => q.JournalEntryDraftId == entry.Id);
                foreach (var item2 in entryDetails)
                {
                    var entryDetailsDto = new JournalEntryDetailsDto()
                    {
                        Id = item2.Id,
                        FinancialAccountId = item2.FinancialAccountId,
                        JournalEntryId = item2.JournalEntryDraftId,
                        DescriptionAr = item2.DescriptionAr,
                        DescriptionEn = item2.DescriptionEn,
                        CostCenterId = item2.CostCenterId,
                        Credit = item2.Credit,
                        Debit = item2.Debit
                    };
                    entry.JournalEntryDetailsDtos.Add(entryDetailsDto);
                }
                list.Add(entry);
            }
            //var result = pagedListJournalEntryDto.GetGenericPagination(list, paramters.PageNumber, paramters.PageSize, Mapper);
            return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);
        }
        public async Task<IRepositoryActionResult> UpdateJournalEntryDraft(UpdateJournalEntryParameter parameter)
        {
            try
            {
                var list = new List<UpdateJournalEntryParameter>();
                var journalentry = await journalEntryDraftRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
                var table = Mapping.Mapper.Map<UpdateJournalEntryParameter, GLJournalEntryDraft>(parameter, journalentry);
                table.Code = journalentry.Code;
                var checkCode = await CheckIsValidCode(table.Code);
                if (checkCode == true)
                {
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "This code Existing before ");
                }
                await journalEntryDraftRepositoryCommand.UpdateAsyn(table);
                await journalEntryDetailsDraftRepositoryCommand.DeleteAsync(q => q.JournalEntryDraftId == parameter.Id);
                foreach (var item in parameter.journalEntryDetails)
                {
                    //var journalDetails = await journalEntryDetailsRepositoryQuery.GetByAsync(q => q.JournalEntryId == parameter.Id);
                    var journalDetails = new GLJournalEntryDraftDetails()
                    {
                           CostCenterId = item.CostCenterId,
                           Credit = item.Credit,
                           Debit = item.Debit,
                           DescriptionAr = item.DescriptionAr,
                           DescriptionEn = item.DescriptionEn,
                           FinancialAccountId = item.FinancialAccountId,
                           JournalEntryDraftId = table.Id
                    };
                    journalEntryDetailsDraftRepositoryCommand.Add(journalDetails);
                    table.CreditTotal += journalDetails.Credit;
                    table.DebitTotal += journalDetails.Debit;

                }
                await journalEntryDraftRepositoryCommand.SaveAsync();
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<IRepositoryActionResult> BlockJournalEntry(int Id, bool isblocked)
        {
            try
            {
                var journalEntry = await journalEntryDraftRepositoryQuery.GetByAsync(q => q.Id == Id);
                if (journalEntry.IsBlock)
                    return repositoryActionResult.GetRepositoryActionResult(journalEntry.Id, RepositoryActionStatus.BadRequest, message: "The Draft is already blocked");
                    
                journalEntry.IsBlock = isblocked;
                await journalEntryDraftRepositoryCommand.UpdateAsyn(journalEntry);

                return repositoryActionResult.GetRepositoryActionResult(
                                                                        result: journalEntry.Id,
                                                                        status: RepositoryActionStatus.Ok,
                                                                        message: "Blocked Successfully"
                                                                        );
            }
            catch (Exception ex)
            {
                return  repositoryActionResult.GetRepositoryActionResult(ex);
                
                

            }
        }
        public async Task<IRepositoryActionResult> GetJournalEntryDraftById(int Id)
        {
            try
            {

                var doubleStringFormat = await invGeneralSettingsHandler.GetDoubleFormat();
                var costCenterData = await journalEntryDraftRepositoryQuery.GetByAsync(s => s.Id == Id);
                if(costCenterData==null)
                    return repositoryActionResult.GetRepositoryActionResult(costCenterData, RepositoryActionStatus.NotFound, message: "Not Found");

                var journal = new JournalEntryDto();
                journal.Id = costCenterData.Id;
                var branch = await branchRepositoryQuery.GetByAsync(q=>q.Id==costCenterData.BranchId);
                journal.BranchId = costCenterData.BranchId;
                journal.BranchName = branch?.ArabicName;
                journal.CreditTotal = costCenterData.CreditTotal;
                var currency = await currencyRepositoryQuery.GetByAsync(q => q.Id == costCenterData.CurrencyId);
                journal.CurrencyId = costCenterData.CurrencyId;
                if (currency != null)
                {
                    journal.CurrencyName = currency?.ArabicName;

                }
                journal.DebitTotal = costCenterData.DebitTotal;
                journal.FTDate = costCenterData.FTDate;
                journal.IsBlock = costCenterData.IsBlock;
                journal.IsDraft = true;
                //journal.Code = costCenterData.Code;
                journal.Notes = costCenterData.Notes;
                journal.Name = costCenterData.Name;
                var jouranlDetails = journalEntryDetailsDraftRepositoryQuery.FindAll(q => q.JournalEntryDraftId == journal.Id);
                foreach (var item in jouranlDetails)
                {
                    var details = new JournalEntryDetailsDto();
                    details.Id = item.Id;
                    details.JournalEntryId = item.JournalEntryDraftId;
                    var cost = await costCenterRepositoryQuery.GetByAsync(q => q.Id == item.CostCenterId);
                    details.CostCenterId = item.CostCenterId;
                    if (cost != null)
                    {
                        details.CostCenterName = cost.ArabicName;
                    }
                    var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == item.FinancialAccountId);
                    details.FinancialAccountId = item.FinancialAccountId;
                    details.FinancialAccountName = financial.ArabicName;
                    details.FinancialAccountCode = financial.AccountCode.Replace(".",string.Empty);
                    details.Debit = item.Debit;
                    details.Credit = item.Credit;
                    details.DescriptionAr = item.DescriptionAr;
                    details.DescriptionEn = item.DescriptionEn;
                    journal.JournalEntryDetailsDtos.Add(details);
                }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
       

                    return repositoryActionResult.GetRepositoryActionResult(journal, RepositoryActionStatus.Ok, message: "Ok");
                return repositoryActionResult.GetRepositoryActionResult(costCenterData, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }

        
    }
}
