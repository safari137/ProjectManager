using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Models.Xero;

namespace ProjectManager.Contracts
{
    public interface IXeroLoadableTransactions
    {
        List<XeroTransaction> GetTransactions();
    }
}
