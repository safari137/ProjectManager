using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.WebUI.Models
{
    public class CreateTimeSheetViewModel
    {
        public Guid XeroEmployeeId { get; set; }

        public DateTime EndDate { get; set; }
    }
}