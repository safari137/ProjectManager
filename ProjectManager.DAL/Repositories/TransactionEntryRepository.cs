using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Accounting;

namespace ProjectManager.DAL.Repositories
{
    public class TransactionEntryRepository : RepositoryBase<TransactionEntry>
    {
        public TransactionEntryRepository(DataContext context) : base(context)
        {
            
        }
    }
}
