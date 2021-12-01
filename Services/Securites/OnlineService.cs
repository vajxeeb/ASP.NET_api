using Models.DataContext;
using Models.Utility;
using Services.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class OnlineService : GenericRepository<Online>, IOnlineService
    {
        private readonly EfDbContext _context;
        public OnlineService(EfDbContext context) : base(context)
        {
            _context = context;
        }

        public bool IsSeverOnline()
        {
            try
            {
                var data = _context.Online.OrderByDescending(t => t.date_online).ThenByDescending(t => t.time_online).FirstOrDefault().online_status == 1;
                return data;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public object Get(string deviceCode, string drawNumber)
        {
            try
            {
                object obj;
                var periodNumber = "";
                if (!string.IsNullOrEmpty(drawNumber))
                {
                    periodNumber = drawNumber;
                }
                else
                {
                    periodNumber = _context.Online.Where(t => (t.online_status == 1 || t.online_status == 2)).OrderByDescending(t => t.date_online).ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();
                }
                var max_length = _context.DigitLenght.FirstOrDefault().max_lenght;

                var cancelList = _context.BillCancel.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).ToList();

                var totalSale = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode &&
                                                    !cancelList.Select(c => c.bill_number).Contains(t.bill_number)).Count();
                //var totalCancel = _context.BillCancel.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).Count();

                var billDetailList = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode && !cancelList.Select(c => c.bill_number).Contains(t.bill_number))
                    .Join(_context.BillDetail, b => b.bill_id, bd => bd.bill_id, (b, bd) => new
                    {
                        bill = b,
                        billDetails = bd
                    })
                    .Where(m => m.billDetails.lottery_number.Length <= max_length && m.bill.date_bill == m.billDetails.date_bill_detail)
                    .AsEnumerable()
                    .OrderBy(m => m.billDetails.lottery_number.Length)
                    .GroupBy(t => new { t.billDetails.lottery_number.Length })
                    .Select(g => new
                    {
                        key = g.Select(n => n.bill.bill_id.ToString()).FirstOrDefault(),
                        digit = g.Select(n => n.billDetails.lottery_number.Length).FirstOrDefault(),
                        price = g.Select(n => n.billDetails.lottery_price).Sum(),
                        billNumber = g.Select(n => n.bill.bill_number)
                    }).ToList();

                obj = new
                {
                    drawNumber = periodNumber,
                    totalSale,
                    totalCancel = cancelList.Count,
                    billDetailList
                };

                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetBillList(string deviceCode, string periodNumber)
        {
            object obj;
            try
            {
                var cancelList = _context.BillCancel.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).ToList();
                var dbBillList = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode &&
                                                !cancelList.Select(c => c.bill_number).Contains(t.bill_number))
                    .OrderByDescending(t => t.date_bill.Date).ThenByDescending(t => t.time_bill).ToList();

                var totalSale = dbBillList.Count();
                var totalPrice = dbBillList.Sum(t => t.bill_price);

                obj = new
                {
                    totalSale,
                    totalPrice,
                    billList = dbBillList.Select(t => new
                    {
                        key = t.bill_id,
                        date = t.date_bill.Date.ToString("dd/M/yyyy"),
                        time = t.time_bill.ToString(),
                        billNumber = t.bill_number,
                        billPrice = t.bill_price
                    })
                };

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetBillDetailList(string billId)
        {
            object obj;
            try
            {
                var dbData = _context.BillDetail.Where(t => t.bill_id == Guid.Parse(billId)).OrderByDescending(t => t.date_bill_detail.Date)
                   .Select(t => new
                   {
                       t.bill_number,
                       number = t.lottery_number,
                       price = t.lottery_price
                   }).ToList();

                obj = new
                {
                    billNumber = dbData.Select(t=>t.bill_number).FirstOrDefault(),
                    totalPrice = dbData.Sum(t => t.price),
                    list = dbData
                };

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public object GetCancelBillList(string deviceCode, string drawNumber)
        {
            object obj;
            try
            {
                var dbCancelList = _context.BillCancel.Where(t => t.period_number == drawNumber && t.device_code == deviceCode)
                    .OrderByDescending(t => t.date_cancel.Date).ThenByDescending(t => t.time_cancel).ToList();

                var totalSale = dbCancelList.Count();
                var totalPrice = dbCancelList.Sum(t => t.bill_price);

                obj = new
                {
                    totalSale,
                    totalPrice,
                    billList = dbCancelList.Select(t => new
                    {
                        key = t.cancel_id,
                        date = t.date_cancel.Date.ToString("dd/M/yyyy"),
                        time = t.time_cancel.ToString(),
                        billNumber = t.bill_number,
                        billPrice = t.bill_price
                    })
                };

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public object GetCancelBillDetailList(string billId)
        {
            object obj;
            try
            {
                var dbData = _context.BillCancelDetail.Where(t => t.cancel_id == Guid.Parse(billId)).OrderByDescending(t => t.date_bill_cancel_detail)
                   .Select(t => new
                   {
                       t.bill_number,
                       number = t.lottery_number,
                       price = t.lottery_price
                   }).ToList();

                obj = new
                {
                    billNumber = dbData.Select(t=>t.bill_number).FirstOrDefault(),
                    totalPrice = dbData.Sum(t => t.price),
                    list = dbData
                };

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GeBillDetailListByDigit(string deviceCode, string drawNumber, string digit)
        {
            object obj;
            try
            {
                var dbData = _context.Bill.Where(t => t.period_number == drawNumber && t.device_code == deviceCode)
                    .Join(_context.BillDetail, b => b.bill_number, bd => bd.bill_number, (b, bd) => new
                    {
                        bill = b,
                        billDetails = bd
                    })
                    .Where(m => m.billDetails.lottery_number.Length == Convert.ToInt32(digit) && m.bill.date_bill.Date == m.billDetails.date_bill_detail.Date)
                    .Select(t => new
                    {
                        t.bill.date_bill,
                        t.bill.time_bill,
                        t.billDetails.lottery_number,
                        t.billDetails.lottery_price
                    }).ToList();

                var totalSale = dbData.Count();
                var totalPrice = dbData.Sum(t => t.lottery_price);

                obj = new
                {
                    totalSale,
                    totalPrice,
                    list = dbData.Select(t => new
                    {
                        date = t.date_bill.Date.ToString("dd/M/yyyy"),
                        time = t.time_bill.ToString(),
                        billNumber = t.lottery_number,
                        billPrice = t.lottery_price
                    })
                };

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCurrentPeriodOfflineDate()
        {
            string offlineDate = _context.Online.Where(t => t.online_status == 1 || t.online_status == 2).OrderByDescending(t => t.date_online).FirstOrDefault().date_offline.ToString("dd/MM/yyyy");
            return offlineDate;
        }
    }
}
