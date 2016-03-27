using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models.Xero    
{
    public class XeroTransaction
    {
        public string Contact { get; set; }

        public XeroTransactionType XeroTransactionType { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? Date { get; set; }
    }

    public enum XeroTransactionType
    {
        Deposit,
        Withdrawal
    };
}
