using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.Api.Common;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;
using Xero.Api.Infrastructure.ThirdParty.ServiceStack.Text;
using ProjectManager.Models.Xero;
using ProjectManager.Contracts;
using ProjectManager.Services.XeroService.BalanceServices;

namespace ProjectManager.Services.XeroService   
{
    public class XeroCashflowConnection : IXeroCashflowConnection
    {
        private readonly List<XeroTransaction> _transactions = new List<XeroTransaction>();
        private readonly List<IXeroLoadableTransactions> _xeroTransactionLoaers = new List<IXeroLoadableTransactions>();

        public decimal GetBalance()
        {
            var balance = new Bbtx1569BalanceService().GetBalance();

            return balance;
        }

        public void RegisterTransactionLoader(IXeroLoadableTransactions xeroTransactionLoader)
        {
            if (xeroTransactionLoader != null)
                this._xeroTransactionLoaers.Add(xeroTransactionLoader);
        }

        public IEnumerable<XeroTransaction> GetOrderedTransactions()
        {
            ExecuteLoaders();

            var orderedTransactions = _transactions
                .OrderBy(t => t.Date)
                .ThenBy(t => t.XeroTransactionType)
                .ToList();

            return orderedTransactions;
        }

        private void ExecuteLoaders()
        {
            foreach (var loader in _xeroTransactionLoaers)
            {
                var transactions = loader.GetTransactions();

                foreach (var transaction in transactions)
                    _transactions.Add(transaction);
            }
        }
    }
}
