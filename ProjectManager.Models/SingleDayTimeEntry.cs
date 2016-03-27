using System;

namespace ProjectManager.Models
{
    public class SingleDayTimeEntry
    {
        public Guid CustomerId { get; set; }

        public string Notes { get; set; }

        public decimal Hours { get; set; }
    }
}