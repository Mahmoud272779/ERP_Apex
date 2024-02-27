using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.RecieptsWithInvoices
{
    public class AddRecieptsForInvoices : BaseClass, IAddRecieptsForInvoices
    {
        private readonly IRepositoryCommand<GLJournalEntry> JournalEntryCommand;
        private readonly IRepositoryCommand<GlReciepts> RecieptRepositoryCommand;
        private readonly IRepositoryQuery<GlReciepts> RecieptRepositoryQuery;

        private readonly IGeneralAPIsService _generalAPIsService;
        private readonly IHttpContextAccessor httpContext;

        public AddRecieptsForInvoices(IRepositoryCommand<GLJournalEntry> JournalEntryCommand,
                                   IRepositoryCommand<GlReciepts> _RecieptRepositoryCommand,
                                   IGeneralAPIsService generalAPIsService, IRepositoryQuery<GlReciepts> RecieptRepositoryQuery,
                                   IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            this.JournalEntryCommand = JournalEntryCommand;
            RecieptRepositoryCommand = _RecieptRepositoryCommand;
            _generalAPIsService = generalAPIsService;
            this.RecieptRepositoryQuery = RecieptRepositoryQuery;
            httpContext = _httpContext;
        }

        public Task<ResponseResult> AddInvoiceReceipt(string personName, int? personCode, string mainInvoiceCode, double debtor, double creditor, int InvoiceTypeId, int signal, double serialize, int NextCode, string browserName, int personId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> blockReceipt(int[] receiptId)
        {
            var receipt = await RecieptRepositoryQuery.GetAllAsyn(x => receiptId.Contains(x.Id));
            if (receipt != null)
                receipt.ToList().ForEach(x => x.IsBlock = true);
            return await RecieptRepositoryCommand.UpdateAsyn(receipt);
        }

        public async Task<bool> deleteInvoiceReceipt(int receiptId)
        {
            bool saved = false;
            if (receiptId == 0)
                return false;
            var receipt = RecieptRepositoryQuery.GetAll().Where(x=> x.Id == receiptId);
            if (receipt != null)
                saved = await RecieptRepositoryCommand.DeleteAsync(receipt);
            return saved;
        }

        public async Task<GlReciepts> getReceipt(int id)
        {
            throw new NotImplementedException();
        }

        //public async Task<GLReciept> getReceipt(int id)
        //{
        //    return await RecieptRepositoryQuery.FindAsync(x => x.Id == id);
        //}

        public async Task<double> GetTotalAmount(int personId)
        {
            var res =  await _generalAPIsService.GetTotalAmountOfPerson(personId,null);
            return (double)res.Data;
        }

        public async Task<bool> updateReceipt(GlReciepts gLReciept)
        {
            throw new NotImplementedException();
        }

     

        //public async Task<bool> updateReceipt(GLReciept gLReciept)
        //{
        //    bool saved = false;
        //    if (gLReciept != null)
        //        saved= await RecieptRepositoryCommand.UpdateAsyn(gLReciept);
        //    return saved;
        //}
    }
}
