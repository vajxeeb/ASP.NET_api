using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class DigitRandom
    {
        [Key]
        public int digit_len { get; set; }
        public string digit_name { get; set; }
        public int text_box { get; set; }
        public int ran_status { get; set; }
    }
}

