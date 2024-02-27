using App.Domain.Models.Response.GeneralLedger;
using App.Infrastructure.settings;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.MultiCollectionReceipts.GetAllMultiCollectionReceipts
{
    public class GetAllMultiCollectionReceiptsHandler : IRequestHandler<GetAllMultiCollectionReceiptsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRoundNumbers _roundNumbers;
        public GetAllMultiCollectionReceiptsHandler(IRepositoryQuery<GlReciepts> glRecieptsQuery, iUserInformation iUserInformation, IRepositoryQuery<InvPersons> invPersonsQuery, IRoundNumbers roundNumbers)
        {
            _GlRecieptsQuery = glRecieptsQuery;
            _iUserInformation = iUserInformation;
            _InvPersonsQuery = invPersonsQuery;
            _roundNumbers = roundNumbers;
        }
        public async Task<ResponseResult> Handle(GetAllMultiCollectionReceiptsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var persons = _InvPersonsQuery.TableNoTracking;
            var AllRecs = _GlRecieptsQuery.TableNoTracking
                .Include(c => c.person)
                .Where(c => c.BranchId == userInfo.CurrentbranchId)
                .Where(c => request.isBank ? c.BankId != null : c.SafeID != null)
                .Where(c => c.RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || c.RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts);
            var totalCount = AllRecs.Count();
            int[] recPersonsIds = null;
            if (request.personId != null)
            {
                recPersonsIds = AllRecs.Where(c => c.PersonId == request.personId && c.MultiCollectionReceiptParentId != null).Select(c => c.MultiCollectionReceiptParentId.Value).ToArray();
            }
            int[] personsForSearch = null;
            if (!string.IsNullOrEmpty(request.searchCriteria))
            {
                personsForSearch = persons.Where(c => !string.IsNullOrEmpty(request.searchCriteria) ? c.ArabicName.Contains(request.searchCriteria) || c.LatinName.Contains(request.searchCriteria) || c.Phone == request.searchCriteria : true).Select(c => c.Id).ToArray();
            }

            var recs = AllRecs
                              .Where(c => c.IsAccredit)
                              .Where(c => request.personId != null ? recPersonsIds.Contains(c.Id) : true)
                              .Where(c => request.Authority != null && request.Authority != 0 ? c.Authority == request.Authority : true)
                              .Where(c => !string.IsNullOrEmpty(request.searchCriteria) ? c.PaperNumber.Contains(request.searchCriteria) || c.RecieptType.Contains(request.searchCriteria) || (personsForSearch != null || personsForSearch.Any() ? personsForSearch.Contains(c.BenefitId) : false) : true)
                              .OrderByDescending(c => c.Id);
            if (!string.IsNullOrEmpty(request.searchCriteria))
            {
                recs = recs.OrderBy(a => a.Code);
            }

            var dataCount = recs.Count();
            var res = recs.Skip(((request.PageNumber ?? 0) - 1) * request.PageSize ?? 0).Take(request.PageSize ?? 0)
                          .Select(c => new GetAllMultiCollectionReceiptsResponseDTO
                          {
                              Id = c.Id,
                              amount = _roundNumbers.GetRoundNumber(c.Amount),
                              RecieptType = c.RecieptType,
                              RecieptDate = c.RecieptDate.ToString(defultData.datetimeFormat),
                              canDelete = !c.IsBlock,
                              isEdit = !c.IsBlock,
                              isBlock = c.IsBlock,
                              Authority = new GetAllMultiCollectionReceipts_Authority
                              {
                                  Id = c.Authority,
                                  arabicName = c.Authority == (int)AuthorityTypes.customers ? "عملاء" : "موردين",
                                  latinName = c.Authority == (int)AuthorityTypes.customers ? "Customers" : "Suppliers",
                              },
                              Benefit = new Benefit
                              {
                                  Id = c.BenefitId,
                                  arabicName = c.BenefitId != 0 ? persons.FirstOrDefault(x => x.Id == c.BenefitId).ArabicName : "الكل",
                                  latinName = c.BenefitId != 0 ? persons.FirstOrDefault(x => x.Id == c.BenefitId).LatinName : "All"

                              }
                          });
            return new ResponseResult
            {
                Data = res,
                DataCount = dataCount,
                TotalCount = totalCount,
                Result = Result.Success
            };
        }
    }
}
