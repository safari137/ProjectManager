using System;
using Xero.Api.Core;

namespace ProjectManager.Services.XeroService.BalanceServices
{
    public abstract class XeroBalanceBase
    {
        private decimal _balance;

        protected XeroBalanceBase()
        {
        }

        public virtual decimal GetBalance()
        {
            return _balance;
        }

        public virtual void Initialize(string accountName)
        {
            const int futureDay = 14;

            var report = XeroApiService.AccountingApi
                .Reports
                .BalanceSheet(DateTime.Today.AddDays(futureDay));

            foreach (var row in report.Rows)
            {
                if (row.Rows == null)
                    continue;

                foreach (var subRow in row.Rows)
                {
                    var previousCell = "";
                    foreach (var cell in subRow.Cells)
                    {
                        if (previousCell == accountName)
                        {
                            decimal balance;
                            decimal.TryParse(cell.Value, out balance);
                            _balance = balance;
                            return;
                        }
                        previousCell = cell.Value;
                    }
                }
            }
        }
    }
}
