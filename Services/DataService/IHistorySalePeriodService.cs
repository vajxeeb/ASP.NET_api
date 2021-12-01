using Models.Utility;
using Services.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using ViewModels;

namespace Services.DataService
{
    public interface IHistorySalePeriodService : IGenericRepository<LotteryNumber>
    {
        object Get(string deviceCode, string periodNumber);
    }
}
