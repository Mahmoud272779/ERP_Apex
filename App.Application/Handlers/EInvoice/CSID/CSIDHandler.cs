using App.Infrastructure;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Models.Security.Authentication.Response.Store.EmployeeResponsesDTOs;

namespace App.Application.Handlers.EInvoice.CSID
{
    public class CSIDHandler : IRequestHandler<CSIDRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvCompanyData> _InvCompanyDataQuery;
        private readonly IRepositoryQuery<GLBranch> _GLBranchQuery;
        private readonly IRepositoryQuery<App.Domain.Entities.Process.Store.EInvoice.CSID> _CSIDQuery;
        private readonly IRepositoryCommand<App.Domain.Entities.Process.Store.EInvoice.CSID> _CSIDCommand;
        private readonly IConfiguration _configuration;
        public CSIDHandler(IRepositoryQuery<InvCompanyData> invCompanyDataQuery, IConfiguration configuration, IRepositoryQuery<Domain.Entities.Process.Store.EInvoice.CSID> cSIDQuery, IRepositoryCommand<Domain.Entities.Process.Store.EInvoice.CSID> cSIDCommand, IRepositoryQuery<GLBranch> gLBranchQuery)
        {
            _InvCompanyDataQuery = invCompanyDataQuery;
            _configuration = configuration;
            _CSIDQuery = cSIDQuery;
            _CSIDCommand = cSIDCommand;
            _GLBranchQuery = gLBranchQuery;
        }
        public async Task<ResponseResult> Handle(CSIDRequest request, CancellationToken cancellationToken)
        {
            var branch = _GLBranchQuery.TableNoTracking.FirstOrDefault(c => c.Id == request.branchId);
            //validation 
            if (branch == null)
                return new ResponseResult
                {
                    Result =  Result.Failed,
                    Alart =
                            new Alart
                            {
                                AlartType = AlartType.error,
                                type = AlartShow.popup,
                                MessageAr = ErrorMessagesAr.ItemNotExist,
                                MessageEn = ErrorMessagesEn.ItemNotExist,
                                titleAr = "حدث خطا",
                                titleEn = "Error"
                            }
                };
            var data = _InvCompanyDataQuery.TableNoTracking
                .Select(c => new CSIDCompanyDataResDTO
                {
                    Id = c.Id,
                    ArabicName = c.ArabicName,
                    LatinName = c.LatinName,
                    FieldAr = c.FieldAr,
                    FieldEn = c.FieldEn,
                    CommercialRegister = c.CommercialRegister,
                    TaxNumber = c.TaxNumber,
                    Phone1 = c.Phone1,
                    Phone2 = c.Phone2,
                    Fax = c.Fax,
                    Website = c.Website,
                    Email = c.Email,    
                    Notes = c.Notes,
                    LatinAddress = c.LatinAddress,
                    ArabicAddress = c.ArabicAddress,
                    OTP = request.OTP,
                    BranchAddress = branch.AddressAr,
                    BranchName = branch.ArabicName
                })
                .FirstOrDefault();
            var jsonRequest = JsonConvert.SerializeObject(data);
            jsonRequest = "1^" + jsonRequest;
            var CSIDRes = TcpListenerServices.Send(jsonRequest, _configuration,tcpType.EInvoice);
            
            
            string path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "CSID", DateTime.Now.ToString("yyyyMMddTHHmmss") + ".txt");
           
            var TCPResponeObj = JsonConvert.DeserializeObject<CSIDResponseResultResponseDTO>(CSIDRes);

            var elem = _CSIDQuery.TableNoTracking.FirstOrDefault();
            if(elem != null)
            {
                elem.otp = request.OTP;
                elem.cSIDKey = TCPResponeObj.CSID;
                elem.error_ar = TCPResponeObj.Error_ar;
                elem.error_en = TCPResponeObj.Error_en;
                elem.sucess = string.IsNullOrEmpty(TCPResponeObj.Error_en);
                elem.branchId = request.branchId;
                elem.cSR = TCPResponeObj.CSR;
                elem.privateKey = TCPResponeObj.PrivateKey;
                elem.publicKey = TCPResponeObj.PublicKey;
                elem.secretNumber = TCPResponeObj.SecretNumber;

                _CSIDCommand.Update(elem);
            }
            else
            {
                _CSIDCommand.Add(new Domain.Entities.Process.Store.EInvoice.CSID
                {
                    otp = request.OTP,
                    cSIDKey = TCPResponeObj.CSID,
                    error_en = TCPResponeObj.Error_en,
                    error_ar = TCPResponeObj.Error_ar,
                    sucess = string.IsNullOrEmpty(TCPResponeObj.Error_en),  
                    branchId = request.branchId,
                    cSR = TCPResponeObj.CSR,
                    privateKey = TCPResponeObj.PrivateKey,
                    publicKey = TCPResponeObj.PublicKey,
                    secretNumber = TCPResponeObj.SecretNumber,
                });
            }
            var saved = await _CSIDCommand.SaveChanges();
            return new ResponseResult
            {
                Data = TCPResponeObj,
                Result = string.IsNullOrEmpty(TCPResponeObj.Error_en) ? Result.Success : Result.Failed,
                Alart = string.IsNullOrEmpty(TCPResponeObj.Error_en) ? null : 
                new Alart
                {
                    AlartType = AlartType.error,
                    type = AlartShow.popup,
                    MessageAr = TCPResponeObj.Error_ar,
                    MessageEn = TCPResponeObj.Error_en,
                    titleAr = "حدث خطا",
                    titleEn = "Error"
                }
            };
        }
    }
    public class CSIDCompanyDataResDTO
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string FieldAr { get; set; }
        public string FieldEn { get; set; }
        public string CommercialRegister { get; set; }
        public string TaxNumber { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string LatinAddress { get; set; }
        public string ArabicAddress { get; set; }
        public string OTP { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
    }
    public class CSIDResponseResultResponseDTO
    {
        public string CSR, CSID, PrivateKey, PublicKey, SecretNumber, Error_ar, Error_en;
    }
    
}
