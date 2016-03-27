using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ProjectManager.Models.Xero;

namespace ProjectManager.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; }

        public virtual Employee Employee { get; set; }

        public DateTime LastLoginDate { get; set; }
    }
}
