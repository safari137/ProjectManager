using System;
using ProjectManager.Models.Xero;
using Xero.Api.Core.Model;

namespace ProjectManager.Services.XeroService.TransactionLoaders
{
    public class InvoiceCostRetriever
    {
        public XeroTransaction CreateCostTransaction(Invoice invoice)
        {
            if (string.IsNullOrEmpty(invoice.Reference))
                return null;

            var amountString = ParseStringForAmount(invoice.Reference);

            decimal cost;

            var success = decimal.TryParse(amountString, out cost);

            if (!success)
                return null;

            var transaction = new XeroTransaction
            {
                Amount = 0 - cost,
                Contact = invoice.Contact.Name + " - Job Cost",
                XeroTransactionType = XeroTransactionType.Withdrawal,
                Date = GetDueDate(invoice.ExpectedPaymentDate.GetValueOrDefault())
        };

            return transaction;
        }

        private static string ParseStringForAmount(string referenceField)
        {
            var startIndex = referenceField.IndexOf('<');
            var endIndex = referenceField.IndexOf('>');

            var strLength = referenceField.Length;

            if (strLength < endIndex)
                throw new InvalidOperationException("endIndex is greater than the string length!");

            return endIndex < 0 ? null : referenceField.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        private static DateTime GetDueDate(DateTime dueDate)
        {
            return dueDate.AddDays(7);
        }
    }
}