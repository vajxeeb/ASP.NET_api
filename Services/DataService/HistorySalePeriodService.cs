using Models.DataContext;
using Models.Utility;
using Services.Repository.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataService
{
    public class HistorySalePeriodService : GenericRepository<LotteryNumber>, IHistorySalePeriodService
    {
        private readonly EfDbContext _context;
        public HistorySalePeriodService(EfDbContext context) : base(context)
        {
            _context = context;
        }
        public object Get(string deviceCode, string periodNumber)
        {
            object obj;
            var cancelList = _context.BillCancel.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).ToList();
            var billList = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode).ToList();


            var max_length = _context.DigitLenght.FirstOrDefault().max_lenght;

            var maxSell = _context.DeviceMaxSell.FirstOrDefault(t => t.device_code == deviceCode).max_sell - billList.Where(t => !cancelList.Select(c => c.bill_number).Contains(t.bill_number)).Sum(t => t.bill_price);
            var startPeriod = _context.Online.FirstOrDefault(t => t.period_number == periodNumber).date_online.ToString("dd/MM/yyyy");
            var totalSell = billList.Where(t => !cancelList.Select(c => c.bill_number).Contains(t.bill_number)).Sum(t => t.bill_price).ToString();
            var totalBill = billList.Count().ToString();
            var totalCancel = cancelList.Count().ToString();

            var totalDigitSell = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode)
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
                      //billNumber = g.Select(n => n.bill.bill_number)
                  }).ToList();

            var lastBill = billList.OrderByDescending(t => t.bill_number).FirstOrDefault();
            var billDetailList = from bill in billList.OrderByDescending(t => t.bill_number) //.OrderByDescending(t => t.date_bill).ThenByDescending(t => t.time_bill)
                                 join cancel in cancelList on bill.bill_number equals cancel.bill_number into cancelLeft
                                 from lj_cancel in cancelLeft.DefaultIfEmpty()
                                 select new
                                 {
                                     key = (lj_cancel != null && lj_cancel.cancel_by == 0) ? lj_cancel.cancel_id : bill.bill_id,
                                     billNumber = bill.bill_number,
                                     dateTime = $"{bill.date_bill:dd/MM/yyyy} {bill.time_bill}",
                                     billPrice = bill.bill_price.ToString(),
                                     billStatus = lj_cancel == null ? "ປົກກະຕິ" : (string.IsNullOrEmpty(lj_cancel.cancel_by.ToString()) ? "ປົກກະຕິ" : lj_cancel.cancel_by == 0 ? $"{deviceCode} (ຍົກເລີກ)" : "ແອັດມີນຍົກເລີກ"),
                                     isLast = (lastBill.bill_number == bill.bill_number) && (lj_cancel == null || string.IsNullOrEmpty(lj_cancel.cancel_by.ToString())),
                                     isCancel = lj_cancel != null && (lj_cancel.cancel_by == 0|| lj_cancel.cancel_by == 1),
                                 };


            obj = new
            {
                maxSell,
                startPeriod,
                totalSell,
                totalBill,
                totalCancel,
                totalDigitSell,
                billDetailList
            };
            return obj;
        }
    }
}



