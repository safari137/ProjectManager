using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models.Xero
{
    public class ItemGroup
    {
        [Key]
        public int ItemGroupId { get; set; }

        public string ItemGroupName { get; set; }

        public string XeroGroupItemCode { get; set; }

        public virtual ICollection<XeroItemCode> XeroItemCodes { get; set; }
    }
}
