using Models.Utility;
using Services.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IOnlineService : IGenericRepository<Online>
    {
        bool IsSeverOnline();

        object Get(string deviceCode, string drawNumber);

        object GetBillList(string deviceCode, string drawNumber);
        object GetBillDetailList(string billId);

        object GetCancelBillList(string deviceCode, string drawNumber);
        object GetCancelBillDetailList(string billId);

        object GeBillDetailListByDigit(string deviceCode, string drawNumber, string digit);

        string GetCurrentPeriodOfflineDate();
    }
}
