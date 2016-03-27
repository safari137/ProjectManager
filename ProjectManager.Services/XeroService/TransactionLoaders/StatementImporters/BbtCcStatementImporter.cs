using System;
using Xero.Api.Core;
using ProjectManager.Contracts;

namespace ProjectManager.Services.XeroService.TransactionLoaders.StatementImporters
{
    public class BbtCcStatementImporter : BankStatementImporter
    {
        private static readonly Guid BbtGuid = new Guid("249a3f8a-e2dc-4504-8e73-6783a3d35bf7");

        public BbtCcStatementImporter() : base(BbtGuid, new BbtDueDate(), "BBT Credit Card")
        {
            
        }
    }

    public class BbtDueDate : IDueDate
    {
        private const int ClosingDay = 23;
        private const int DueDay = 12;

        public DateTime GetDueDate(DateTime date)
        {
            var dueDate = date.AddMonths(date.Day <= ClosingDay ? 1 : 2);

            dueDate = dueDate.AddDays(DueDay - dueDate.Day);

            return dueDate;
        }
    }
}
