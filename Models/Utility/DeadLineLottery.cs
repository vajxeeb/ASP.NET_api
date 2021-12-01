using System.ComponentModel.DataAnnotations;

namespace Models.Utility
{
    public class DeadLineLottery
    {
        [Key]
        public int dll_id { get; set; }
        public int max_date { get; set; }
    }
}
