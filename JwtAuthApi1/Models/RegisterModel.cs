using System;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }  
        [EmailAddress]
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }
        public string Role { get; set; }
        public string Loc_EMP { get; set; }
        public string JOB_title { get; set; }
        public int emp_id { get; set; }
        public DateTime LastSeen { get; set; }

    }
}
