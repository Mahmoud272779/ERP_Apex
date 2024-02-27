using App.Domain.Models.Request.General;
using MediatR;

namespace App.Application.Handlers.GeneralAPIsHandler.GetStoresDropDownForReports
{
    public class GetStoresDropDownForReportsRequest : dropdownListOfItemsForReports,IRequest<ResponseResult>
    {
    }
}
