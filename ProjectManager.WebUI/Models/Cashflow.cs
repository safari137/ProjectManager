using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManager.Models.Xero;

namespace ProjectManger.WebUI.Models
{
    public class Cashflow
    {
        public decimal Balance { get; set; }

        public IEnumerable<XeroTransaction> XeroTransactions { get; set; }  
    }
}