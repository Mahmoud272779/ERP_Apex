using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public static class financialAccountsHelper
    {
        public static IQueryable<GLFinancialAccount> getAllAccounts(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery) => financialAccountRepositoryQuery.TableNoTracking;
        public static IQueryable<GLJournalEntryDetails> getAllGLJournalEntryDetails(IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery) => journalEntryRepositoryQuery.TableNoTracking.Include(x => x.GLFinancialAccount);
        public static IQueryable<GLBank> getAllBanks(IRepositoryQuery<GLBank> _gLBanKQuery) => _gLBanKQuery.TableNoTracking;
        public static IQueryable<GLSafe> getAllGLSafes(IRepositoryQuery<GLSafe> _gLSafeQuery) => _gLSafeQuery.TableNoTracking;
        public static IQueryable<GlReciepts> getAllGLReciept(IRepositoryQuery<GlReciepts> _gLRecieptQuery) => _gLRecieptQuery.TableNoTracking;
        public static IQueryable<GLOtherAuthorities> getAllGLOtherAuthorities(IRepositoryQuery<GLOtherAuthorities> _gLOtherAuthoritiesQuery) => _gLOtherAuthoritiesQuery.TableNoTracking;
        public static async Task<bool> CheckIsValidCode(string Code, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            var journalEntry = await financialAccountRepositoryQuery.SingleOrDefault(
                   cust => cust.AccountCode == Code);
            return journalEntry == null ? false : true;
        }
        public static async void setAutoCodeValue(GLFinancialAccount table, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            var LastMainCode = financialAccountRepositoryQuery.FindBy(s => s.ParentId == null).ToList();
            if (LastMainCode.Count() > 0)
            {
                table.MainCode = LastMainCode.Last().MainCode + 1;
                var nextCode = table.MainCode.ToString();
                table.autoCoding = nextCode;
            }
            else
            {
                table.MainCode = 1;
                var nextCode = table.MainCode.ToString();
                table.autoCoding = nextCode;
            }
        }
        public static async void ParentCodes(GLFinancialAccount table, GLGeneralSetting setting, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            var LastMainCode = financialAccountRepositoryQuery.FindBy(s => s.ParentId == null).OrderBy(x => x.MainCode).ToList();
            if (LastMainCode.Count() > 0)
            {
                table.MainCode = LastMainCode.Last().MainCode + 1;
                var numOfMain = setting.subCodeLevels.First().value;
                var nextCode = table.MainCode.ToString();
                #region old
                //var dif = numOfMain - nextCode.Length;
                //table.autoCoding = nextCode;
                //for (int k = 0; k < dif; k++)
                //{
                //    nextCode = "0" + nextCode;
                //}
                #endregion
                table.autoCoding = nextCode;
                nextCode = nextCode.ToString().PadLeft(numOfMain, '0');
                table.AccountCode = nextCode;
            }
            else
            {
                table.MainCode = 1;
                var numOfMain = setting.subCodeLevels.First().value;
                var nextCode = table.MainCode.ToString();
                #region old
                //var dif = numOfMain - nextCode.Length;
                //table.autoCoding = nextCode;
                //for (int k = 0; k < dif; k++)
                //{
                //    nextCode = "0" + nextCode;
                //}
                #endregion
                nextCode = nextCode.ToString().PadLeft(numOfMain, '0');
                table.AccountCode = nextCode;
            }
        }
        public static async void ChildCodes(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery,GLFinancialAccount table, GLGeneralSetting setting, int? ParentId, bool isOld = false, List<GLFinancialAccount> arryaTable = null, bool isMoved = false, bool isRecoding = false)
        {
            var LastSubCodelis = financialAccountRepositoryQuery.FindBy(s => s.ParentId == ParentId).ToList();
            if (LastSubCodelis.Count() == 0)
            {
                var LastSubCodeliss = financialAccountRepositoryQuery.FindAll(q => q.Id == ParentId);

                ChildCodesWithCountZero(table, setting, LastSubCodeliss.Last());
            }
            else
            {

                var LastSubCodeliss = financialAccountRepositoryQuery.FindAll(q => q.Id == ParentId);

                ChildCodesWithCount(financialAccountRepositoryQuery,table, setting, LastSubCodelis, LastSubCodeliss.Last(), isOld, arryaTable, isMoved, isRecoding);
            }
        }
        public static async void ChildCodesWithCountZero(GLFinancialAccount table, GLGeneralSetting setting,
                                                      GLFinancialAccount LastSubCodeliss)
        {
            table.MainCode = LastSubCodeliss.MainCode;
            //var numOfSub = setting.SubCode;


            var codeLevel = LastSubCodeliss.AccountCode.Split('.').Length + 1;
            int numOfSub;
            if (setting.subCodeLevels.Select(x => x.value).ToArray().Length >= codeLevel)
                numOfSub = setting.subCodeLevels.Skip(codeLevel - 1).Select(x => x.value).First();
            else
                numOfSub = setting.subCodeLevels.Select(x => x.value).Last();



            table.SubCode = LastSubCodeliss.SubCode + 1;
            //  var next = (LastSubCode.AccountCode.Last());
            var nextCode = (1).ToString();
            var dif = numOfSub - nextCode.Length;
            table.autoCoding = LastSubCodeliss.autoCoding + "." + nextCode;
            for (int k = 0; k < dif; k++)
            {
                nextCode = "0" + nextCode;
            }
            table.AccountCode = LastSubCodeliss.AccountCode + "." + nextCode;
        }
        public static async void ChildCodesWithCount(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery,GLFinancialAccount table, GLGeneralSetting setting, List<GLFinancialAccount> LastSubCodelis,
                                                GLFinancialAccount LastSubCodeliss, bool isOld = false, List<GLFinancialAccount> arrayTable = null, bool isMoved = false, bool isRecoding = false)
        {
            var LastSubCode = LastSubCodelis.OrderBy(x => int.Parse(x.autoCoding.Split('.').Last())).Last();
            var findLastCode = LastSubCode.autoCoding;
            var codeLevel = LastSubCodeliss.AccountCode.Split('.').Length;
            int numOfSub;
            if (setting.subCodeLevels.Take(codeLevel).Any())
                if (setting.subCodeLevels.Count() < codeLevel)
                    numOfSub = setting.subCodeLevels.Skip(codeLevel).Select(x => x.value).First();
                else
                    numOfSub = setting.subCodeLevels.Select(x => x.value).Last();
            else
                numOfSub = setting.subCodeLevels.Select(x => x.value).Last();
            string code = "";
            if (table.autoCoding != null)
                code = table.autoCoding.Split('.').Last();


            if (arrayTable != null)
            {
                numOfSub = setting.subCodeLevels.Skip(table.AccountCode.Split('.').Length).Select(x => x.value).First();
                //var dif = numOfSub - code.Length;

                for (int i = 0; i < arrayTable.Count(); i++)
                {
                    var autoCoding = arrayTable.ElementAt(i);
                    if (autoCoding != null)
                        autoCoding.autoCoding = table.autoCoding + "." + (i + 1).ToString();
                    code = (i + 1).ToString();
                    #region old loop
                    //for (int k = 0; k < dif; k++)
                    //{
                    //    code = "0" + code;
                    //}
                    #endregion
                    code = int.Parse(code).ToString().PadLeft(numOfSub, '0');
                    if (autoCoding.AccountCode != "")
                        autoCoding.AccountCode = autoCoding.AccountCode + '.';
                    autoCoding.AccountCode = table.AccountCode + "." + code;
                    _CheckAutoCodeLenth(autoCoding.AccountCode, numOfSub);
                }
            }
            else
            {
                if (isOld)
                {
                    if (isMoved)
                    {
                        code = financialAccountRepositoryQuery.FindAll(x => x.ParentId == LastSubCodeliss.Id).OrderBy(x => int.Parse(x.autoCoding.Split('.').Last())).Select(x => x.autoCoding).LastOrDefault().Split('.').Last();
                        if (int.Parse(code) != 0)
                            code = (int.Parse(code) + 1).ToString();
                        else
                            code = "1";
                    }
                    var dif = numOfSub - code.Length;
                    table.autoCoding = LastSubCodeliss.autoCoding + "." + code;
                    #region old loop
                    //for (int k = 0; k < dif; k++)
                    //{
                    //    code = "0" + code;
                    //}
                    #endregion
                    code = int.Parse(code).ToString().PadLeft(numOfSub, '0');
                    table.AccountCode = LastSubCodeliss.AccountCode + "." + code;
                    _CheckAutoCodeLenth(table.AccountCode, numOfSub);
                }
                else
                {
                    table.SubCode = LastSubCode.SubCode + 1;
                    table.MainCode = LastSubCode.MainCode;
                    var next = LastSubCode.AccountCode.Split('.').ToList();
                    //   var next = table.SubCode - table.MainCode;
                    if (next.Last().ToString().Length > numOfSub)
                        table.AccountCode = null;
                    else
                    {
                        //var nextCode = (Int32.Parse(next.Last()) + 1).ToString();
                        //var dif = numOfSub - nextCode.Length;
                        #region old loop
                        //for (int k = 0; k < dif; k++)
                        //{
                        //    nextCode = "0" + nextCode;
                        //}
                        #endregion
                        if (LastSubCodelis.Count() == 0)
                            code = LastSubCodeliss.AccountCode + '.' + (1).ToString().PadLeft(numOfSub, '0');
                        else
                            code = LastSubCodeliss.AccountCode + '.' + (int.Parse(LastSubCodelis.OrderBy(x => int.Parse(x.autoCoding.Split('.').Last())).Last().autoCoding.Split('.').Last()) + 1).ToString().PadLeft(numOfSub, '0');

                        table.autoCoding = LastSubCodeliss.autoCoding + "." + (int.Parse(LastSubCodelis.OrderBy(x => int.Parse(x.autoCoding.Split('.').Last())).Last().autoCoding.Split('.').Last()) + 1).ToString();
                        table.AccountCode = code;
                        _CheckAutoCodeLenth(table.AccountCode, numOfSub);
                    }
                }
            }
        }
        public static void _CheckAutoCodeLenth(string autoCode, int numOfSub)
        {
            if (autoCode.Split('.').Last().Length > numOfSub)
                autoCode = null;
        }
        public static async void Recoding(List<GLFinancialAccount> arrayTable, GLGeneralSetting setting)
        {
            if (arrayTable.Any())
            {
                foreach (var item in arrayTable)
                {
                    int numOfSub;

                    string _code;
                    var Length = item.autoCoding.Split('.');
                    for (int i = 0; i < Length.Length; i++)
                    {
                        foreach (var code in Length)
                        {
                            if (setting.subCodeLevels.Take(Length.Length).Any())
                                numOfSub = setting.subCodeLevels.Skip(Length.Length).Select(x => x.value).First();
                            else
                                numOfSub = setting.subCodeLevels.Select(x => x.value).Last();
                            var dif = numOfSub - Length.Length;
                            _code = code;
                            for (int k = 0; k < dif; k++)
                            {
                                _code = "0" + _code;
                            }

                        }

                    }

                }
            }
        }
        public static async Task<bool> CheckIsValidCodeInUpate(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery,string accountCode, int? id)
        {
            //var list = financialAccountRepositoryQuery.FindQueryable(q=>q.AccountCode!=null);
            //var empCodes = financialAccountRepositoryQuery.FindSelectorQueryable<string>(list, q => q.AccountCode);
            //var List = empCodes.ToList();
            //var parentDatas = financialAccountRepositoryQuery.FindAll(q => !List.Contains(q.AccountCode));
            var code = await financialAccountRepositoryQuery.GetFirstOrDefault(
                   cust => cust.AccountCode == accountCode
                   && cust.Id != id);

            return code;

        }
        public class DeleteResponse
        {
            public int? parentId { get; set; }
        }
        public static async Task<bool> checkIfHasChildren(int id, IQueryable<GLFinancialAccount> gLFinancialAccounts)
        {
            var check = gLFinancialAccounts.Where(x => x.ParentId == id).Any();
            return check;
        }
        public static async Task<bool> CanDelete(string autoCoding, IQueryable<GLFinancialAccount> allAccounts, IQueryable<GLBank> Banks, IQueryable<GLJournalEntryDetails> JournalEntryDetails,
                                           IQueryable<GLSafe> Safe, IQueryable<GlReciepts> Reciept, IQueryable<GLOtherAuthorities> OtherAuthorities)
        {
            var gLJournalEntryDetailscanDelete = JournalEntryDetails.Where(x => x.GLFinancialAccount.autoCoding.StartsWith(autoCoding)).Any();

            if (gLJournalEntryDetailscanDelete)
                return false;
            else
                return true;
            #region old
            //var children = allAccounts.Where(x => x.autoCoding.StartsWith(autoCoding)).Select(x => x.Id).ToArray();

            //var gLBanKQuerycanDelete = Banks.Where(x => children.Contains((int)x.FinancialAccountId)).Any();



            //var gLSafecanDelete = Safe.Where(x => x.FinancialAccountId != null ? children.Contains((int)x.FinancialAccountId) : false).Any();


            //var gLRecieptcanDelete = Reciept.Where(x => children.Contains(x.FinancialAccountId.Value)).Any();


            //var gLOtherAuthoritiescanDelete = OtherAuthorities.Where(x => children.Contains((int)x.FinancialAccountId)).Any();
            #endregion
        }

        public static async Task<List<GLFinancialAccount>> GetUserChildRoles(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery,List<GLFinancialAccount> firstchildIds)
        {
            List<GLFinancialAccount> userChildRoles = new List<GLFinancialAccount>();
            if (firstchildIds != null)
            {
                foreach (var item in firstchildIds)
                {
                    var firstchild = await financialAccountRepositoryQuery.SingleOrDefault(s => s.Id == item.Id && s.IsBlock == false
                             , includes: role1 => role1.financialAccounts);
                    //var firstchild = await financialAccountRepositoryQuery.TableNoTracking.Include(e => e.financialAccounts).SingleOrDefaultAsync();
                    var rolesChilderenId = GetAllChildren(firstchild, financialAccountRepositoryQuery);
                    userChildRoles.Add(firstchild);
                    userChildRoles.AddRange(rolesChilderenId);
                }
            }


            return userChildRoles;
        }
        public static List<GLFinancialAccount> GetAllChildren(GLFinancialAccount parent, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            List<GLFinancialAccount> children = new List<GLFinancialAccount>();
            PopulateChildren(parent, children, financialAccountRepositoryQuery);
            return children;
        }
        public static void PopulateChildren(GLFinancialAccount parent, List<GLFinancialAccount> children, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            List<GLFinancialAccount> myChildren;

            if (TryGetItschildren(parent, out myChildren))
            {
                children.AddRange(myChildren);
                if (myChildren != null)
                {
                    foreach (GLFinancialAccount child in myChildren)
                    {
                        var firstchild = financialAccountRepositoryQuery.SingleOrDefault(s => s.Id == child.Id && s.IsBlock == false
                             , includes: role1 => role1.financialAccounts
                            ).Result;

                        PopulateChildren(firstchild, children, financialAccountRepositoryQuery);
                    }
                }

            }
        }
        public static bool TryGetItschildren(GLFinancialAccount role, out List<GLFinancialAccount> list)
        {
            list = new List<GLFinancialAccount>();
            if (role != null)
            {
                if (role.financialAccounts.Count() == 0)
                {
                    return false;
                }
                else
                {
                    if (role.financialAccounts != null)
                    {
                        foreach (var item in role.financialAccounts)
                        {
                            list.Add(item);
                        }
                    }


                    return true;
                }
            }
            return false;

        }
        public static GLFinancialAccount PopulateChildren(GLFinancialAccount sourceOrgano, ICollection<GLFinancialAccount> organos)
        {
            GLFinancialAccount organo_ = new GLFinancialAccount();
            var children = organos.Where(x => x.ParentId == sourceOrgano.Id);
            if (children != null)
            {
                foreach (var child in children)
                {
                    //child
                    GLFinancialAccount organoChild_ = new GLFinancialAccount();
                    organoChild_.Id = child.Id;
                    organoChild_.ParentId = child.ParentId;
                    organoChild_.ArabicName = child.ArabicName;
                    organoChild_.LatinName = child.LatinName;
                    organo_.financialAccounts = new List<GLFinancialAccount>();
                    //organo_.financialAccounts.add(organoChild_);
                    PopulateChildren(child, organos);
                }
            }

            return organo_;
        }
        public static async void HistoryFinancialAccount(
                                                        IRepositoryCommand<GLFinancialAccountHistory> financialAccountHistoryRepositoryCommand,
                                                        iUserInformation _iUserInformation,int currencyId, string accountCode, int status, int fA_Nature, int finalAccount, double credit,
                                                        double debit, string notes, int? parentId, int? hasCostCenter, string browserName, string lastTransactionAction, string addTransactionUser)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var history = new GLFinancialAccountHistory()
            {
                employeesId = userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                LastTransactionAction = lastTransactionAction,
                AddTransactionUser = addTransactionUser,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                LastTransactionUserAr = userInfo.employeeNameAr.ToString(),
                CurrencyId = currencyId,
                AccountCode = accountCode,
                Status = status,
                FA_Nature = fA_Nature,
                FinalAccount = finalAccount,
                Credit = credit,
                Debit = debit,
                Notes = notes,
                ParentId = parentId,
                HasCostCenter = hasCostCenter,
                BrowserName = browserName,
            };
            financialAccountHistoryRepositoryCommand.Add(history);
        }
        public static void recodingAccounts(List<GLFinancialAccount> listOfAccounts, GLGeneralSetting settings)
        {
            for (int i = 0; i < listOfAccounts.Count(); i++)
            {
                var accountCode = listOfAccounts.ElementAt(i);
                var accountAutoCode = listOfAccounts.ElementAt(i).autoCoding;
                var accountAutoCodeSplit = listOfAccounts.ElementAt(i).autoCoding.Split('.');
                string newAccountCode = "";
                string finalAccountCode = "";
                for (int x = 0; x < accountAutoCodeSplit.Count(); x++)
                {

                    var splitedCode = accountAutoCodeSplit.ElementAt(x);
                    int numOfSub = 0;
                    if (settings.subCodeLevels.Count() <= x)
                        numOfSub = settings.subCodeLevels.Select(x => x.value).Last();
                    else
                        numOfSub = settings.subCodeLevels.Select(x => x.value).ElementAt(x);
                    #region old 
                    //var dif = numOfSub - splitedCode.Length;
                    ////for (int d = 0; d < dif; d++)
                    ////{
                    ////    splitedCode = "0" + splitedCode;
                    ////}
                    ///
                    #endregion

                    splitedCode = int.Parse(splitedCode).ToString().PadLeft(numOfSub, '0');
                    if (finalAccountCode != "")
                        finalAccountCode = finalAccountCode + '.';
                    newAccountCode = splitedCode;
                    finalAccountCode = finalAccountCode + newAccountCode;
                }
                accountCode.AccountCode = finalAccountCode;
            }
        }
    }
}
