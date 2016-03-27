using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.Contracts;
using ProjectManager.Models.Xero;
using Xero.Api.Core.Model.Reports;

namespace ProjectManager.Services.XeroService.TransactionLoaders.StatementImporters
{
    public class BankStatementImporter : IXeroLoadableTransactions
    {
        private readonly string _accountName;
        private readonly Dictionary<DateTime, decimal> _billDictionary = new Dictionary<DateTime, decimal>();
        private decimal _payments;
        private readonly IDueDate _dueDateCalculator;
        private Guid _bankId = Guid.Empty;

        public BankStatementImporter(string accountName, IDueDate dueDateCalculator)
        {
            this._accountName = accountName;
            this._dueDateCalculator = dueDateCalculator;
        }

        public BankStatementImporter(Guid bankId, IDueDate dueDateCalculator, string accountName)
        {
            if (bankId == Guid.Empty)
                throw new InvalidOperationException("Guid is invalid.");

            this._bankId = bankId;
            this._dueDateCalculator = dueDateCalculator;
            this._accountName = accountName;
        }

        public void Start()
        {
            this.LoadStatement();
            this.ApplyPayments();
        }


        private void LoadStatement()
        {
            if (_bankId == Guid.Empty)
                _bankId = GetBankId(_accountName).GetValueOrDefault();
             
            var today = DateTime.Today;

            var startdate = today.AddMonths(-2);
            startdate = startdate.AddDays(1 - startdate.Day);

            var statement = XeroApiService.AccountingApi
                .Reports
                .BankStatement(_bankId, startdate, today);

            ProcessStatement(statement);
        }

        private void ProcessStatement(Report report)
        {
            foreach (var row in report.Rows)
            {
                if (row.Rows == null)
                    continue;

                foreach (var subRow in row.Rows)
                {
                    ProcessRow(subRow);
                }
            }
        }

        private void ProcessRow(ReportRow row)
        {
            var isUnimportantRow = (row.Cells.Count < 7);

            if (isUnimportantRow)
                return;
           
            DateTime date;
            if (!DateTime.TryParse(row.Cells.ElementAt(0).Value, out date))
                return;

            decimal amount;

            var amountElement = (row.Cells.ElementAt(1).Value == "Opening Balance") ? 6 : 5;

            if (!decimal.TryParse(row.Cells.ElementAt(amountElement).Value, out amount))
                return;

            var isDeposit = (amount > 0);

            if (isDeposit)
            {
                _payments += amount;
                return;
            }

            var dueDate = _dueDateCalculator.GetDueDate(date);

            if (_billDictionary.ContainsKey(dueDate))
                _billDictionary[dueDate] += amount;
            else
                _billDictionary.Add(dueDate, amount);
        
        }

        private void ApplyPayments()
        {
            for (var i = 0; i < _billDictionary.Count; i++)
            {
                var key = _billDictionary.ElementAt(i).Key;

                if (_billDictionary[key] <= (_payments * -1))
                {
                    _billDictionary[key] += _payments;
                    _payments = 0;
                    break;
                }
                else
                {
                    _payments += _billDictionary[key];
                    _billDictionary[key] = 0;
                }

            }
        }

        private Guid? GetBankId(string accountName)
        {
            var whereString = "Name == \"" + accountName + "\"";
            var bankAccount = XeroApiService.AccountingApi
                .Accounts
                .Where(whereString)
                .Find()
                .SingleOrDefault();

            return bankAccount?.Id;
        }

        public List<XeroTransaction> GetTransactions()
        {
            this.Start();

            return ConvertDictionaryToXeroTransactionList();
        }

        private List<XeroTransaction> ConvertDictionaryToXeroTransactionList()
        {
            var transactions = new List<XeroTransaction>();

            foreach (var definition in _billDictionary)
            {
                if (definition.Value == 0)
                    continue;

                var transaction = new XeroTransaction
                {
                    Date = definition.Key,
                    Amount = definition.Value,
                    XeroTransactionType = XeroTransactionType.Withdrawal,
                    Contact = _accountName
                    };

                transactions.Add(transaction);
            }
            return transactions;
        }
    }
}
