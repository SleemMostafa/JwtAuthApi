using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthApi1.Models
{
    public class information
    {
        [Key]
        public int id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Loc_emp { get; set; }
        public string Job_title { get; set; }
        public int emp_id { get; set; }
        public DateTime LastSeen { get; set; }

    }
}
