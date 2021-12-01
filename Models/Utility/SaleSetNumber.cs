using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class SaleSetNumber
    {
        [Key]
        public string lottery_number { get; set; }
        public string lottery_name { get; set; }
        public int lottery_digit { get; set; }
        public DateTime date_register { get; set; }
        public int regis_by { get; set; }
    }
}

