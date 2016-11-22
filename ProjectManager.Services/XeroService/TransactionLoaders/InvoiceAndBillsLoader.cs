using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.Contracts;
using ProjectManager.Models.Xero;
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
                .Find();

            foreach (var invoice in invoices)
            {
                var useExpectedPaymentDate = invoice.Type == InvoiceType.AccountsReceivable && invoice.ExpectedPaymentDate > DateTime.MinValue;
                var newTransaction = new XeroTransaction
                {
                    Contact = invoice.Contact.Name,
                    Date = (useExpectedPaymentDate) ? invoice.ExpectedPaymentDate : invoice.DueDate,
                    Amount = (invoice.Type == InvoiceType.AccountsReceivable) ? invoice.AmountDue : invoice.AmountDue * -1,
                    XeroTransactionType = (invoice.Type == InvoiceType.AccountsReceivable) ? XeroTransactionType.Deposit : XeroTransactionType.Withdrawal
                };

                if (invoice.Type == InvoiceType.AccountsReceivable)
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