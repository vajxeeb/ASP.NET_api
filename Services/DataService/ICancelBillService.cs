using Models.Utility;
using Services.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using ViewModels;

namespace Services.DataService
{
    public interface ICancelBillService : IGenericRepository<BillCancel>
    {
        object GetCancelBill(string deviceCode);

        void InsertCancelBill(string deviceCode, int deviceNumber, string billId, string reasonCancel);
    }
}
