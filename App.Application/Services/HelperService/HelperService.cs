using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Domain.Entities.Process;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Context;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces;
using App.Infrastructure.Interfaces.Repository;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService
{
    public class HelperService : BaseClass, IHelperService
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationReadDbConnection _readDbConnection;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IApplicationOracleDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryQuery<InvGeneralSettings> _SettingRepositoryQuery;
        //private readonly IRoundNumbers roundNumbers;

        public HelperService(
            IHttpContextAccessor httpContextAccessor, 
            IMemoryCache memoryCache, 
            IApplicationReadDbConnection readDbConnection,
            IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
            IRepositoryQuery<InvGeneralSettings> SettingRepositoryQuery,
            //IRoundNumbers roundNumbers,
            IApplicationOracleDbContext context) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _readDbConnection = readDbConnection;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _context = context;
            _memoryCache = memoryCache;
            _SettingRepositoryQuery = SettingRepositoryQuery;
           
        }
        public async Task<DropdownLists> FillDropDowns(List<int> pagesListID)
        {
          // _context.Connection.Open();
            using (var transaction = _context.Connection.BeginTransaction())
            {
                try
                {
                    _context.Database.UseTransaction(transaction as DbTransaction);
                    var parameters = new { Branch_Id = 1 };
                    DropdownLists DropdownListData = new DropdownLists();
                    var tasks = new List<Task<IReadOnlyCollection<LookUps>>>();
                    var watch1 = new System.Diagnostics.Stopwatch();
                    watch1.Start();

                    if (pagesListID.Contains((int)DropDownSelection.Units))
                    {
                        tasks.Add(Task.Run(async () => DropdownListData.Units = (await _context.Connection.QueryAsync<LookUps>("select UnitId ID,UnitCode Code,LatinName LatinName,ArabicName ArabicName from InvStpUnits whree BranchId = {0} and Active = true",1, transaction)).AsList()));
                        //tasks.Add(Task.Run(async () => DropdownListData.Projects = await _readDbConnection.QueryAsync<LookUps>("select PROJECT_ID ID,PROJECT_CODE Code,LATIN_NAME LatinName,ARABIC_NAME ArabicName from STP_PROJECT",transaction)));
                    }

                    var data = await Task.WhenAll(tasks.ToArray());
                        watch1.Stop();
                        var time1 = watch1.ElapsedMilliseconds;
                    return DropdownListData;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _context.Connection.Close();
                }
            }
           
        }
        int Reverse(int x)
        {
            if (x != 0 && x <= Int16.MaxValue && x >= Int32.MinValue)
            {
                int num = 0;
                while(x > 0)
                {
                    num = (x % 10) + (num * 10); 
                    x = x / 10;
                }
                return num;
            }
            else
            {
                return 0;
            }    

        }
        public string convertListToString(List<string> ListData)
        {
            string tempStr = "";
            int counter = 1;
            foreach (var item in ListData)
            {
                if(counter== ListData.Count)
                {
                    tempStr += item;

                }
                else
                {
                    tempStr += item + ",";
                }
                counter++;
            }
            return tempStr;
        }
        public List<string> convertStringToList(string strData)
        {
            List<string> templist = new List<string>();
            if (strData != null)
            {
                var data = strData.Split(',');
                foreach (var item in data)
                {
                    templist.Add(item);
                }
            }

            return templist;
        }

        // فى مشكله فى تعدد قواعد البيانات 
        public async Task<InvGeneralSettings> GetAllSettings()
        {
          return  await GetAllSettings(false);
        }
            public async Task<InvGeneralSettings> GetAllSettings( bool dataChanged)
        {
            var cacheKey = "GeneralSettingErp";
            if (dataChanged)
            {
                _memoryCache.Remove(cacheKey);
            }
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out InvGeneralSettings settingsList ))
            {

                //calling the server
                settingsList = await _SettingRepositoryQuery.SingleOrDefault(q=>q.Id==1);

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromDays(1)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, settingsList, cacheExpiryOptions);
            }
            return  settingsList ;
        }
        public async Task<InvGeneralSettings> GetAllGeneralSettings()
        {
            var setting =await _SettingRepositoryQuery.TableNoTracking.FirstOrDefaultAsync();
            return setting;
        }
        public async Task<double> GetFinanicalAccountTotalAmount(int id, string autoCoding, IQueryable<GLJournalEntryDetails> _C_D)
        {
            var C_D = _C_D
                .Where(x => (x.GLFinancialAccount.autoCoding.StartsWith((autoCoding+".")) || x.GLFinancialAccount.autoCoding == autoCoding) && x.journalEntry.IsBlock != true)
                .Select(x => new { x.Id, x.Credit, x.Debit, x.GLFinancialAccount.autoCoding }).OrderBy(x => x.Id).ToList();
            var credit = C_D.Select(x => x.Credit).Sum();
            var Debit = C_D.Select(x => x.Debit).Sum();
            var sum = credit - Debit;
             var MidpointRounding = _invGeneralSettingsQuery.TableNoTracking.First().Other_Decimals;

            var value = Numbers.RoundedUp(sum,MidpointRounding); 
            return value;
        }
    }
 }
