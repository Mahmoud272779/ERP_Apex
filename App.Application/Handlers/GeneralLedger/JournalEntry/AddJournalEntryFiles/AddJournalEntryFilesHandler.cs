using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class AddJournalEntryFilesHandler : BusinessBase<GLJournalEntry>,IRequestHandler<AddJournalEntryFilesRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand;

        public AddJournalEntryFilesHandler(IRepositoryActionResult repositoryActionResult, IRepositoryCommand<GLJournalEntryFiles> journalEntryFilesRepositoryCommand) : base(repositoryActionResult)
        {
            this.journalEntryFilesRepositoryCommand = journalEntryFilesRepositoryCommand;
        }

        public async Task<IRepositoryActionResult> Handle(AddJournalEntryFilesRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                var list = new List<JournalEntryFilesDto>();
                var journalDetails = new GLJournalEntryFiles();
                journalDetails.JournalEntryId = parameter.JournalEntryId;
                var img = parameter.ImagePath;
                if (img == null)
                {
                    if (string.IsNullOrEmpty(journalDetails.File) || journalDetails.File == "string")
                    {
                        journalDetails.File = "JournalEntry\\8-30-2020 5-45-17 PMCapture.PNG";
                    }
                }
                else
                {
                    //foreach (var item in img)
                    //{
                    //    var path = _hostingEnvironment.WebRootPath;
                    //    string filePath = Path.Combine("JournalEntry\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + item.FileName.Replace(" ", ""));
                    //    string actulePath = Path.Combine(path, filePath);
                    //    using (var fileStream = new FileStream(actulePath, FileMode.Create))
                    //    {
                    //        await item.CopyToAsync(fileStream);
                    //    }
                    //    journalDetails.File = filePath;
                    //}
                }
                journalEntryFilesRepositoryCommand.Add(journalDetails);
                return repositoryActionResult.GetRepositoryActionResult(journalDetails.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
