using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class Quota
    {
        [Key]
        public int digit_lenght { get; set; }
        public long max_values { get; set; }
        public int price_per_number { get; set; }
    }
}

