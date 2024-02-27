using App.Application.SignalRHub;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Process.Store;
using App.Infrastructure;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
//using System.Data.Entity;

namespace App.Application
{
    public class branchsService : iBranchsService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepositoryQuery<InvEmployeeBranch> _employeeBranchQuery;
        private readonly IRepositoryCommand<InvEmployeeBranch> _employeeBranchCommand;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IRepositoryQuery<signalR> _signalRQuery;

        public branchsService(IHttpContextAccessor httpContext,
            IRepositoryQuery<InvEmployeeBranch> employeeBranchQuery,
            IRepositoryCommand<InvEmployeeBranch> employeeBranchCommand,
            IHubContext<NotificationHub> hub,
            IRepositoryQuery<signalR> signalRQuery)
        {
            _httpContext = httpContext;
            _employeeBranchQuery = employeeBranchQuery;
            _employeeBranchCommand = employeeBranchCommand;
            _hub = hub;
            _signalRQuery = signalRQuery;
        }
        public async Task<string> getEmployeeId()
        {
            Exception ex = new Exception(StatusCodes.Status401Unauthorized.ToString());
            var token = await _httpContext.HttpContext.GetTokenAsync("access_token");
            if (token == null)
                return null;
            if (string.IsNullOrEmpty(token))
                return "0";
            var employeeId = contextHelper.GetEmployeeId(token);
            return employeeId;
        }
        public async Task<ResponseResult> getBranches()
        {
            var empId = await getEmployeeId();
            if (empId == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };
            var employeeId = StringEncryption.DecryptString(empId);
            if (employeeId == "0")
                return new ResponseResult()
                {
                    Note = "Error JWT Token",
                    Result = Result.Failed
                };

            var branches = _employeeBranchQuery.TableNoTracking.Include(x => x.Branch).Where(x=> x.EmployeeId == int.Parse(employeeId)).Select(x => new { x.BranchId, x.Branch.ArabicName, x.Branch.LatinName, selected = x.current });
            if (!branches.Where(x => x.selected == true).Any())
            {
                var FirstempBranch = _employeeBranchQuery.GetAll().Where(x => x.EmployeeId == int.Parse(employeeId)).FirstOrDefault();
                FirstempBranch.current = true;
                await _employeeBranchCommand.UpdateAsyn(FirstempBranch);
                branches = _employeeBranchQuery.TableNoTracking.Include(x => x.Branch).Where(x => x.EmployeeId == int.Parse(employeeId)).Select(x => new { x.BranchId, x.Branch.ArabicName, x.Branch.LatinName, selected = x.current });
            }
            return new ResponseResult()
            {
                Data = branches,
                Note = Actions.Success,
                Result = Result.Success
            };
        }
        public async Task<ResponseResult> GetBranchesForAllUsers(int pageNumber,int pageSize)
        {

            var userBranches = _employeeBranchQuery.TableNoTracking.GroupBy(m => m.EmployeeId).Select(g => new
            {
                Employee = g.Key,
                data = g.Select(user => new
                {
                    user.BranchId,
                    user.current
                })
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            return new ResponseResult()
            {
                Data = userBranches,
                Note = Actions.Success,
                Result = Result.Success
            };
        }
        public async Task<ResponseResult> updatedSelectedBranch(int branchId)
        {
            var employeeId = StringEncryption.DecryptString(getEmployeeId().Result);
            if (employeeId == "0")
                return new ResponseResult()
                {
                    Note = "Error JWT Token",
                    Result = Result.Failed
                };
            var branchs = _employeeBranchQuery.TableNoTracking.Where(x => x.EmployeeId == int.Parse(employeeId));
            if(!branchs.Where(x=> x.BranchId == branchId).Any())
                return new ResponseResult()
                {
                    Note = Actions.userDoseNotHaveAccessToThisBranch,
                    Result = Result.Failed
                };
            var listOfBranches = new List<InvEmployeeBranch>();
            foreach (var branch in branchs)
            {
                branch.current = branch.BranchId == branchId ? true : false;
                listOfBranches.Add(branch); 
            }
            var saved = await _employeeBranchCommand.UpdateAsyn(listOfBranches);
            var connetionsId = _signalRQuery.TableNoTracking.Where(c => c.InvEmployeesId == int.Parse(employeeId)).Select(c => c.connectionId).ToArray();
            //await _hub.Clients.Clients(connetionsId).SendAsync(defultData.ReloadNotification); // this for chrome dublicate to handle brance its have bug 

            return new ResponseResult()
            {
                Note = saved ? Actions.Success : Actions.SaveFailed,
                Result = saved ? Result.Success : Result.Failed
            };
        }
    }
}
