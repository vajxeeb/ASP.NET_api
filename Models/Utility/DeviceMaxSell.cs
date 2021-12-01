using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class DeviceMaxSell
    {
        [Key]
        public int mds_id { get; set; }
        public string device_code { get; set; }
        public int max_sell { get; set; }
        public int mds_status { get; set; }
        public DateTime date_register { get; set; }


    }
}

