using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Utility
{
    public class BillCancelDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bcd_id { get; set; }
        public Guid cancel_id { get; set; }
        public string bill_number { get; set; }
        public string lottery_number { get; set; }
        public int lottery_price { get; set; }
        public DateTime date_bill_cancel_detail { get; set; }
    }
}
