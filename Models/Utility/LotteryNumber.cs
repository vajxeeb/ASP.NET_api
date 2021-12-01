using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class LotteryNumber
    {
        [Key]
        public int ln_id { get; set; }
        public string lottery_number { get; set; }
        public int ln_status { get; set; }
        public int max_sell { get; set; }
        public TimeSpan time_register { get; set; }
        public DateTime date_register { get; set; }
    }
}
