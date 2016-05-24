using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Models.Xero;

namespace ProjectManager.Models
{
    public class Memo
    {
        public int MemoId { get; set; }

        public DateTime DateTime { get; set; }

        public string Notes { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
