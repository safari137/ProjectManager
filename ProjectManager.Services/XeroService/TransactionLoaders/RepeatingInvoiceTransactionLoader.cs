using System.Collections.Generic;
using ProjectManager.Contracts;
using ProjectManager.Models.Xero;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;

namespace ProjectManager.Services.XeroService.TransactionLoaders
{
    public class RepeatingInvoiceTransactionLoader : IXeroLoadableTransactions
    {
        private readonly List<XeroTransaction> _transactions = new List<XeroTransaction>();
        private readonly int _forecastWeeks, _forecastMonths;
         
        public RepeatingInvoiceTransactionLoader(int forecastWeeks, int forecastMonths)
        {
            this._forecastMonths = forecastMonths;
            this._forecastWeeks = forecastWeeks;
        }

        public List<XeroTransaction> GetTransactions()
        {
            BuildRepeatingBills();

            return _transactions;
        }

        private void BuildRepeatingBills()
        {
            var billTemplates = XeroApiService.AccountingApi
                .RepeatingInvoices
                .Where("Total > 0")
                .And("Status != \"DELETED\"")
                .Find();

            foreach (var billTemplate in billTemplates)
            {
                if (billTemplate.Schedule.Unit == UnitType.Monthly)
                    AddMonthlyBills(billTemplate);
                
                else
                    AddWeeklyBills(billTemplate);
            }
        }

        private void AddWeeklyBills(RepeatingInvoice billTemplate)
        {
            var date = billTemplate.Schedule.NextScheduledDate.GetValueOrDefault();

            for (var i = 0; i < _forecastWeeks; i++)
            {
                _transactions.Add(new XeroTransaction
                {
                    Contact = billTemplate.Contact.Name,
                    Amount = 0 - billTemplate.Total,
                    Date = date,
                    XeroTransactionType = XeroTransactionType.Withdrawal
                });

                date = date.AddDays(7);
            }
        }

        private void AddMonthlyBills(RepeatingInvoice billTemplate)
        {
            var date = billTemplate.Schedule.NextScheduledDate.GetValueOrDefault();

            for (var i = 0; i < _forecastMonths; i++)
            {
                _transactions.Add(new XeroTransaction
                {
                    Contact = billTemplate.Contact.Name,
                    Amount = 0 - billTemplate.Total,
                    Date = date,
                    XeroTransactionType = XeroTransactionType.Withdrawal
                });

                date = date.AddMonths(1);
            }
        }
    }
}