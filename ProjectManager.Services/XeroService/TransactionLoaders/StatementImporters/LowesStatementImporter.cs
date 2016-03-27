using System;
using ProjectManager.Contracts;
using Xero.Api.Core;

namespace ProjectManager.Services.XeroService.TransactionLoaders.StatementImporters
{
    public class LowesStatementImporter : BankStatementImporter
    {
        private static readonly Guid BankAcountIdLowes = new Guid("1996ecc3-7a19-48b2-9bdd-95b4aadd6a4e");

        public LowesStatementImporter() : base(BankAcountIdLowes, new LowesDueDate(), "Lowe's Credit Card")
        {
            
        }
    }

    public class LowesDueDate : IDueDate
    {
        private const int DueDay = 20;

        public DateTime GetDueDate(DateTime date)
        {
            var dueDate = date.AddMonths(1);

            return dueDate.AddDays(DueDay - dueDate.Day);
        }
    }
}
