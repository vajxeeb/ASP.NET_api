using Models.Utility;
using Services.Repository.Interface;

namespace Services
{
    public interface IMobileVersionService : IGenericRepository<MobileVersion>
    {
        bool IsMobileVersionLatest(string versionName);
        object GetVersionDeviceNumberCode(string versionName, string deviceImei);
    }
}
