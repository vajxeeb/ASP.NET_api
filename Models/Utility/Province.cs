using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Utility
{
    public class Province
    {
        [Key]
        public int provice_id { get; set; }
        public string province_name { get; set; }
    }
}
