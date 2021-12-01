using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class MobileVersion
    {
        [Key]
        public string version_name { get; set; }
        public int version_status { get; set; }
        public DateTime? version_date { get; set; }
    }
}
