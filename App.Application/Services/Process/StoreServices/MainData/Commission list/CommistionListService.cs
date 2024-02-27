using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Commission_list
{
   public class CommistionListService : BaseClass , ICommistionListService
    {
        private readonly IRepositoryQuery<InvCommissionList> CommListQuery;
        private readonly IRepositoryCommand<InvCommissionList> CommListCommand;
        private readonly IRepositoryQuery<InvCommissionSlides> CommSlideQuery;
        private readonly IRepositoryCommand<InvCommissionSlides> CommSlideCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IHistory<InvCommissionListHistory> history;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<InvGeneralSettings> settingsQuery;
        public CommistionListService(IRepositoryQuery<InvCommissionList> _CommListQuery
                                        , IRepositoryCommand<InvCommissionList> _CommListCommand
                                        , IRepositoryQuery<InvCommissionSlides> _CommSlideQuery
                                        , IRepositoryCommand<InvCommissionSlides> _CommSlideCommand 
                                        ,ISystemHistoryLogsService systemHistoryLogsService
                                        , IHistory<InvCommissionListHistory> history
                                        , IRepositoryQuery<InvGeneralSettings> settingsQuery
                                        , IHttpContextAccessor _HttpContext)
                                        : base(_HttpContext)
        {
            CommListQuery = _CommListQuery;
            CommListCommand = _CommListCommand;
            CommSlideQuery = _CommSlideQuery;
            CommSlideCommand = _CommSlideCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
             httpContext = _HttpContext;
            this.settingsQuery = settingsQuery;
        }

        public async Task<ResponseResult> AddCommissionList(CommissionListRequest parameter )
        {
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;

            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };


            var ArabicCommExist = await CommListQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName  );
            if (ArabicCommExist != null)
                return new ResponseResult() { Data = null, Id = ArabicCommExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var LatinCommExist = await CommListQuery.GetByAsync(a =>  a.LatinName == parameter.LatinName);
            if (LatinCommExist != null)
                return new ResponseResult() { Data = null, Id = LatinCommExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            if (parameter.Type == (int)CommissionType.Slides && parameter.Slides.Count == 0)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData   };

            var settings = await settingsQuery.GetByAsync(a => 1 == 1);
            if (parameter.Type == (int)CommissionType.Slides)
            {

                int counter = 1;
                foreach (var slide in parameter.Slides)
                {
                    if (slide.SlideTo < slide.SlideFrom)
                        return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.SlideToLessThanSlideFrom };
                    //var commissionValue = Math.Round(((slide.SlideTo - slide.SlideFrom) / 100) * slide.Ratio, settings.Other_Decimals);
                   if(slide.isSetUserRatio)
                    {
                        var commissionValue = Math.Round(((slide.SlideTo - slide.SlideFrom) / 100) * slide.Ratio, settings.Other_Decimals);
                        if (commissionValue != slide.Value)
                            return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.ErrorInCalculations };

                    }
                    else
                    {
                        var commissionRatio = Math.Round((slide.Value*100)/ (slide.SlideTo - slide.SlideFrom) , settings.Other_Decimals);
                        if (commissionRatio != slide.Ratio)
                            return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.ErrorInCalculations };

                    }

                    counter++;
                }
            }
                int NextCode = 1;
                if(CommListQuery.Count() !=0)
                   NextCode = CommListQuery.GetMaxCode(e => e.Code) + 1;

                // var table = Mapping.Mapper.Map<CommissionListRequest, InvCommissionList>(parameter);
                var CommList = new InvCommissionList();
                CommList.ArabicName = parameter.ArabicName;
                CommList.LatinName = parameter.LatinName;
                CommList.Type = parameter.Type;
            if (parameter.Type == (int)CommissionType.Fixed)
                CommList.Ratio = parameter.Ratio;
            else if (parameter.Type == (int)CommissionType.Slides)
                CommList.Ratio = null;

                CommList.Target = parameter.Target;
              
                CommList.Code = NextCode;

          var res=    CommListCommand.Add(CommList);
        
            if (parameter.Type == (int)CommissionType.Slides)
                {
                    List<InvCommissionSlides> commSlides = new List<InvCommissionSlides>();
                    int counter = 1;

                    foreach (var slide in parameter.Slides)
                   {

                    InvCommissionSlides commslide = new InvCommissionSlides();
                  
                       commslide.CommissionId = CommList.Id;
                       commslide.SlideNo = counter;
                       commslide.SlideFrom = slide.SlideFrom;
                       commslide.SlideTo = slide.SlideTo;
                       commslide.Ratio = slide.Ratio;
                       commslide.Value = slide.Value;
                       counter++;
                       commSlides.Add(commslide);
                   }
                if(res)
                {
                     CommSlideCommand.AddRange(commSlides);
                   await CommSlideCommand.SaveAsync();
                }
                  
                }
             

            history.AddHistory(CommList.Id, CommList.LatinName, CommList.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addCommissionList);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }


        public async Task<ResponseResult> GetCommissionList(CommissionListSearch parameters)
        {

            var totalCount = CommListQuery.Count();
            var resData = await CommListQuery.GetAllIncludingAsync(0, 0
                                , a => (a.Code.ToString().Contains(parameters.Name) ||
                                string.IsNullOrEmpty(parameters.Name) ||
                                a.ArabicName.Contains(parameters.Name) ||
                                a.LatinName.Contains(parameters.Name)),
                                 e => (string.IsNullOrEmpty(parameters.Name) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1))
                                 , e => e.Slides, s => s.SalesMan);

            resData.Where(a => a.SalesMan.Count() == 0).Select(a => { a.CanDelete = true; return a; }).ToList();
            resData.Select(a => { a.SalesMan = null; return a; }).ToList();

            var count = resData.Count();
            if (parameters.PageSize > 0&& parameters.PageNumber > 0) 
            {
                resData = resData.Skip((parameters.PageNumber-1 ) * parameters.PageSize ).Take(parameters.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            return new ResponseResult() { Data = resData, DataCount=count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed , TotalCount= totalCount };
        }

        public async Task<ResponseResult> UpdateCommissionList(UpdateCommissionListRequest parameters)
        {
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };


            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;
            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            var ArabicCommExist = await CommListQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);
            if (ArabicCommExist != null)
                return new ResponseResult() { Data = null, Id = ArabicCommExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };
            var LatinCommExist = await CommListQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (LatinCommExist != null)
                return new ResponseResult() { Data = null, Id = LatinCommExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            var settings = await settingsQuery.GetByAsync(a => 1 == 1);
            if (parameters.Type == (int)CommissionType.Slides)
            {

                int counter = 1;
                foreach (var slide in parameters.Slides)
                {
                    if (slide.SlideTo < slide.SlideFrom)
                        return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.SlideToLessThanSlideFrom };
                    if (slide.isSetUserRatio)
                    {
                        var commissionValue = Math.Round(((slide.SlideTo - slide.SlideFrom) / 100) * slide.Ratio, settings.Other_Decimals);
                        if (commissionValue != slide.Value)
                            return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.ErrorInCalculations };

                    }
                    else
                    {
                        var commissionRatio = Math.Round((slide.Value * 100) / (slide.SlideTo - slide.SlideFrom), settings.Other_Decimals);
                        if (commissionRatio != slide.Ratio)
                            return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.ErrorInCalculations };

                    }
                    counter++;
                }
            }



            var data = await CommListQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            data.Id = parameters.Id;
            data.ArabicName = parameters.ArabicName;
            data.LatinName = parameters.LatinName;
            data.Type = parameters.Type;
            if (parameters.Type == (int)CommissionType.Fixed)
                data.Ratio = parameters.Ratio;
            else if (parameters.Type == (int)CommissionType.Slides)
                data.Ratio = null;

            data.Target = parameters.Target;
           
            //data.Code = parameters.Code;
 
            await CommListCommand.UpdateAsyn(data);

            await CommSlideCommand.DeleteAsync(a => a.CommissionId == parameters.Id);

            if (parameters.Type == (int)CommissionType.Slides)
            {
                List<InvCommissionSlides> commSlides = new List<InvCommissionSlides>();
                int counter = 1;
               // var settings = await settingsQuery.GetByAsync(a => 1 == 1);
                foreach (var slide in parameters.Slides)
                {
                    //if (slide.SlideTo < slide.SlideFrom)
                    //    return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.SlideToLessThanSlideFrom };
                    //var commissionValue = Math.Round(((slide.SlideTo - slide.SlideFrom) / 100) * slide.Ratio, settings.Other_Decimals);
                    //if (commissionValue != slide.Value )
                    //    return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.ErrorInCalculations };

                    InvCommissionSlides commslide = new InvCommissionSlides();

                    commslide.CommissionId = parameters.Id;
                    commslide.SlideNo = counter;
                    commslide.SlideFrom = slide.SlideFrom;
                    commslide.SlideTo = slide.SlideTo;
                    commslide.Ratio = slide.Ratio;
                    commslide.Value = slide.Value;
                    counter++;
                    commSlides.Add(commslide);

                }
                //CommSlideCommand.AddRange(commSlides);
                //CommSlideCommand.SaveAsync();

                CommSlideCommand.AddRange(commSlides);
                await CommSlideCommand.SaveAsync();
            }
            history.AddHistory(data.Id, data.LatinName, data.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCommissionList);
            return new ResponseResult() { Data = null, Id =null, Result = Result.Success };

        }


        //public async Task<ResponseResult> UpdateActiveCommissionList(UpdateActiveCommissionList parameters)
        //{
        //    var CommissionLists = CommListQuery.TableNoTracking.Where(e => parameters.CommissionList.Contains(e.CommissionId));
        //    var CommissionList = CommissionLists.ToList();

        //    CommissionList.Select(e => { e.ac = parameters.Active; return e; }).ToList();
        //    if (parameters.CommissionList.Contains(1))
        //        ColorsList.Where(q => q.ColorId == 1).Select(e => { e.Active = (int)Status.Active; return e; }).ToList();
        //    var rssult = await ColorsRepositoryCommand.UpdateAsyn(ColorsList);
        //    foreach (int comm in parameters.CommissionList)
        //    {
        //        var data = await CommListQuery.GetByIdAsync(comm);
        //        if (data == null)
        //            continue;


        //        await CommListCommand.UpdateAsyn(data);
        //        string browserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
        //        AddCommissionListHistory(data.CommissionId, data.ArabicName, data.LatinName, data.Type, data.Ratio, data.Target, data.Active, browserName, "U", null);

        //    }
        //    return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        //}

        public async Task<ResponseResult> DeleteCommissionList(SharedRequestDTOs.Delete ListCode)
        {
                await CommSlideCommand.DeleteAsync(a => ListCode.Ids.Contains(a.CommissionId ));
            await CommListCommand.DeleteAsync(a => ListCode.Ids.Contains(a.Id));
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteCommissionList);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }

       

        public async Task<ResponseResult> GetCommissionListHistory(int CommissionId)
        {
            return await history.GetHistory( a=>a.EntityId==CommissionId);

        }

        public async Task<ResponseResult> GetCommissionListDropDown()
        {
            var UnitsList = CommListQuery.TableNoTracking.Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName });

            return new ResponseResult() { Data = UnitsList, Id = null, Result = UnitsList.Any() ? Result.Success : Result.Failed };

        }

     /*   public ResponseResult CheckSlides( List<CommissionSlidesRequest> slides  , int settingOfDecimal)
        {
            
                int counter = 1;
                foreach (var slide in slides)
                {
                    if (slide.SlideTo < slide.SlideFrom)
                        return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.SlideToLessThanSlideFrom };
                    var commissionValue = Math.Round(((slide.SlideTo - slide.SlideFrom) / 100) * slide.Ratio, settingOfDecimal);
                    if (commissionValue != slide.Value)
                        return new ResponseResult() { Data = null, Id = counter, Result = Result.Failed, Note = Actions.ErrorInCalculations };

                    counter++;
                }
            

        }*/

    }
}
