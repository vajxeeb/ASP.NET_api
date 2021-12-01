using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class Branch
    {
        [Key]
        public int branch_id { get; set; }
        public string branch_code { get; set; }
        public int province_id { get; set; }
        public int create_by { get; set; }
        public DateTime create_date { get; set; }
    }
}

