﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models.Xero
{
    public class TrackingItem
    {
        [Required]
        [MaxLength(80)]
        public string Name { get; set; }

        public Guid Id { get; set; }
    }
}
