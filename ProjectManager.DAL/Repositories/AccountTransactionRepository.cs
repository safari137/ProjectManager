using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Accounting;

namespace ProjectManager.DAL.Repositories
{
    public class AccountTransactionRepository : RepositoryBase<AccountTransaction>
    {
        public AccountTransactionRepository(DataContext context) : base(context)
        {
        }

    }
}
