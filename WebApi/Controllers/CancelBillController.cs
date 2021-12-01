using Library.Core.Infrastructure;
using Library.Core.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DataService;
using System;
using System.Collections.Generic;
using ViewModels;

namespace WebApp.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CancelBillController : ControllerBase
    {
        private readonly IRetrieveDataFromTokenService _retrieveDataFromTokenService;
        private readonly ICancelBillService _cancelBillService;
        private readonly IOnlineService _onlineService;

        public CancelBillController(IRetrieveDataFromTokenService retrieveDataFromTokenService
            , ICancelBillService cancelBillService
            , IOnlineService onlineService
            )
        {
            _retrieveDataFromTokenService = retrieveDataFromTokenService;
            _cancelBillService = cancelBillService;
            _onlineService = onlineService;
        }

        [HttpGet("get")]
        public IActionResult Get(string deviceCode)
        {
            Response response = new Response();
            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();
                if (!_onlineService.IsSeverOnline())
                {
                    response.Status = false;
                    response.StatusCode = Library.Core.ViewModels.StatusCode.Unauthorized;
                    response.Message = "Server is offline.";
                    response.Data = new object { };
                    return Ok(response);
                }
                var data = _cancelBillService.GetCancelBill(deviceCode);
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = data;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = Library.Core.ViewModels.StatusCode.InternalServerError;
                response.Message = ex.Message;
                response.Data = new object { };
            }
            return Ok(response);
        }

        [HttpPost("cancel/add")]
        public IActionResult Add(string deviceCode, int deviceNumber, string billId, string reasonCancel)
        {
            Response response = new Response();
            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();
                if (!_onlineService.IsSeverOnline())
                {
                    response.Status = false;
                    response.StatusCode = Library.Core.ViewModels.StatusCode.Unauthorized;
                    response.Message = "Server is offline.";
                    response.Data = new object { };
                    return Ok(response);
                }
                _cancelBillService.InsertCancelBill(deviceCode, deviceNumber, billId, reasonCancel);
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = new object { };
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = Library.Core.ViewModels.StatusCode.InternalServerError;
                response.Message = ex.Message;
                response.Data = new object { };
            }
            return Ok(response);
        }

    }
}
