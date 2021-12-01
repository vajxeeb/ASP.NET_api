using System.Collections.Generic;

namespace ViewModels
{
    public class BillViewModel
    {
        public string deviceCode { get; set; }
        public string billNumber { get; set; }
        public string periodNumber { get; set; }

        public List<SaleViewModel> SaleList { get; set; }
    }
}
