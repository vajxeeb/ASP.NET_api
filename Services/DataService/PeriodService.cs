using Models.DataContext;
using Models.Utility;
using Services.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataService
{
    public class PeriodService : GenericRepository<Online>, IPeriodService
    {
        private readonly EfDbContext _context;
        public PeriodService(EfDbContext context) : base(context)
        {
            _context = context;
        }

        public object Get(string deviceCode)
        {
            try
            {
                object obj;
                if (_context.Online.Where(t => t.online_status == 3).Any())
                {
                    var billDetailList = _context.Bill.Where(t => t.device_code == deviceCode)
                        .Join(_context.Online, t => t.period_number, o => o.period_number, (t, o) => new
                        {
                            online = o,
                            bill = t
                        })
                        .OrderByDescending(t => t.online.date_online).ThenByDescending(t => t.online.time_online)
                        .AsEnumerable()
                        .GroupBy(t => new { t.bill.period_number })
                        .Select(g => new
                        {
                            periodNumber = g.Select(n => n.bill.period_number).FirstOrDefault(),
                            date = g.Select(n => n.online.date_online.ToString("dd/M/yyyy")).FirstOrDefault(),
                            totalPrice = g.Sum(n => n.bill.bill_price).ToString()
                        }).ToList();

                    obj = new
                    {
                        billDetailList
                    };

                    return obj;
                }
                else
                {
                    obj = new
                    {
                        billDetailList = new List<object>()
                    };

                    return obj;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public object GetV2(string deviceCode)
        {
            try
            {
                object obj;
                var plList = _context.Bill.Where(t => t.device_code == deviceCode).Select(t => t.period_number).Distinct().ToList();
                var billDetailList = _context.Bill.Where(t => t.device_code == deviceCode).Select(t => t.period_number).Distinct()
                    .Join(_context.Online, b => b, o => o.period_number, (b, o) => new
                    {
                        period_number = b,
                        Online = o
                    })
                    //.Where(t => t.Online.period_number == t.period_number)
                    .Select(t => new
                    {
                        periodNumber = t.period_number,
                        deviceCode = deviceCode,
                        dateOffline = t.Online.date_offline.ToString("dd/MM/yyy")
                    }).ToList();

                obj = new
                {
                    billDetailList
                };
                
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
