using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.Contracts;
using ProjectManager.Models.Xero;
using Xero.Api.Core;
using Xero.Api.Core.Model.Reports;

namespace ProjectManager.Services.XeroService.TransactionLoaders
{
    public class PayrollBillTrasactionLoader : IXeroLoadableTransactions
    {
        private readonly Dictionary<DateTime, decimal> _billDictionary = new Dictionary<DateTime, decimal>(); 
        private readonly List<XeroTransaction> _transactions = new List<XeroTransaction>();
        private readonly string _contactName;
        private Guid _contactId = Guid.Empty;

        public PayrollBillTrasactionLoader(string contactName)
        {
            _contactName = contactName;
        }

        public PayrollBillTrasactionLoader(Guid contactId, string contactName)
        {
            this._contactId = contactId;
            this._contactName = contactName;
        }

        private void ProcessAllCells()
        {
            if (_contactId == Guid.Empty)
                _contactId = GetContactId().GetValueOrDefault();

            var beginningOfYearDate = DateTime.Today.AddMonths(-13);

            var report = XeroApiService.AccountingApi
                .Reports
                .AgedPayables(_contactId, null, beginningOfYearDate, null);

            foreach (var row in report.Rows.Where(row => row.Rows != null))
            {
                for (var i = 1; i < row.Rows.Count-1; i++)
                {
                    ProcessCell(row.Rows[i]);
                }
            }
        }

        private void ProcessCell(ReportRow row)
        { 
            if (row.Cells.Count < 8)
                return;

            DateTime dueDate;
            if (!DateTime.TryParse(row.Cells[2].Value, out dueDate))
                return;

            if (dueDate < DateTime.Today.AddDays(-90))
                return;

            decimal amount;
            decimal.TryParse(row.Cells[7].Value, out amount);

            if (amount <= 0)
                return;

            BuildDictionary(dueDate, amount);
        }

        private void BuildDictionary(DateTime dueDate, decimal amount)
        {
            if (_billDictionary.ContainsKey(dueDate))
                _billDictionary[dueDate] += amount;
            else
                _billDictionary.Add(dueDate, amount);
        }

        private void ConvertDictionaryToTransactionList()
        {
            foreach (var definition in _billDictionary)
            {
                var transaction = new XeroTransaction()
                {
                    Contact = _contactName,
                    Date = definition.Key,
                    Amount = 0 - definition.Value,
                    XeroTransactionType = XeroTransactionType.Withdrawal
                };

                _transactions.Add(transaction);
            }
        }

        private Guid? GetContactId()
        {
            var whereRequest = "Name == \"" + _contactName + "\"";

            var contact = XeroApiService.AccountingApi
                .Contacts
                .Where(whereRequest)
                .Find()
                .FirstOrDefault();

            return contact?.Id;
        }

        public List<XeroTransaction> GetTransactions()
        {
            ProcessAllCells();
            ConvertDictionaryToTransactionList();

            return _transactions;
        }
    }
}
