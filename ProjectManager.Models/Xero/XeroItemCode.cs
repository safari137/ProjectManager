using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models.Xero
{
    public class XeroItemCode
    {
        public int XeroItemCodeId { get; set; }

        public string ItemCode { get; set; }

        public decimal Quantity { get; set; }

        public int ItemGroupId { get; set; }

        public virtual ItemGroup ItemGroup { get; set; }
    }
}
