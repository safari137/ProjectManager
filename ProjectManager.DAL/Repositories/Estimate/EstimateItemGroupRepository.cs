using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Estimator;

namespace ProjectManager.DAL.Repositories.Estimate
{
    public class EstimateItemGroupRepository : RepositoryBase<EstimateItemGroup>
    {
        public EstimateItemGroupRepository(DataContext context) : base(context)
        {
            
        }
    }
}
