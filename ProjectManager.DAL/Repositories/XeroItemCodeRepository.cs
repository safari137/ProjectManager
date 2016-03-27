using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Xero;

namespace ProjectManager.DAL.Repositories
{
    public class XeroItemCodeRepository : RepositoryBase<XeroItemCode>
    {
        public XeroItemCodeRepository(DataContext context) : base(context)
        {
            
        }
    }
}
