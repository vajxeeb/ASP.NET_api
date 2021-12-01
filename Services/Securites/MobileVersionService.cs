using Models.DataContext;
using Models.Utility;
using Services.Repository.Implementation;
using System;
using System.Linq;
using ViewModels;

namespace Services
{
    public class MobileVersionService : GenericRepository<MobileVersion>, IMobileVersionService
    {
        private readonly EfDbContext _context;
        public MobileVersionService(EfDbContext context) : base(context)
        {
            _context = context;
        }
        public bool IsMobileVersionLatest(string versionName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(versionName)) return false;

                if (_context.MobileVersion.OrderByDescending(t => t.version_date).FirstOrDefault(t => t.version_status == 1).version_name == versionName)
                    return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public object GetVersionDeviceNumberCode(string versionName, string deviceImei)
        {
            bool isVersionLatest = false;
            try
            {
                if (string.IsNullOrWhiteSpace(versionName)) isVersionLatest = false;
                else if (_context.MobileVersion.OrderByDescending(t => t.version_date).FirstOrDefault(t => t.version_status == 1).version_name == versionName) isVersionLatest = true;
                else isVersionLatest = false;

                var obj = _context.Device.Where(t => t.device_imei == deviceImei)
                    .Join(_context.Branch, dev => dev.branch_id, br => br.branch_id, (dev, br) => new
                    {
                        device = dev,
                        branch = br
                    })
                    .Join(_context.Province, br => br.branch.province_id, pr => pr.provice_id, (br, pr) => new
                    {
                        br.device.device_number,
                        br.device.device_code,
                        br.branch.branch_id,
                        branchName = pr.province_name + " ເລກ " + br.branch.branch_code,
                        isVersionLatest,
                    }).FirstOrDefault();
                if (obj == null)
                {
                    obj = new
                    {
                        device_number = 0,
                        device_code = "",
                        branch_id = 0,
                        branchName = "",
                        isVersionLatest,
                    };
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
