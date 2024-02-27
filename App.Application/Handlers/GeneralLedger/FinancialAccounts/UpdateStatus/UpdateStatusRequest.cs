﻿using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class UpdateStatusRequest : SharedRequestDTOs.UpdateStatus, IRequest<IRepositoryActionResult>
    {
    }
}
