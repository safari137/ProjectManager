using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models;

namespace ProjectManager.DAL.Repositories
{
    public class JobRepository : RepositoryBase<Job>
    {
        public JobRepository(DataContext context) : base(context)
        {
            
        }
    }
}
