using Microsoft.EntityFrameworkCore;
using Models.DataContext;
using Models.Utility;
using Services.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace Services.DataService
{
    public class NumberStatusService : GenericRepository<LotteryNumber>, INumberStatusService
    {
        private readonly EfDbContext _context;
        public NumberStatusService(EfDbContext context) : base(context)
        {
            _context = context;
        }

        public Tuple<int, object> GetNumberList()
        {
            var maxlength = _context.DigitLenght.FirstOrDefault().max_lenght;
            var periodNumber = _context.Online.Where(t => t.online_status == 1 || t.online_status == 2).OrderByDescending(t => t.date_online).ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();

            var lotteryNumberList = _context.LotteryNumber.Where(t => t.ln_status == 1).OrderBy(t => t.lottery_number.Length).ToList();
            var billDetailList = _context.BillDetail
                .Join(_context.Bill.Where(b => b.period_number == periodNumber), bd => bd.bill_id, t => t.bill_id, (bd, t) => new
                {
                    bill = t,
                    billDetails = bd
                })
                .Where(t => t.billDetails.date_bill_detail.Date == t.bill.date_bill.Date && t.billDetails.bill_id == t.bill.bill_id)
                .OrderBy(t => t.billDetails.lottery_number.Length)
                .Select(t => new
                {
                    t.billDetails.bill_number,
                    t.billDetails.lottery_number,
                    t.billDetails.lottery_price,
                    t.billDetails.date_bill_detail
                })
                .ToList();

            List<SaleViewModel> viewModelList = new List<SaleViewModel>();

            int highestCount = 0;

            var lotCount = lotteryNumberList.GroupBy(t => t.lottery_number.Length).Select(g => new { count = g.Select(n => n.lottery_number.Length).Count() }).ToList();

            for (int i = 0; i < lotCount.Count(); i++)
            {
                if (i == 0)
                    highestCount = lotCount[i].count;
                else
                    highestCount = lotCount[i].count > highestCount ? lotCount[i].count : highestCount;
            }

            foreach (var lot in lotteryNumberList)
            {
                var bdList = billDetailList.Where(t => t.lottery_number == lot.lottery_number).ToList();
                SaleViewModel model = new SaleViewModel
                {
                    lotteryNumber = lot.lottery_number,
                    lotteryPrice = bdList.Count() > 0 ? (lot.max_sell - bdList.Sum(t => t.lottery_price)) : lot.max_sell,
                };

                viewModelList.Add(model);
            }
            var data = viewModelList.OrderBy(t => t.lotteryNumber.Length).GroupBy(t => t.lotteryNumber.Length)
                 .Select(g => new
                 {
                     key = g.Select(n => n.lotteryNumber.Length).FirstOrDefault(),
                     highestCount,
                     list = g.Select(n => n).ToList()
                 }); ;

            return new Tuple<int, object>(lotteryNumberList.GroupBy(t => t.lottery_number.Length).Count(), data);
        }
    }
}



