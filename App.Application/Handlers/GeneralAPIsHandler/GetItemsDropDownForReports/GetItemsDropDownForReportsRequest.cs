using App.Domain.Models.Request.General;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetItemsDropDownForReports
{
    public class GetItemsDropDownForReportsRequest : dropdownListOfItemsForReports, IRequest<ResponseResult>
    {
    }
}
