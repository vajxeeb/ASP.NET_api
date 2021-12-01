using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Utility
{
    public class BillDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bd_id { get; set; }
        public Guid bill_id { get; set; }
        public string bill_number { get; set; }
        public string lottery_number { get; set; }
        public int lottery_price { get; set; }

        //[System.Data.Entity.Core.Objects.DataClasses.EdmFunction("Edm", "TruncateTime")]
        public DateTime date_bill_detail { get; set; }
    }
}
