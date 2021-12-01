using Library.Core.Infrastructure;
using Library.Core.Models.Securities;
using Library.Core.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Services;
using Services.DataService;
using System;
using System.Net.Http.Headers;

namespace WebApp.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class HistorySalePeriodController : ControllerBase
    {
        private readonly IHistorySalePeriodService _historySalePeriodService;

        public HistorySalePeriodController(IHistorySalePeriodService historySalePeriodService)
        {
            _historySalePeriodService = historySalePeriodService;
        }

        [HttpGet("get")]
        public ActionResult Get(string deviceCode, string periodNumber)
        {
            return Ok(_historySalePeriodService.Get(deviceCode, periodNumber));
        }

    }
}
