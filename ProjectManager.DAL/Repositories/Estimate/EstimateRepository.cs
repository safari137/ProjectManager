using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;

namespace ProjectManager.DAL.Repositories.Estimate
{
    public class EstimateRepository : RepositoryBase<Models.Estimator.Estimate>
    {
        public EstimateRepository(DataContext context) : base(context)
        {
            
        }
    }
}
