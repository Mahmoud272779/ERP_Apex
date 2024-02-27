using App.Application.Helpers.AlterDatabaseServices;
using App.Application.Helpers.UpdateSystem.Services;
using App.Application.Helpers.UpdateSystem.Updates;
using App.Application.SignalRHub;
using App.Domain.Entities;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Process.Store;
using App.Infrastructure;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.settings;
using App.Infrastructure.UserManagementDB;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace App.Application
{
    public class loginService : iLoginService
    {
        private readonly iAuthService iAuthService;
        private readonly ClientSqlDbContext _clientcontext;
        private readonly IConfiguration _configuration;
        private readonly iUpdateService _iUpdateService;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly ERP_UsersManagerContext _userManagementcontext;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IWebHostEnvironment webHostEnvironment;
        private bool isPeriodEnded = false;

        public loginService(    
                            iAuthService iAuthService,
                            ClientSqlDbContext Clientcontext,
                            IConfiguration configuration,
                            iUpdateService iUpdateService,
                            ISystemHistoryLogsService systemHistoryLogsService,
                            IMemoryCache memoryCache,
                            IHubContext<NotificationHub> hub,
                            IWebHostEnvironment webHostEnvironment)
        {
            this.iAuthService = iAuthService;
            _clientcontext = Clientcontext;
            _configuration = configuration;
            _iUpdateService = iUpdateService;
            _systemHistoryLogsService = systemHistoryLogsService;
            _userManagementcontext = new ERP_UsersManagerContext(configuration);
            _memoryCache = memoryCache;
            _hub = hub;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task<ResponseResult> login(loginReqDTO parm)
        {
            var isTest = _configuration["ApplicationSetting:isInTest"];
            var listOfTestCompanies = _configuration["ApplicationSetting:TestListCompanys"].Split(',');
            if (isTest == "1")
            {
                if (!listOfTestCompanies.Any(c => c == parm.companyName))
                    return new ResponseResult
                    {
                        Result = Result.UnAuthorized,
                        ErrorMessageAr = "لا يمكن الدخول علي هذه الشركة من برنامج الاختبارات",
                        ErrorMessageEn = "You can not open this company from the test project"
                    };
            }

            SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:UserManagerConnection"]);
            con.Open();
            var isDeploying = false;
            try
            {
                isDeploying = con.Query<bool>("select top 1 [isDeploying] from  [genralSettings]").FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            if (isDeploying)
            {
                if (!listOfTestCompanies.Any(c => c == parm.companyName))
                {
                    return new ResponseResult
                    {
                        Result = Result.Success,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "جاري تحديث النظام",
                            MessageEn = "System is updating",
                            titleAr = "تحديث النظام",
                            titleEn = "Update"
                        }
                    };

                }
            }

            bool isTechnUser = false;
            var splitedCompanyName = parm.companyName.Split('#');
            if (splitedCompanyName.Length > 1)
                if (splitedCompanyName.LastOrDefault() == Helpers.Helpers.generateSecretCode())
                {
                    StringBuilder _companyName = new StringBuilder();
                    foreach (var item in splitedCompanyName.Where(c => c != splitedCompanyName.LastOrDefault()))
                    {
                        _companyName.Append(item);
                    }
                    parm.companyName = _companyName.ToString();
                    isTechnUser = true;
                }




            #region get company inforamtion and check is valid
            var Company = _userManagementcontext.UserApplicationCashes.AsNoTracking()
                                .Include(x => x.UserApplication)
                                .Include(x => x.SubReqPeriods)
                                .Include(x => x.UserApplicationApps)
                                .Include(x => x.Bundles)
                                .Include(x => x.AdditionalPriceSubscriptions)
                                .Where(x => x.UserApplication.CompanyLogin == parm.companyName)
                                .Where(x => x.SubReqPeriods.Where(c => c.DateFrom <= DateTime.Now).OrderBy(c => c.Id).LastOrDefault().DateFrom <= DateTime.Now).OrderBy(x => x.Id)
                                .OrderBy(c => c.Id)
                                .LastOrDefault();




            if (Company == null)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.CompanyIsNotExist,
                    ErrorMessageEn = ErrorMessagesEn.CompanyIsNotExist,
                };

            if (Company.ManagerConfirmation != true)
            {
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.AccountNotActive,
                    ErrorMessageEn = ErrorMessagesEn.AccountNotActive,
                };
            }

            var isPeriodStared = Company.SubReqPeriods.OrderBy(c => c.Id).LastOrDefault().DateTo.Date > DateTime.Now.Date ? true : false; //.Where(x => x.SubReqPeriods.Where(x => x.DateTo >= DateTime.Now).Any()).Any();
            if (!isPeriodStared)
            {
                if (/*Company.SubReqPeriods.OrderBy(c => c.Id).LastOrDefault().DateTo.Date < DateTime.Now.Date ? true : false*/Company.SubReqPeriods.Where(x => x.DateTo <= DateTime.Now).Any())
                {
                    isPeriodEnded = true;
                }
                else
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = ErrorMessagesAr.PeriodIsNotStarted,
                        ErrorMessageEn = ErrorMessagesEn.PeriodIsNotStarted,
                    };
            }

            var appsID = Company.UserApplicationApps.Select(x => x.AppId).ToArray();
            var Allapp = _userManagementcontext.Apps.AsNoTracking().Include(h => h.AppParents)
                .Where(x => appsID.Contains(x.Id))
                .Select(x => new
                {
                    appNameAr = x.ArabicName,
                    appNameEn = x.LatinName,
                    x.Id
                });
            #region GetData
            var isCompanyExist = Company != null ? true : false;
            var apps = Allapp;

            var endTimeOfLastPeriod = Company.SubReqPeriods.OrderBy(x => x.Id).LastOrDefault().DateTo;
            var startTimeOfLastPeriod = Company.SubReqPeriods.OrderBy(x => x.Id).LastOrDefault().DateFrom;
            var DBName = Company.UserApplication.DatabaseName;
            #endregion

            if (!isCompanyExist)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.UsernameOrPasswordIsWrong,
                    ErrorMessageEn = ErrorMessagesEn.UsernameOrPasswordIsWrong,
                };
            #endregion









            #region set connection string and check is user valid 
            var connectionString = $"Data Source={_configuration["ApplicationSetting:serverName"]};" +
                                   $"Initial Catalog={DBName};" +
                                   $"user id={_configuration["ApplicationSetting:UID"]};" +
                                   $"password={_configuration["ApplicationSetting:Password"]};" +
                                   $"MultipleActiveResultSets=true;";
            _clientcontext.Database.SetConnectionString(connectionString);


            //"Data Source=144.24.219.185,59648;Initial Catalog=ERP_UsersManager;user id=sa;password=kr7rxc#E9Jm@ZFqV;MultipleActiveResultSets=true;TrustServerCertificate=True;"





            #region desk top pos commented
            //if (parm.isPOS)
            //{
            //    if (string.IsNullOrEmpty(parm.DeviceId))
            //        return new ResponseResult()
            //        {
            //            Result = Result.Failed,
            //            Note = Actions.DeviceIdisRequired
            //        };
            //    var POSDevices = _clientcontext.POSDevices.AsNoTracking().Where(x => !x.isDeleted);
            //    if (!POSDevices.Where(x => x.DeviceId == parm.DeviceId).Any())
            //    {
            //        if (!Company.LastOrDefault().IsInfinityNumbersOfPos)
            //        {
            //            var AdditionalPOS = Company.LastOrDefault().AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == 3).FirstOrDefault()?.Count ?? 0;
            //            if (Company.LastOrDefault().AllowedNumberOfPosofBundle + AdditionalPOS <= POSDevices.Count())
            //            {
            //                return new ResponseResult()
            //                {
            //                    Result = Result.Failed,
            //                    Note = Actions.YouHaveTheMaxmumOfPOS,
            //                    ErrorMessageAr = "لقد تخطيت الحد الاقصي من عدد نقاط البيع المتاحه ليدك",
            //                    ErrorMessageEn = "You have the maximum of the POS you can use for your bundle"
            //                };
            //            }
            //            else
            //            {
            //                _clientcontext.POSDevices.Add(new Domain.Entities.POS.POSDevices
            //                {
            //                    DeviceId = parm.DeviceId,
            //                    isActive = true,
            //                    isDeleted = false
            //                });
            //                _clientcontext.SaveChanges();
            //            }
            //        }

            //    }

            //}

            // var isUserExist = _clientcontext.userAccount.Where(x => EF.Functions.Collate(x.username,defultData.CollationsCaseSensitivity) == parm.username
            #endregion






            AlterDatabaseService.skipMigration(DBName, _configuration);

            var migrations = _clientcontext.Database.GetPendingMigrations();
            var isHavePendingMigration = migrations.Any();
            if (isHavePendingMigration)
            {
                await _clientcontext.Database.MigrateAsync();
            }
            var isUserExist = _clientcontext.userAccount
                .AsNoTracking()
                .Include(c=> c.employees)
                .Where(x => (EF.Functions.Collate(x.username, defultData.CollationsCaseSensitivity) == parm.username || EF.Functions.Collate(x.email, defultData.CollationsCaseSensitivity) == parm.username)
                                                                  && EF.Functions.Collate(x.password, defultData.CollationsCaseSensitivity) == parm.password)
                .FirstOrDefault();

            if (isUserExist == null)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.UsernameOrPasswordIsWrong,
                    ErrorMessageEn = ErrorMessagesEn.UsernameOrPasswordIsWrong,
                };
            if (!apps.Any())
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.ThisCompanyDoseNotHaveAnyApplication,
                    ErrorMessageEn = ErrorMessagesEn.ThisCompanyDoseNotHaveAnyApplication,
                };
            if (!isUserExist.isActive)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.UserIsNotActive,
                    ErrorMessageEn = ErrorMessagesEn.UserIsNotActive,
                };
            if (isTechnUser && isUserExist.id != 1)
                return new ResponseResult
                {
                    ErrorMessageAr = "دخول الدعم الفني يجب ان يكون علي حساب مدير النظام",
                    ErrorMessageEn = "Technical Support entry should be from super admin"
                };
            #endregion








            //skipe migration
            //var migrations = _clientcontext.Database.GetPendingMigrations();
            



            #region Checking Employee Info And Permission List 
            var empInfo = _clientcontext.employees.AsNoTracking().Where(x => x.Id == isUserExist.employeesId).FirstOrDefault();
            var _userPermissionId = _clientcontext.permissionLists
                .AsNoTracking()
                .Include(x => x.UserAndPermission)
                .Where(x => x.UserAndPermission.Where(d => d.userAccountId == isUserExist.id).Any())
                .FirstOrDefault();

            if (_userPermissionId == null)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.UserDoesNotHavePermission,
                    ErrorMessageEn = ErrorMessagesEn.UserDoesNotHavePermission,
                };
            var userPermissionId = _userPermissionId.UserAndPermission.FirstOrDefault();
            if (userPermissionId == null)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.UserDoesNotHavePermission,
                    ErrorMessageEn = ErrorMessagesEn.UserDoesNotHavePermission,
                };
            #endregion


            #region Update Database DefultData

            await _iUpdateService.UpdateDatabase(_clientcontext, webHostEnvironment , DBName);

            var settings = _clientcontext.invGeneralSettings.FirstOrDefault();

            if (settings.UpdateFilesNumber != Defults.UpdateFilesNumber)
            {

                await ReportFilesUpdate.UpdatePrintFiles(_clientcontext, webHostEnvironment);
                settings.UpdateFilesNumber = Defults.UpdateFilesNumber;
                _clientcontext.invGeneralSettings.Update(settings);
                _clientcontext.SaveChanges();
            }
            #endregion








            #region rules
            //set additional rules for super admin
            if (userPermissionId.permissionListId == 1)
            {
                var TotalRules = returnSubForms.returnRules(true, 1).Where(c => !_clientcontext.rules.AsNoTracking().Where(x => x.permissionListId == 1).Select(x => x.subFormCode).Contains(c.subFormCode));
                if (TotalRules.Any())
                {
                    _clientcontext.rules.AddRange(TotalRules);
                    _clientcontext.SaveChanges();
                }

            }
            var appIds = apps.Select(x => x.Id).ToArray();
            var _rules = _clientcontext.rules.AsNoTracking()
                .Where(x => x.permissionListId == userPermissionId.permissionListId)
                
                .ToList()
                .GroupBy(x => x.subFormCode)
                .Select(x => x.FirstOrDefault());


            var isExistKitchen = _rules.Where(a => a.subFormCode == (int)SubFormsIds.Kitchens).FirstOrDefault();
            if (isExistKitchen == null)
            {
                _rules=_rules.Append(returnSubForms.returnRules().Where(a => a.subFormCode == (int)SubFormsIds.Kitchens).FirstOrDefault());
            }
            var Listrules = _rules.ToList();
           
            //var anyRules = Listrules.Where(x => x.isShow).Any();
            if (!Listrules.Where(x => x.isShow).Any() && !isPeriodEnded)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.UserDoesNotHavePermission,
                    ErrorMessageEn = ErrorMessagesEn.UserDoesNotHavePermission,
                };


            var permissionInfo = _clientcontext.permissionLists.AsNoTracking().Where(x => x.Id == userPermissionId.permissionListId).FirstOrDefault();
            var userInfo = new userInfo()
            {
                userId = isUserExist.id.ToString(),
                userName = isUserExist.username,
                arabicName = empInfo.ArabicName,
                latinName = empInfo.LatinName,
                permissionNameAr = permissionInfo != null ? permissionInfo.arabicName : null,
                permissionNameEn = permissionInfo != null ? permissionInfo.latinName : null,
                imageUrl = isUserExist.employees.ImagePath
            };
            #endregion





            #region Token Generation
            int offlinePOSServerDeviceId = 0;
            if (parm.isPOS && parm.DeviceId.Split('-')[1] == defultData.offlineDevPassword)
            {
                if (isUserExist.id != 1)
                    return new ResponseResult
                    {
                        ErrorMessageAr = "تسجيل الدخول الاوفلاين من خلال مدير النظام فقط",
                        ErrorMessageEn = "Offline login for super admin only",
                        Result = Result.Failed
                    };
                if (!Allapp.Any(c => c.Id == 1))
                    return new ResponseResult
                    {
                        ErrorMessageAr = "يجب الاشتراك في برنامج المخزون",
                        ErrorMessageEn = "You should subscribe with store application",
                        Result = Result.Failed
                    };
                string RequestedDevice = parm.DeviceId.Split('-')[0];
                var saved_Devices = _clientcontext.POSDevices.AsNoTracking().Where(c => c.DeviceId == RequestedDevice);
                if (saved_Devices.Any())
                {
                    offlinePOSServerDeviceId = saved_Devices.FirstOrDefault().Id;
                }
                else
                {
                    var NewDevice = new Domain.Entities.POS.POSDevices
                    {
                        DeviceId = parm.DeviceId.Split('-')[0],
                        isActive = true,
                        isDeleted = false
                    };
                    _clientcontext.POSDevices.Add(NewDevice);
                    _clientcontext.SaveChanges();
                    offlinePOSServerDeviceId = NewDevice.Id;
                }

            }
            var token = await iAuthService.getAuthToken(isUserExist.employeesId.ToString(), userInfo, "", DBName, endTimeOfLastPeriod.ToString(), parm.companyName, parm.isPOS, isPeriodEnded, isTechnUser);

            var signinlogs = new signinLogs()
            {
                userAccountid = isUserExist.id,
                signinDateTime = DateTime.Now,
                token = token.token,
                lastActionTime = DateTime.Now,
                stayLoggedin = (parm.isPOS && parm.DeviceId.Split('-')[1] == defultData.offlineDevPassword) || isTechnUser ? true : false
            };
            //delete old logs
            var dateBefore5Days = DateTime.Now.Subtract(TimeSpan.FromDays(5));
            var oldsigninLogs = _clientcontext.signinLogs.Where(x => x.signinDateTime < dateBefore5Days);
            _clientcontext.signinLogs.RemoveRange(oldsigninLogs);
            _clientcontext.signinLogs.Add(signinlogs);
            await _clientcontext.SaveChangesAsync();
            #endregion






            #region Set Defult Branch
            var empBranchs = _clientcontext.employeeBranch.AsNoTracking().Where(x => x.EmployeeId == isUserExist.employeesId);
            var lisOfEmpBranchs = new List<InvEmployeeBranch>();
            int index = 0;
            foreach (var branch in empBranchs)
            {
                branch.current = index == 0 ? true : false;
                lisOfEmpBranchs.Add(branch);
                index++;
            }

            _clientcontext.employeeBranch.UpdateRange(lisOfEmpBranchs);
            _clientcontext.SaveChanges();
            #endregion








            
            var glSettings = _clientcontext.gLGeneralSettings.AsNoTracking().FirstOrDefault();
           
            var itemFundClosed = settings.Funds_Items;
            var customerFundClosed = settings.Funds_Customers;
            var suppliersFundClosed = settings.Funds_Supplires;
            var safesFundClosed = settings.Funds_Safes;
            var bankfundClosed = settings.Funds_Banks;
            var GLFundClosed = glSettings.isFundsClosed;
            Listrules.ForEach(x =>
            {
                if (itemFundClosed && x.subFormCode == (int)SubFormsIds.items_Fund)
                {
                    x.isShow = false;
                }
                if (customerFundClosed && x.subFormCode == (int)SubFormsIds.Customres_Fund)
                {
                    x.isShow = false;
                }
                if (suppliersFundClosed && x.subFormCode == (int)SubFormsIds.Suppliers_Fund)
                {
                    x.isShow = false;
                }
                if (safesFundClosed && x.subFormCode == (int)SubFormsIds.Safes_Fund)
                {
                    x.isShow = false;
                }
                if (bankfundClosed && x.subFormCode == (int)SubFormsIds.Banks_Fund)
                {
                    x.isShow = false;
                }
                if (GLFundClosed && x.subFormCode == (int)SubFormsIds.OpeningBalance_GL)
                {
                    x.isShow = false;
                }


                if(!x.isVisible)
                    x.isShow = false;
                if (!appIds.Contains(x.applicationId) && x.applicationId != 0)
                    x.isShow = false;
                if(x.subFormCode == (int)SubFormsIds.Kitchens)
                    x.isShow= false;
                if (x.subFormCode == (int)SubFormsIds.Printers)
                    x.isShow = false;
            });
            if (isPeriodEnded)
            {
                Listrules.ForEach(x =>
                {
                    if (x.Id != (int)SubFormsIds.GetCompanySubscriptionInformation || !x.isVisible ) 
                        x.isShow = false;
                    else
                        x.isShow = true;
                });
            }

            var rules = Listrules.Select(x => new
            {
                Id = x.subFormCode,
                x.isAdd,
                x.isEdit,
                x.isDelete,
                x.isShow,
                x.isPrint
            }).OrderBy(x => x.Id)
            .GroupBy(x => x.Id)
            .Select(y => y.FirstOrDefault());

            var coInfo = _clientcontext.companyData.FirstOrDefault();


            //POS Printing Files
            byte[] PosPrintFilesAr = null;
            byte[] PosPrintFilesEn = null;
            byte[] ReturnPosPrintFilesAr = null;
            byte[] ReturnPosPrintFilesEn = null;
            if (parm.isPOS)
            {
                var files = _clientcontext.reportFiles.AsNoTracking().Where(x => x.ReportFileName == "POSInvoice" || x.ReportFileName == "ReturnPOSInvoice");
                PosPrintFilesAr = files.First(x => x.IsArabic && x.ReportFileName == "POSInvoice").Files;
                PosPrintFilesEn = files.First(x => !x.IsArabic && x.ReportFileName == "POSInvoice").Files;
                ReturnPosPrintFilesAr = files.First(x => x.IsArabic && x.ReportFileName == "ReturnPOSInvoice").Files;
                ReturnPosPrintFilesEn = files.First(x => !x.IsArabic && x.ReportFileName == "ReturnPOSInvoice").Files;
            }

            if (!isTechnUser)
            {
                //Logout notification
                var signalRConnections = _clientcontext.signalR.AsNoTracking().ToHashSet();
                await NotificationUser(parm.companyName, isUserExist.employeesId, signalRConnections);
            }


            //Log in History
            await _systemHistoryLogsService.SystemHistoryLogsServiceLogin(isUserExist.employeesId, lisOfEmpBranchs.Where(x => x.current == true).FirstOrDefault().BranchId, isTechnUser);

            //response
            return new ResponseResult()
            {
                Result = Result.Success,
                Data = new loginResponse()
                {
                    isPeriodEnded = isPeriodEnded,
                    apps = apps,
                    authToken = token,
                    endPeriod = endTimeOfLastPeriod,
                    startPeriod = startTimeOfLastPeriod,
                    isHaveUpdate = isHavePendingMigration,
                    Premissions = rules,
                    companyInfo = new companyInfo()
                    {
                        companyNameAr = coInfo.ArabicName,
                        companyNameEn = coInfo.LatinName,
                        subId = Company.SubReqPeriods.OrderBy(c => c.Id).LastOrDefault().SubReqId
                    },
                    ServerID = offlinePOSServerDeviceId
                },
                PosPrintFilesAr = PosPrintFilesAr,
                PosPrintFilesEn = PosPrintFilesEn,
                ReturnPosPrintFilesAr = ReturnPosPrintFilesAr,
                ReturnPosPrintFilesEn = ReturnPosPrintFilesEn
            };
        }
        public async Task DemandLimitNotification(int userId, int employeeId)
        {
            var userStores = _clientcontext.otherSettingsStores.AsNoTracking().Include(x => x.otherSettings).Include(x => x.otherSettings.userAccount)
                                                               .Where(x => x.otherSettings.userAccount.id == userId).Select(c => c.InvStpStoresId).ToArray();
            var signalR = _clientcontext.signalR.AsNoTracking().Where(x => x.InvEmployeesId == employeeId).FirstOrDefault();
            if (signalR == null)
                return;
            var stores = _clientcontext.stores.AsNoTracking();
            var ListDemandLimitNotificationResponse = new List<DemandLimitNotificationResponse>();
            foreach (var storeId in userStores)
            {
                var checkStoreItems = _clientcontext.invoiceDetails.AsNoTracking()
                                                                   .Include(x => x.InvoicesMaster)
                                                                   .Include(x => x.Items)
                                                                   .Include(x => x.Items.Stores)
                                                                   .Where(x => x.InvoicesMaster.StoreId == storeId)
                                                                   .Where(x => x.Items.Stores.Where(c => c.StoreId == storeId).First().DemandLimit != 0);
                var currentStore = stores.Where(x => x.Id == storeId).FirstOrDefault();
                if (checkStoreItems.Any())
                {
                    var itemsCount = checkStoreItems.Count();
                    ListDemandLimitNotificationResponse.Add(new DemandLimitNotificationResponse()
                    {
                        storeId = storeId,
                        arabicNote = $"يوجد تنبيه بحد الطلب في مخزن {currentStore.ArabicName} لعدد {itemsCount} صنف",
                        latinNote = $"There is Notification of Demand Limit in Store : {currentStore.LatinName} for {itemsCount} of items"
                    });
                }

            }
            if (ListDemandLimitNotificationResponse.Any())
                await _hub.Clients.Clients(signalR.connectionId).SendAsync(defultData.DemandLimit, ListDemandLimitNotificationResponse);
        }
        private async Task NotificationUser(string companyName, int employeeId, HashSet<signalR> signalRs)
        {
            var userConnection = signalRs.Where(x => x.InvEmployeesId == employeeId && x.isOnline == true);
            if (userConnection.Any())
            {
                var LogoutResponse = new ResponseResult()
                {
                    Result = Result.logout,
                    ErrorMessageEn = "Another Person Has Login Into Your Account From Another Place",
                    ErrorMessageAr = "تم تسجيل الدخول الي حسابكم من مكان اخر"
                };
                await _hub.Clients.Clients(userConnection.FirstOrDefault().connectionId).SendAsync(defultData.LogoutNotification, LogoutResponse);
            }
        }
    }
    public class companyInformationRequest
    {
        public string CompanyLogin { get; set; }
        public string SecurityKey { get; set; }
    }
    public class loginResponse
    {
        public bool isPeriodEnded { get; set; }
        public DateTime startPeriod { get; set; }
        public DateTime endPeriod { get; set; }
        public object apps { get; set; }
        public object authToken { get; set; }
        public bool isHaveUpdate { get; set; }
        public object Premissions { get; set; }
        public companyInfo companyInfo { get; set; }
        public int updateNumber { get; set; } = Defults.updateNumber;
        public int ServerID { get; set; }
    }
    public class clinetLoginInformationResponse
    {
        public bool isCompanyExist { get; set; }
        public bool isHavePeriod { get; set; }
        public string dbName { get; set; }
        public object Apps { get; set; }
        public DateTime endTimeOfLastPeriod { get; set; }
        public DateTime startTimeOfLastPeriod { get; set; }
    }
    public class companyInfo
    {
        public string companyNameAr { get; set; }
        public string companyNameEn { get; set; }
        public int subId { get; set; }
    }
    public class DemandLimitNotificationResponse
    {
        public int storeId { get; set; }
        public string arabicNote { get; set; }
        public string latinNote { get; set; }
    }
}
