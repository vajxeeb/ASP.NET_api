using Library.Core.Infrastructure;
using Library.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DataService;
using System;

namespace WebApp.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class PeriodController : ControllerBase
    {
        private readonly IPeriodService _onlineService;
        private readonly IRetrieveDataFromTokenService _retrieveDataFromTokenService;

        public PeriodController(IPeriodService onlineService, IRetrieveDataFromTokenService retrieveDataFromTokenService)
        {
            _onlineService = onlineService;
            _retrieveDataFromTokenService = retrieveDataFromTokenService;
        }

        [HttpGet("get")]
        public ActionResult Get(string deviceCode)
        {
            Response response = new Response();

            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();

                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.Get(deviceCode);
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


        [HttpGet("getv2")]
        public ActionResult GetV2(string deviceCode)
        {
            Response response = new Response();

            try
            {
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _onlineService.GetV2(deviceCode);
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
