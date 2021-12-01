using Library.Core.Infrastructure;
using Library.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.DataService;
using System;

namespace WebApp.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class NumberStatusController : ControllerBase
    {
        private readonly IRetrieveDataFromTokenService _retrieveDataFromTokenService;
        private readonly INumberStatusService _saleService;

        public NumberStatusController(IRetrieveDataFromTokenService retrieveDataFromTokenService, INumberStatusService saleService)
        {
            _retrieveDataFromTokenService = retrieveDataFromTokenService;
            _saleService = saleService;
        }

        [HttpGet("getnumberlist")]
        public IActionResult GetNumberList()
        {
            Response response = new Response();
            try
            {
                //var tokenHolder = _retrieveDataFromTokenService.GetClaims();
                var data = _saleService.GetNumberList();

                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = data.Item2;
                response.TotalRecords = data.Item1;
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
