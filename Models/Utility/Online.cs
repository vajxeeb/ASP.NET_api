using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class Online
    {
        [Key]
        public int online_id { get; set; }
        public string period_number { get; set; }
        public int online_status { get; set; }
        public DateTime date_online { get; set; }
        public TimeSpan time_online { get; set; }
        public DateTime date_offline { get; set; }
        public TimeSpan time_offline { get; set; }
        public int create_by { get; set; }
    }
}
