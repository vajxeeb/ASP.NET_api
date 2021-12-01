using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Securites
{
    public class UserSeller
    {
        [Key]
        public int usid { get; set; }
        public string device_code { get; set; }
        public string us_pwd { get; set; }
        public int us_status { get; set; }
        public int online_status { get; set; }
        public DateTime date_register { get; set; }
        public int create_by { get; set; }
        public int branch_id { get; set; }
        public int unit_id { get; set; }
        public int ref_code { get; set; }
    }
}
