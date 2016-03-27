using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models.Xero
{
    public class XeroItem
    {
        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public int XeroItemCodeId { get; set; }

        public decimal Cost { get; set; }

        public decimal SalePrice { get; set; }

        public string ItemNameDescription => ItemName + " - " + Description;
    }
}
