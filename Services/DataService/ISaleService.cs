using Models.Utility;
using Services.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using ViewModels;

namespace Services.DataService
{
    public interface ISaleService : IGenericRepository<Bill>
    {
        [Obsolete]
        string GetCurrentPeriodNumber();
        [Obsolete]
        void CheckNumberPrice(string lotteryNumber, int lotteryPrice);
        [Obsolete]
        List<SaleViewModel> GetSellSetNumber(string lotteryNumber, int lotteryPrice);
        [Obsolete]
        Tuple<int, string> CheckOverMaxSellOrBlancePrice(int lotteryNumberLength, int lotteryPrice, int totalPrice, string deviceCode, string periodNumber);
        [Obsolete]
        string InsertSale(string deviceCode, int deviceNumber, string periodNumber, List<SaleViewModel> saleViewModelList);



        object GetConfigData();
        bool IsOverMaxSell(string deviceCode);
        List<SaleViewModel> GetSellSetNumberV2(string lotteryNumber, int lotteryPrice);
        Tuple<int, string, object> InsertSaleV2(string deviceCode, int deviceNumber, string periodNumber, List<SaleViewModel> saleViewModelList);
        List<SaleSetNumber> GetSellSetNumberList();
    }
}
