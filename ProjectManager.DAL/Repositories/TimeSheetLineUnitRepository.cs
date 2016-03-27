using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.DAL.Data;
using ProjectManager.Models.Xero;

namespace ProjectManager.DAL.Repositories
{
    public class TimesheetLineUnitRepository : RepositoryBase<TimeSheetLineUnit>
    {
        public TimesheetLineUnitRepository(DataContext context) : base(context)
        {
        }
    }
}
