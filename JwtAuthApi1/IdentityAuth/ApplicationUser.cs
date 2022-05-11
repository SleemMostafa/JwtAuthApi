using Microsoft.AspNetCore.Identity;
using System;

namespace JwtAuthApi.IdentityAuth
{
    public class ApplicationUser : IdentityUser
    {

        public string Role { get; internal set; }
        public string Loc_EMP { get; internal set; }
        public string JOB_title { get; internal set; }
        public int emp_id { get; internal set; }
        public DateTime LastSeen { get; internal set; }
    }
}
