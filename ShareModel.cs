using System;
using System.Collections.Generic;
using System.Text;

namespace check_share_price
{
    public class ShareModel
    {
        public string Symbol { get; set; }
        public DateTime DateTime { get; }
        public decimal Price { get; set; }

        public ShareModel()
        {
            DateTime = DateTime.UtcNow;
        }
        
    }
}
