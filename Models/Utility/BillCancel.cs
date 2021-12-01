using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class BillCancel
    {
        [Key]
        public Guid cancel_id { get; set; }
        public string bill_number { get; set; }
        public string period_number { get; set; }
        public string device_code { get; set; }

        public int bill_price { get; set; }
        public string reason_cancel { get; set; }

        public int cancel_by { get; set; }
        public DateTime date_cancel { get; set; }
        public TimeSpan time_cancel { get; set; }
        public int ref_code { get; set; }
    }
}
