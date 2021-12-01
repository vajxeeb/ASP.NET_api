using Microsoft.EntityFrameworkCore;
using Models.DataContext;
using Models.Utility;
using Npgsql;
using Services.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;


namespace Services.DataService
{
    public class SaleService : GenericRepository<Bill>, ISaleService
    {
        private readonly EfDbContext _context;
        public SaleService(EfDbContext context) : base(context)
        {
            _context = context;
        }

        public string GetCurrentPeriodNumber()
        {
            var drawNumber = "";
            drawNumber = _context.Online.Where(t => t.online_status == 1).OrderByDescending(t => t.date_online).ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();
            return drawNumber;
        }

        public object GetConfigData()
        {
            object obj = new
            {
                periodNumber = "",
                maxDigitLength = 0,
            };
            try
            {
                var v_periodNumber = _context.Online.Where(t => t.online_status == 1).OrderByDescending(t => t.date_online).ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();
                var v_maxDigitLength = _context.DigitLenght.FirstOrDefault().max_lenght;
                obj = new
                {
                    periodNumber = v_periodNumber,
                    maxDigitLength = v_maxDigitLength,
                    randomDigitList = new List<DigitRandom>() //_context.DigitRandom.ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }

        public void CheckNumberPrice(string lotteryNumber, int lotteryPrice)
        {
            var max_lenght = _context.DigitLenght.FirstOrDefault().max_lenght;
            if (lotteryNumber.Length > max_lenght)
                throw new Exception($"ເລກສ່ຽງສາມາດຂາຍໄດ້ {max_lenght} ຫຼັກເທົ່ານັ້ນ");

            if (lotteryPrice < 1000)
                throw new Exception($"Price can\'t be lower 1000 kip");

            if (lotteryPrice % 1000 != 0)
                throw new Exception($"price should be multiple 1000 kip");

            var ln_Data = _context.LotteryNumber.FirstOrDefault(t => t.lottery_number == lotteryNumber);

            if (ln_Data != null)
            {
                if (ln_Data.ln_status != 1)
                    throw new Exception($"ເລກສ່ຽງ {lotteryNumber} ເຕັມແລ້ວ");
                //throw new Exception($"{lotteryNumber} is over limit sale");

                var db_lottery_price = _context.BillDetail.Where(t => Convert.ToInt32(t.lottery_number) == Convert.ToInt32(lotteryNumber) && t.date_bill_detail.Date == DateTime.Today.Date).Sum(t => t.lottery_price);

                var realTimeMaxPrice = ln_Data.max_sell - db_lottery_price;

                if (db_lottery_price == 0)
                {
                    if (lotteryPrice > ln_Data.max_sell)
                        throw new Exception($"ເລກສ່ຽງ {lotteryNumber} ສາມາດຊື້ໄດ້ {realTimeMaxPrice} ຕາມທີ່ກຳນົດໄວ້");
                    //throw new Exception($"{lotteryNumber} can buy {realTimeMaxPrice} please modify it");
                }
                else if (realTimeMaxPrice == 0)
                {
                    throw new Exception($"can not sell {lotteryNumber}");
                }
                else if (lotteryPrice > realTimeMaxPrice)
                {
                    throw new Exception($"{lotteryNumber} can buy {realTimeMaxPrice} please modify it");
                }

            }
        }

        public List<SaleViewModel> GetSellSetNumber(string lotteryNumber, int lotteryPrice)
        {
            List<SaleViewModel> saleViewModelList = new List<SaleViewModel>();
            var max_lenght = _context.DigitLenght.FirstOrDefault().max_lenght;
            if (lotteryNumber.Length <= max_lenght)
            {
                if (lotteryNumber.Length < 3)
                {
                    var lotteryData = _context.SaleSetNumber.Where(t => t.lottery_number == lotteryNumber).FirstOrDefault();

                    if (lotteryData == null)
                    {
                        SaleViewModel saleViewModel = new SaleViewModel
                        {
                            lotteryNumber = lotteryNumber,
                            lotteryPrice = lotteryPrice
                        };
                        saleViewModelList.Add(saleViewModel);
                    }
                    else
                    {
                        var saleSetNumberList = _context.SaleSetNumber.Where(t => t.lottery_name == lotteryData.lottery_name && t.lottery_digit == lotteryNumber.Length).ToList();
                        var lotteryNumberArray = saleSetNumberList.Select(t => Convert.ToInt32(t.lottery_number)).ToList();
                        var ln_list = _context.LotteryNumber.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number))).ToList();

                        foreach (var item in saleSetNumberList)
                        {
                            var ln_Data = ln_list.FirstOrDefault(t => t.lottery_number == item.lottery_number);

                            if (ln_Data != null)
                            {
                                if (ln_Data.ln_status == 1)
                                {
                                    SaleViewModel saleViewModel = new SaleViewModel
                                    {
                                        lotteryNumber = item.lottery_number,
                                        lotteryPrice = lotteryPrice
                                    };
                                    saleViewModelList.Add(saleViewModel);
                                }
                            }
                            else
                            {
                                SaleViewModel saleViewModel = new SaleViewModel
                                {
                                    lotteryNumber = item.lottery_number,
                                    lotteryPrice = lotteryPrice
                                };
                                saleViewModelList.Add(saleViewModel);
                            }
                            if (saleViewModelList.Count == 12)
                                break;
                        }
                    }
                }
                else
                {
                    string substring = lotteryNumber[(lotteryNumber.Length - 2)..];
                    string prefixstring = lotteryNumber[0..^2];

                    var lotteryName = _context.SaleSetNumber.Where(t => t.lottery_number == substring).FirstOrDefault().lottery_name;

                    if (string.IsNullOrEmpty(lotteryName))
                    {
                        SaleViewModel saleViewModel = new SaleViewModel
                        {
                            lotteryNumber = lotteryNumber,
                            lotteryPrice = lotteryPrice
                        };
                        saleViewModelList.Add(saleViewModel);
                    }
                    else
                    {
                        var saleSetNumberDb = _context.SaleSetNumber.Where(t => t.lottery_name == lotteryName).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        var saleSetNumberList = _context.SaleSetNumber.Where(t => t.lottery_name == saleSetNumberDb.lottery_name && t.lottery_digit == saleSetNumberDb.lottery_digit)
                                                //.OrderBy(x => Guid.NewGuid()).Take(12).ToList();
                                                .OrderBy(x => Guid.NewGuid()).ToList();

                        var lotteryNumberArray = saleSetNumberList.Select(t => Convert.ToInt32(t.lottery_number)).ToList();

                        var ln_list = _context.LotteryNumber.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number))).ToList();
                        var lotteryPriceList = _context.BillDetail.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number)) && t.date_bill_detail.Date == DateTime.Today.Date).ToList();


                        foreach (var item in saleSetNumberList)
                        {
                            var db_lottery_price = lotteryPriceList.Where(t => Convert.ToInt32(t.lottery_number) == Convert.ToInt32(item.lottery_number)).Sum(t => t.lottery_price);
                            var ln_Data = ln_list.FirstOrDefault(t => t.lottery_number == item.lottery_number);

                            if (ln_Data != null)
                            {
                                if ((ln_Data.max_sell - db_lottery_price) >= lotteryPrice)
                                {
                                    if (ln_Data.ln_status == 1)
                                    {
                                        SaleViewModel saleViewModel = new SaleViewModel
                                        {
                                            lotteryNumber = prefixstring + item.lottery_number,
                                            lotteryPrice = lotteryPrice
                                        };
                                        saleViewModelList.Add(saleViewModel);
                                    }
                                }
                            }
                            else
                            {
                                SaleViewModel saleViewModel = new SaleViewModel
                                {
                                    lotteryNumber = prefixstring + item.lottery_number,
                                    lotteryPrice = lotteryPrice
                                };
                                saleViewModelList.Add(saleViewModel);
                            }
                            if (saleViewModelList.Count == 12)
                                break;

                        }
                    }
                }

            }
            return saleViewModelList;

        }

        public bool IsOverMaxSell(string deviceCode)
        {
            try
            {
                string periodNumber = _context.Online.Where(t => t.online_status == 1).OrderByDescending(t => t.date_online).ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();

                var billSum = _context.Bill.Where(t => t.device_code == deviceCode && t.period_number == periodNumber).Sum(t => t.bill_price);
                var maxSell = _context.DeviceMaxSell.FirstOrDefault(t => t.device_code == deviceCode).max_sell;
                if (maxSell == 0) return true;

                return billSum > maxSell;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public Tuple<int, string> CheckOverMaxSellOrBlancePrice(int lotteryNumberLength, int lotteryPrice, int totalPrice, string deviceCode, string periodNumber)
        {
            try
            {
                int errorType = 0;
                string message = "";

                var cancelList = _context.BillCancel.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).ToList();

                var billSum = _context.Bill.Where(t => t.device_code == deviceCode && t.period_number == periodNumber
                            && !cancelList.Select(c => c.bill_number).Contains(t.bill_number)).Sum(t => t.bill_price);
                var maxSell = _context.DeviceMaxSell.FirstOrDefault(t => t.device_code == deviceCode).max_sell;


                if (maxSell == 0) errorType = 1; // re-login
                else if (billSum > maxSell) errorType = 1; // re-login
                else if (billSum + totalPrice + lotteryPrice > maxSell) // show balance_max_sell
                {
                    errorType = 2;
                    message = $"ຍອດເຫຼືອໃນການຂາຍແມ່ນ {billSum + totalPrice - maxSell}";
                }

                return new Tuple<int, string>(errorType, message);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(2, ex.Message);
            }
        }

        public string InsertSale(string deviceCode, int deviceNumber, string periodNumber, List<SaleViewModel> saleViewModelList)
        {
            var newbillId = "";
            try
            {
                // check period_number online
                if (!_context.Online.Any(t => t.period_number == periodNumber && t.online_status == 1))
                    throw new Exception("Invalid draw number");

                var lotteryNumberArray = saleViewModelList.Select(t => Convert.ToInt32(t.lotteryNumber)).ToList();

                var ln_list = _context.LotteryNumber.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number))).ToList();
                var max_lenght = _context.DigitLenght.FirstOrDefault().max_lenght;

                var lotteryPriceList = _context.BillDetail.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number)) && t.date_bill_detail.Date == DateTime.Today.Date).ToList();


                foreach (var item in saleViewModelList)
                {
                    if (item.lotteryNumber.Length > max_lenght)
                        throw new Exception($"{item.lotteryNumber} can't over {max_lenght} digit");

                    if (item.lotteryPrice < 1000)
                        throw new Exception($"Price can\'t be lower 1000 kip");

                    if (item.lotteryPrice % 1000 != 0)
                        throw new Exception($"price should be multiple 1000 kip");

                    var ln_Data = ln_list.FirstOrDefault(t => t.lottery_number == item.lotteryNumber);

                    if (ln_Data != null)
                    {
                        //throw new Exception($"invalid lottery number {item.lotteryNumber}");

                        if (ln_Data.ln_status != 1)
                            throw new Exception($"{item.lotteryNumber} number is over limit sale");

                        var db_lottery_price = lotteryPriceList.Where(t => Convert.ToInt32(t.lottery_number) == Convert.ToInt32(item.lotteryNumber)).Sum(t => t.lottery_price);

                        var realTimeMaxPrice = ln_Data.max_sell - db_lottery_price;

                        if (db_lottery_price == 0)
                        {
                            if (item.lotteryPrice > ln_Data.max_sell)
                                throw new Exception($"{item.lotteryNumber} can buy {realTimeMaxPrice} please modify it");
                        }
                        else if (realTimeMaxPrice == 0)
                        {
                            throw new Exception($"can not sell {item.lotteryNumber}");
                        }
                        else if (item.lotteryPrice > realTimeMaxPrice)
                        {
                            throw new Exception($"{item.lotteryNumber} can buy {realTimeMaxPrice} please modify it");
                        }

                    }
                }

                var billAutoNumber = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).Count() + 1;
                newbillId = periodNumber + deviceCode + (billAutoNumber.ToString().Length == 1 ? "000" + billAutoNumber : "00" + billAutoNumber);

                var devRefCode = _context.Device.FirstOrDefault(t => t.device_code == deviceCode).device_ref;

                var user = _context.UserSeller.Where(t => t.device_code == deviceCode).FirstOrDefault();

                var bill = new Bill
                {
                    bill_id = Guid.NewGuid(),
                    bill_number = newbillId,
                    period_number = periodNumber,
                    device_code = deviceCode,
                    device_ref = devRefCode,
                    bill_price = saleViewModelList.Sum(t => t.lotteryPrice),
                    date_bill = DateTime.Now,
                    time_bill = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                    branch_id = user.branch_id,
                    unit_id = user.unit_id,
                    ref_code = deviceNumber
                };
                _context.Add(bill);

                var list = new List<BillDetail>();
                foreach (var item in saleViewModelList)
                {
                    var model = new BillDetail
                    {
                        bill_id = bill.bill_id,
                        bill_number = newbillId,
                        lottery_number = item.lotteryNumber,
                        lottery_price = item.lotteryPrice,
                        date_bill_detail = DateTime.Today.Date
                    };
                    list.Add(model);
                }
                _context.AddRange(list);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return newbillId;
        }

        #region version 2

        public List<SaleViewModel> GetSellSetNumberV2(string lotteryNumber, int lotteryPrice)
        {
            List<SaleViewModel> saleViewModelList = new List<SaleViewModel>();
            var max_lenght = _context.DigitLenght.FirstOrDefault().max_lenght;
            if (lotteryNumber.Length <= max_lenght)
            {
                if (lotteryNumber.Length < 3)
                {
                    var lotteryData = _context.SaleSetNumber.Where(t => t.lottery_number == lotteryNumber).FirstOrDefault();

                    if (lotteryData == null)
                    {
                        SaleViewModel saleViewModel = new SaleViewModel
                        {
                            lotteryNumber = lotteryNumber,
                            lotteryPrice = lotteryPrice
                        };
                        saleViewModelList.Add(saleViewModel);
                    }
                    else
                    {
                        var saleSetNumberList = _context.SaleSetNumber.Where(t => t.lottery_name == lotteryData.lottery_name && t.lottery_digit == lotteryNumber.Length).ToList();

                        foreach (var item in saleSetNumberList)
                        {
                            SaleViewModel saleViewModel = new SaleViewModel
                            {
                                lotteryNumber = item.lottery_number,
                                lotteryPrice = lotteryPrice
                            };
                            saleViewModelList.Add(saleViewModel);
                            if (saleViewModelList.Count == 12)
                                break;
                        }
                    }
                }
                else
                {
                    string substring = lotteryNumber[(lotteryNumber.Length - 2)..];
                    string prefixstring = lotteryNumber[0..^2];

                    var lotteryName = _context.SaleSetNumber.Where(t => t.lottery_number == substring).FirstOrDefault().lottery_name;

                    if (string.IsNullOrEmpty(lotteryName))
                    {
                        SaleViewModel saleViewModel = new SaleViewModel
                        {
                            lotteryNumber = lotteryNumber,
                            lotteryPrice = lotteryPrice
                        };
                        saleViewModelList.Add(saleViewModel);
                    }
                    else
                    {
                        var saleSetNumberDb = _context.SaleSetNumber.Where(t => t.lottery_name == lotteryName).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        var saleSetNumberList = _context.SaleSetNumber.Where(t => t.lottery_name == saleSetNumberDb.lottery_name && t.lottery_digit == saleSetNumberDb.lottery_digit)
                                                //.OrderBy(x => Guid.NewGuid()).Take(12).ToList();
                                                .OrderBy(x => Guid.NewGuid()).ToList();

                        foreach (var item in saleSetNumberList)
                        {
                            SaleViewModel saleViewModel = new SaleViewModel
                            {
                                lotteryNumber = prefixstring + item.lottery_number,
                                lotteryPrice = lotteryPrice
                            };
                            saleViewModelList.Add(saleViewModel);

                            if (saleViewModelList.Count == 12)
                                break;

                        }
                    }
                }

            }
            return saleViewModelList;

        }

        public Tuple<int, string, object> InsertSaleV2(string deviceCode, int deviceNumber, string periodNumber, List<SaleViewModel> saleViewModelList)
        {
            object obj;
            var newbillId = "";
            List<string> removeBillNumberList = new List<string>();
            List<SaleViewModel> newSaleViewModelList = new List<SaleViewModel>(saleViewModelList);

            int errorType = 0;
            string message = "";

            try
            {
                // check period_number online
                if (!_context.Online.Any(t => t.period_number == periodNumber && t.online_status == 1))
                    throw new Exception("Invalid draw number");

                var lotteryNumberArray = saleViewModelList.Select(t => Convert.ToInt32(t.lotteryNumber)).ToList();

                var ln_list = _context.LotteryNumber.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number))).ToList();
                var max_lenght = _context.DigitLenght.FirstOrDefault().max_lenght;

                var lotteryPriceList = _context.BillDetail.Where(t => lotteryNumberArray.Contains(Convert.ToInt32(t.lottery_number)) && t.date_bill_detail.Date == DateTime.Today.Date).ToList();

                var quotaList = _context.Quota.ToList();


                foreach (var item in newSaleViewModelList)
                {
                    bool flag = false;
                    var quotaFunction = _context.QuotaFunction.FromSqlRaw($"SELECT * FROM fn_quota('{periodNumber}','{item.lotteryNumber}')").FirstOrDefault();

                    /// •	When lottery number found status_balance or lottery_status is “false” mean this number has over balance or this number has block.
                    /// •	Do not insert this number into database but show this number after print.
                    if (quotaFunction != null)
                    {
                        if (quotaFunction.bal_stat.ToLower() == "true" && quotaFunction.lot_stat.ToLower() == "true")
                        {
                            if (item.lotteryPrice > quotaFunction.lot_bal)
                            {
                                saleViewModelList.Remove(item);
                                removeBillNumberList.Add(item.lotteryNumber);
                                flag = true;
                            }
                        }
                        else
                        {
                            saleViewModelList.Remove(item);
                            removeBillNumberList.Add(item.lotteryNumber);
                            flag = true;
                        }
                    }
                    else
                    {
                        var quotaData = quotaList.Where(t => t.digit_lenght == item.lotteryNumber.Length).FirstOrDefault();
                        if (quotaData != null && item.lotteryPrice > quotaData.price_per_number)
                        {
                            saleViewModelList.Remove(item);
                            removeBillNumberList.Add(item.lotteryNumber);
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        #region MyRegion CheckNumberPrice

                        if (item.lotteryNumber.Length > max_lenght)
                            //throw new Exception($"{item.lotteryNumber} can't over {max_lenght} digit");
                            throw new Exception($"ເລກສ່ຽງສາມາດຂາຍໄດ້ {max_lenght} ຫຼັກເທົ່ານັ້ນ");

                        if (item.lotteryPrice < 1000)
                            throw new Exception($"Price can\'t be lower 1000 kip");

                        if (item.lotteryPrice % 1000 != 0)
                            throw new Exception($"price should be multiple 1000 kip");

                        var ln_Data = ln_list.FirstOrDefault(t => t.lottery_number == item.lotteryNumber);

                        if (ln_Data != null)
                        {
                            //throw new Exception($"invalid lottery number {item.lotteryNumber}");

                            if (ln_Data.ln_status != 1)
                                //throw new Exception($"{item.lotteryNumber} number is over limit sale");
                                throw new Exception($"ເລກສ່ຽງ {item.lotteryNumber} ເຕັມແລ້ວ");

                            var db_lottery_price = lotteryPriceList.Where(t => Convert.ToInt32(t.lottery_number) == Convert.ToInt32(item.lotteryNumber)).Sum(t => t.lottery_price);

                            var realTimeMaxPrice = ln_Data.max_sell - db_lottery_price;

                            if (db_lottery_price == 0)
                            {
                                if (item.lotteryPrice > ln_Data.max_sell)
                                    //throw new Exception($"{item.lotteryNumber} can buy {realTimeMaxPrice} please modify it");
                                    throw new Exception($"ເລກສ່ຽງ {item.lotteryNumber} ສາມາດຊື້ໄດ້ {realTimeMaxPrice} ຕາມທີ່ກຳນົດໄວ້");
                            }
                            else if (realTimeMaxPrice == 0)
                            {
                                throw new Exception($"can not sell {item.lotteryNumber}");
                            }
                            else if (item.lotteryPrice > realTimeMaxPrice)
                            {
                                throw new Exception($"{item.lotteryNumber} can buy {realTimeMaxPrice} please modify it");
                            }

                        }
                        #endregion CheckNumberPrice
                    }
                }

                #region CheckOverMaxSellOrBlancePrice

                //var cancelList = _context.BillCancel.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).ToList();

                //var billSum = _context.Bill.Where(t => t.device_code == deviceCode && t.period_number == periodNumber
                //                        && !cancelList.Select(c => c.bill_number).Contains(t.bill_number)).Sum(t => t.bill_price);
                //var deviceMaxSell = _context.DeviceMaxSell.FirstOrDefault(t => t.device_code == deviceCode).max_sell;


                //var totalPrice = saleViewModelList.Sum(t => t.lotteryPrice);

                //if (deviceMaxSell == 0) errorType = 1; // re-login
                //else if (billSum > deviceMaxSell) errorType = 1; // re-login
                //else if (billSum + totalPrice > deviceMaxSell) // show balance_max_sell
                //{
                //    errorType = 2;
                //    message = $"ຍອດເຫຼືອໃນການຂາຍແມ່ນ {billSum + totalPrice - deviceMaxSell}";
                //}

                #endregion CheckOverMaxSellOrBlancePrice

                if (errorType == 0 && saleViewModelList.Count > 0)
                {
                    var billAutoNumber = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).Count() + 1;
                    newbillId = periodNumber + deviceCode + (billAutoNumber.ToString().Length == 1 ? "000" + billAutoNumber : "00" + billAutoNumber);

                    var devRefCode = _context.Device.FirstOrDefault(t => t.device_code == deviceCode).device_ref;

                    var user = _context.UserSeller.Where(t => t.device_code == deviceCode).FirstOrDefault();

                    var bill = new Bill
                    {
                        bill_id = Guid.NewGuid(),
                        bill_number = newbillId,
                        period_number = periodNumber,
                        device_code = deviceCode,
                        device_ref = devRefCode,
                        bill_price = saleViewModelList.Sum(t => t.lotteryPrice),
                        date_bill = DateTime.Now,
                        time_bill = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                        branch_id = user.branch_id,
                        unit_id = user.unit_id,
                        ref_code = deviceNumber
                    };
                    _context.Add(bill);

                    var list = new List<BillDetail>();
                    foreach (var item in saleViewModelList)
                    {
                        var model = new BillDetail
                        {
                            bill_id = bill.bill_id,
                            bill_number = newbillId,
                            lottery_number = item.lotteryNumber,
                            lottery_price = item.lotteryPrice,
                            date_bill_detail = DateTime.Today.Date
                        };
                        list.Add(model);
                    }
                    _context.AddRange(list);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            obj = new
            {
                newbillId,
                saleViewModelList,
                totalPrice = saleViewModelList.Count > 0 ? saleViewModelList.Sum(t => t.lotteryPrice) : 0,
                removeBillNumberList = removeBillNumberList.ToArray()
            };

            return new Tuple<int, string, object>(errorType, message, obj);
        }

        public List<SaleSetNumber> GetSellSetNumberList()
        {
            return _context.SaleSetNumber.ToList();
        }
        #endregion version 2

    }
}