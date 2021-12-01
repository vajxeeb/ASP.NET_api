using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.ViewModels
{
    public class Response
    {
        public Response()
        {
            Status = true;
            StatusCode = StatusCode.Success;
            Message = StatusCode.Success.ToString();
        }
        public bool Status { get; set; }
        public StatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; }
        public object Data { get; set; }
    }

    public enum StatusCode
    {
        Success = 200,
        Created = 201,
        NoDataAvailable=204,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500,
        BlanceMaxSell = 1000,
        OverMaxSell = 1001,
        QuotaMaxValues = 1002,
    }
}
