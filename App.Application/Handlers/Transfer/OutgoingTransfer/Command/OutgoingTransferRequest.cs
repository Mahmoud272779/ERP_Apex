using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using System.Text.Json.Serialization;

namespace App.Application.Handlers.Transfer
{

    public class AddOutgoingTransferRequest : InvoiceMasterRequest, IRequest<ResponseResult>
    {


    }
    public class UpdateOutgoingTransferRequest : UpdateInvoiceMasterRequest, IRequest<ResponseResult>
    {


    }
    public class getByIdTransferRequest : IRequest<ResponseResult>
    {
        public int Id { get; set; }

    }
    public class DeleteTransferRequest : IRequest<ResponseResult>
    {
        public int[] Ids { get; set; }

    }
    public class GetAllOutgoingByStoreID : IRequest<ResponseResult>
    {
        public int StoreId { get; set; }
        public int pageSize { get; set; }
        public int PageNumber { get; set; }

    }
    public class GetAllOutgoingTransferRequest :  IRequest<ResponseResult>
    {
        public string Code { get; set; }
        public List<int> StoreId { get; set; }
        public List<int> StoreIdTo { get; set; }
        public List<int> transferStatus { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        [JsonIgnore]
        public int docTypeId { get; set; }
        [JsonIgnore]
        public int DeletedDocTypeId { get; set; }



    }
}
