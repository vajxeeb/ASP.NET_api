using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class Device
    {
        [Key]
        public int device_number { get; set; }
        public int device_type_id { get; set; }
        public int brand_id { get; set; }
        public string device_code { get; set; }
        public string device_ref { get; set; }
        public string device_imei { get; set; }
        public int status_device { get; set; }
        public int branch_id { get; set; }
        public int unit_id { get; set; }
        public int register_by { get; set; }
        public DateTime device_date { get; set; }
        public int code_type { get; set; }

    }
}