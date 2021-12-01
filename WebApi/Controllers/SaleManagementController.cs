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
    public class SaleManagementController : ControllerBase
    {
        private readonly IRetrieveDataFromTokenService _retrieveDataFromTokenService;
        private readonly ISaleService _saleService;
        private readonly IOnlineService _onlineService;

        public SaleManagementController(IRetrieveDataFromTokenService retrieveDataFromTokenService
            , ISaleService saleService
            , IOnlineService onlineService
            )
        {
            _retrieveDataFromTokenService = retrieveDataFromTokenService;
            _saleService = saleService;
            _onlineService = onlineService;
        }

        [HttpGet("getdrawnumber")]
        public IActionResult GetDrawNumber()
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
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _saleService.GetCurrentPeriodNumber();
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

        [HttpGet("getconfigdata")]
        public IActionResult GetConfigData()
        {
            Response response = new Response();
            try
            {
                if (!_onlineService.IsSeverOnline())
                {
                    response.Status = false;
                    response.StatusCode = Library.Core.ViewModels.StatusCode.Unauthorized;
                    response.Message = "Server is offline.";
                    response.Data = new object { };
                    return Ok(response);
                }
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "OK";
                response.Data = _saleService.GetConfigData();
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


        [HttpGet("checknumberprice")]
        public IActionResult CheckNumberPrice(string lotteryNumber, int lotteryPrice, int totalPrice, string deviceCode, string periodNumber)
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

                var maxSellData = _saleService.CheckOverMaxSellOrBlancePrice(lotteryNumber.Length, lotteryPrice, totalPrice, deviceCode, periodNumber);

                if (maxSellData != null)
                {
                    if (maxSellData.Item1 != 0)
                    {
                        response.Status = false;
                        response.StatusCode = maxSellData.Item1 == 1 ?
                                                Library.Core.ViewModels.StatusCode.OverMaxSell
                                                : (
                                                    maxSellData.Item1 == 2 ? Library.Core.ViewModels.StatusCode.BlanceMaxSell
                                                    : Library.Core.ViewModels.StatusCode.QuotaMaxValues
                                                );
                        response.Message = maxSellData.Item2;
                        response.Data = new object { };
                        return Ok(response);
                    }
                }

                _saleService.CheckNumberPrice(lotteryNumber, lotteryPrice);
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

        [HttpGet("getsellsetnumber")]
        public IActionResult GetSellSetNumber(string lotteryNumber, int lotteryPrice, int totalPrice, string deviceCode, string periodNumber)
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
                var data = _saleService.GetSellSetNumber(lotteryNumber, lotteryPrice);

                var maxSellData = _saleService.CheckOverMaxSellOrBlancePrice(lotteryNumber.Length, data.Count * lotteryPrice, totalPrice, deviceCode, periodNumber);

                if (maxSellData != null)
                {
                    if (maxSellData.Item1 != 0)
                    {
                        response.Status = false;
                        response.StatusCode = maxSellData.Item1 == 1 ? Library.Core.ViewModels.StatusCode.OverMaxSell : Library.Core.ViewModels.StatusCode.BlanceMaxSell;
                        response.Message = maxSellData.Item2;
                        response.Data = new object { };
                        return Ok(response);
                    }
                }

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

        [HttpPost("add/sale/{deviceCode}/{deviceNumber}")]
        public IActionResult Add(string deviceCode, int deviceNumber, [FromBody] BillViewModel billViewModel)
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
                var billNumber = _saleService.InsertSale(deviceCode, deviceNumber, billViewModel.periodNumber, billViewModel.SaleList);
                response.Status = true;
                response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                response.Message = "Successfull sale add";
                response.Data = billNumber;
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

        [HttpGet("getsellsetnumberv2")]
        public IActionResult GetSellSetNumberV2(string lotteryNumber, int lotteryPrice)
        {
            Response response = new Response();
            try
            {
                if (!_onlineService.IsSeverOnline())
                {
                    response.Status = false;
                    response.StatusCode = Library.Core.ViewModels.StatusCode.Unauthorized;
                    response.Message = "Server is offline.";
                    response.Data = new object { };
                    return Ok(response);
                }
                var data = _saleService.GetSellSetNumber(lotteryNumber, lotteryPrice);

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

        [HttpPost("addv2/sale/{deviceCode}/{deviceNumber}")]
        public IActionResult AddV2(string deviceCode, int deviceNumber, [FromBody] BillViewModel billViewModel)
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
                var billData = _saleService.InsertSaleV2(deviceCode, deviceNumber, billViewModel.periodNumber, billViewModel.SaleList);

                if (billData.Item1 == 0)
                {
                    response.Status = true;
                    response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                    response.Message = "Successfull sale add";
                    response.Data = billData.Item3;
                }
                else if (billData.Item1 != 0)
                {
                    response.Status = false;
                    response.StatusCode = billData.Item1 == 1 ?
                                            Library.Core.ViewModels.StatusCode.OverMaxSell :
                                            Library.Core.ViewModels.StatusCode.BlanceMaxSell;
                    response.Message = billData.Item2;
                    response.Data = new object { };
                }

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
