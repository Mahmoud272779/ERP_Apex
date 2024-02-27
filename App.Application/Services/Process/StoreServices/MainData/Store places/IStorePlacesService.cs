using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Store_places
{
    public interface IStorePlacesService
    {
        Task<ResponseResult> AddStorePlace(StorePlacesParameter parameter);
        Task<ResponseResult> GetListOfStorePlaces(StorePlacesSearch parameters);
        Task<ResponseResult> UpdateStorePlaces(UpdateStorePlacesParameter parameters);
        Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters);
        Task<ResponseResult> DeleteStorePlaces(SharedRequestDTOs.Delete ListCode);
        Task<ResponseResult> GetStorePlaceHistory(int StorePlaceId);
        Task<ResponseResult> GetStorePlacesDropDown();
        Task<ResponseResult> GetAllStorePlacesDropDown();
    }
}
