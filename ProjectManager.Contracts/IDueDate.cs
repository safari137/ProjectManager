using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Contracts
{
    public interface IDueDate
    {
        DateTime GetDueDate(DateTime date);
    }
}
