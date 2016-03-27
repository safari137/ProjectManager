using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Xero;

namespace ProjectManager.DAL.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>
    {
        public EmployeeRepository(DataContext context) : base(context)
        {
        }
    }
}
