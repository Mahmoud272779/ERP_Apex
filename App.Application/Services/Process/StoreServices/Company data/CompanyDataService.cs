using App.Application.Helpers.Service_helper.FileHandler;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Company_data
{
    public class CompanyDataService : BaseClass, ICompanyDataService
    {
        readonly private IRepositoryQuery<InvCompanyData> CompanyQuery;
        private readonly IFileHandler _fileHandler;
        readonly private IRepositoryCommand<InvCompanyData> CompanyCommand;
        readonly private IHttpContextAccessor httpContext;

        public CompanyDataService(IRepositoryQuery<InvCompanyData> _CompanyQuery,
                                    IFileHandler fileHandler
                                  , IRepositoryCommand<InvCompanyData> _CompanyCommand
                                  , IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            CompanyQuery = _CompanyQuery;
            _fileHandler = fileHandler;
            CompanyCommand = _CompanyCommand;
            httpContext = _httpContext;
        }

        public async Task<ResponseResult> UpdateCompanyData(UpdateCompanyDataRequest parameters)
        {
            //if (parameters.Id == 0)
            //    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.arabicName == null)
                parameters.arabicName = "";

            if (parameters.latinName == null)
                parameters.latinName = "";

            if (string.IsNullOrEmpty(parameters.arabicName.Trim()))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (string.IsNullOrEmpty(parameters.latinName.Trim()))
                parameters.latinName = parameters.arabicName.Trim();

            parameters.latinName = parameters.latinName.Trim();
            parameters.arabicName = parameters.arabicName.Trim();

            var data = await CompanyQuery.GetByAsync(a => a.Id == 1);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            data.ArabicName = parameters.arabicName;
            data.CommercialRegister = parameters.commercialRegister;
            data.Email = parameters.email;
            data.Fax = parameters.fax;
            data.FieldAr = parameters.fieldAr;
            data.FieldEn = parameters.fieldEn;
            data.LatinName = parameters.latinName;
            data.Notes = parameters.notes;
            data.Phone1 = parameters.phone1;
            data.Phone2 = parameters.phone2;
            data.TaxNumber = parameters.taxNumber;
            data.Website = parameters.website;
            data.LatinAddress = parameters.latinAddress;
            data.ArabicAddress = parameters.arabicAddress;
            if (parameters.changeImage)
            {
                if (parameters.image != null)
                {
                    data.Image = _fileHandler.SaveImage(parameters.image, "CompanyData", true);
                    data.imageFile = null;
                }
                else
                {
                    data.Image = "";
                    data.imageFile = null;

                }
            }
            await CompanyCommand.UpdateAsyn(data);

            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }

        public async Task<ResponseResult> GetCompanyData(bool fromSystem = false)
        {
            var resData = await CompanyQuery.GetByIdAsync(1);


            var dataDto = new CompanyDataDto();

            dataDto.Id = resData.Id;
            dataDto.ArabicName = resData.ArabicName;
            dataDto.LatinName = resData.LatinName;
            dataDto.FieldAr = resData.FieldAr;
            dataDto.FieldEn = resData.FieldEn;
            dataDto.TaxNumber = resData.TaxNumber;
            dataDto.CommercialRegister = resData.CommercialRegister;
            dataDto.Phone1 = resData.Phone1;
            dataDto.Phone2 = resData.Phone2;
            dataDto.Fax = resData.Fax;
            dataDto.Email = resData.Email;
            dataDto.Website = resData.Website;
            dataDto.Notes = resData.Notes;
            dataDto.ArabicAddress = resData.ArabicAddress;
            dataDto.LatinAddress = resData.LatinAddress;

            if (resData.Image != null)
            {
                dataDto.Image = resData.Image;
                if (fromSystem)
                {
                    //dataDto.imageFile = resData.imageFile;
                }

            }



            return new ResponseResult() { Data = dataDto, Id = null, Result = Result.Success };

        }


    }
}
