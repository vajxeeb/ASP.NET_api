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
    public class CancelBillService : GenericRepository<BillCancel>, ICancelBillService
    {
        private readonly EfDbContext _context;
        public CancelBillService(EfDbContext context) : base(context)
        {
            _context = context;
        }

        public object GetCancelBill(string deviceCode)
        {
            List<BillDetail> billDetailList = new List<BillDetail>();

            var periodNumber = _context.Online.Where(t => t.online_status == 1).OrderByDescending(t => t.date_online)
                                        .ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();

            var billData = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode)
                                        //.OrderByDescending(t => t.date_bill).ThenByDescending(t => t.time_bill).FirstOrDefault();
                                        .OrderByDescending(t => t.bill_number).FirstOrDefault();

            var cancelData = _context.BillCancel.FirstOrDefault(t => t.bill_number == billData.bill_number && t.period_number == periodNumber && t.device_code == deviceCode);


            if (billData != null && cancelData == null)
                billDetailList = _context.BillDetail.Where(t => t.bill_id == billData.bill_id).ToList();

            else if (billData != null && cancelData != null)
            {
                List<BillCancelDetail> cancelDetailList = new List<BillCancelDetail>();
                cancelDetailList = _context.BillCancelDetail.Where(t => t.cancel_id == cancelData.cancel_id).ToList();
                foreach (var item in cancelDetailList)
                {
                    BillDetail billDetail = new BillDetail
                    {
                        lottery_number = item.lottery_number,
                        lottery_price = item.lottery_price
                    };
                    billDetailList.Add(billDetail);
                }
            }

            object obj = new
            {
                deviceCode,
                hasCancel = cancelData != null,
                billId = billData != null ? billData.bill_id : Guid.NewGuid(),
                billNumber = billData != null ? billData.bill_number : "",
                billTotal = billData != null ? billData.bill_price : 0,
                billDetailList = billDetailList
            };

            return obj;
        }

        public void InsertCancelBill(string deviceCode, int deviceNumber, string billId, string reasonCancel)
        {
            try
            {
                var periodNumber = _context.Online.Where(t => t.online_status == 1).OrderByDescending(t => t.date_online).ThenBy(t => t.time_online).Select(t => t.period_number).FirstOrDefault();

                //var billData = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode && t.bill_number == billId)
                //                            .OrderByDescending(t => t.date_bill.Date).ThenByDescending(t => t.time_bill).FirstOrDefault();

                var billData = _context.Bill.Where(t => t.period_number == periodNumber && t.device_code == deviceCode && t.bill_id == Guid.Parse(billId)).FirstOrDefault();

                if (billData != null)
                {
                    var billDetailList = _context.BillDetail.Where(t => t.bill_id == billData.bill_id && t.date_bill_detail.Date == billData.date_bill.Date).ToList();
                    var cancelBill = new BillCancel
                    {
                        cancel_id = Guid.NewGuid(),
                        bill_number = billData.bill_number,
                        period_number = billData.period_number,
                        device_code = billData.device_code,
                        bill_price = billData.bill_price,
                        reason_cancel = reasonCancel == "1" ? "can\'t print" : (reasonCancel == "2" ? "Buyer cancel" : "Add wrong number"),
                        date_cancel = DateTime.Now.Date,
                        time_cancel = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                    };
                    _context.Add(cancelBill);

                    var list = new List<BillCancelDetail>();

                    foreach (var item in billDetailList)
                    {
                        var detail = new BillCancelDetail
                        {
                            cancel_id = cancelBill.cancel_id,
                            bill_number = item.bill_number,
                            lottery_number = item.lottery_number,
                            lottery_price = item.lottery_price,
                            date_bill_cancel_detail = DateTime.Now.Date,
                        };
                        list.Add(detail);
                    }

                    _context.AddRange(list);

                    //_context.Entry(billData).State = EntityState.Deleted;
                    //_context.Bill.Remove(billData);
                    _context.BillDetail.RemoveRange(billDetailList);

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}



