using Library.Core.Infrastructure;
using Library.Core.Models.Securities;
using Library.Core.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Services;
using System;
using System.Net.Http.Headers;

namespace WebApp.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class DashBoardController : ControllerBase
    {
        private readonly IOnlineService _onlineService;
        private readonly IRetrieveDataFromTokenService _retrieveDataFromTokenService;

        public DashBoardController(IOnlineService onlineService, IRetrieveDataFromTokenService retrieveDataFromTokenService)
        {
            _onlineService = onlineService;
            _retrieveDataFromTokenService = retrieveDataFromTokenService;
        }

        [HttpGet("get")]
        public ActionResult Get(string deviceCode, string drawNumber)
        {
            //var tokenHolder = _retrieveDataFromTokenService.GetClaims();
            return Ok(_onlineService.Get(deviceCode, drawNumber));
        }

        [HttpGet("get/billlist")]
        public ActionResult GetBillList(string deviceCode, string drawNumber)
        {
            Response response = new Response();

            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();

                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.GetBillList(deviceCode, drawNumber);
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

        [HttpGet("get/billdetaillist")]
        public ActionResult GetBillDetailList(string billId) //change
        {
            Response response = new Response();

            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();

                response.Status = true;
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.GetBillDetailList(billId);
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


        [HttpGet("get/cancelbilllist")]
        public ActionResult GetCancelBillList(string deviceCode,string drawNumber)
        {
            Response response = new Response();

            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();

                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.GetCancelBillList(deviceCode, drawNumber);
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

        [HttpGet("get/cancelbilldetaillist")]
        public ActionResult GetCancelBillDetailList(string billId)//change
        {
            Response response = new Response();

            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();

                response.Status = true;
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.GetCancelBillDetailList(billId);
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


        [HttpGet("get/billdetaillistbydigit")]
        public ActionResult GeBillDetailListByDigit(string deviceCode,string digit, string drawNumber)
        {
            Response response = new Response();

            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();

                response.Status = true;
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.GeBillDetailListByDigit(deviceCode, drawNumber, digit);
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
