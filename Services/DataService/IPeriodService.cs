using Models.Utility;
using Services.Repository.Interface;

namespace Services.DataService
{
    public interface IPeriodService : IGenericRepository<Online>
    {
        object Get(string deviceCode);

        object GetV2(string deviceCode);
    }
}
