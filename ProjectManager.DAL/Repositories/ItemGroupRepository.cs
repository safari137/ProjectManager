using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Xero;

namespace ProjectManager.DAL.Repositories
{
    public class ItemGroupRepository : RepositoryBase<ItemGroup>
    {
        public ItemGroupRepository(DataContext context) : base(context)
        {
            
        }
    }
}
