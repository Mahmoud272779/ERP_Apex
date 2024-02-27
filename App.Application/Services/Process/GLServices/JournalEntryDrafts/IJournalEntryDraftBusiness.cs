using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.JournalEntryDrafts
{
    public interface IJournalEntryDraftBusiness
    {
        Task<IRepositoryActionResult> AddJournalEntryDraft(JournalEntryParameter parameter);
        Task<IRepositoryActionResult> GetJournalEntryDraft();
        Task<IRepositoryActionResult> UpdateJournalEntryDraft(UpdateJournalEntryParameter parameter);
        Task<IRepositoryActionResult> BlockJournalEntry(int Id, bool isblocked);
        Task<IRepositoryActionResult> GetJournalEntryDraftById(int Id);
        Task<string> AddAutomaticCodeDraft();
    }
}
