using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.Contracts;
using ProjectManager.Models.Xero;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;

namespace ProjectManager.Services.XeroService.TransactionLoaders
{
    public class InvoiceAndBillsLoader : IXeroLoadableTransactions
    {
        private readonly List<XeroTransaction> _transactions = new List<XeroTransaction>();
        private readonly InvoiceCostRetriever _invoiceCostRetriever;


        public InvoiceAndBillsLoader()
        {
            _invoiceCostRetriever = new InvoiceCostRetriever();
        }

        private void BuildInvoicesAndBills()
        {
            var invoices = XeroApiService.AccountingApi
                .Invoices
                .Where("AmountDue > 0")
                .OrderByDescending("DueDate")
                .Find()
                .Where(i => i.DueDate > DateTime.Today.AddMonths(-1));
  

            foreach (var invoice in invoices)
            {
                var newTransaction = new XeroTransaction
                {
                    Contact = invoice.Contact.Name,
                    Date = invoice.DueDate,
                    Amount = invoice.AmountDue,
                    XeroTransactionType = XeroTransactionType.Deposit
                };

                if (invoice.Type == InvoiceType.AccountsPayable)
                {
                    newTransaction.Amount *= -1;
                    newTransaction.XeroTransactionType = XeroTransactionType.Withdrawal;
                }
                else
                {
                    var costTransaction = _invoiceCostRetriever.CreateCostTransaction(invoice);
                    if (costTransaction != null)
                        _transactions.Add(costTransaction);
                }

                _transactions.Add(newTransaction);
            }
        }

        public List<XeroTransaction> GetTransactions()
        {
            this.BuildInvoicesAndBills();

            return _transactions;
        }
    }
}