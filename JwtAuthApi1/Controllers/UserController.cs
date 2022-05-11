using JwtAuthApi.IdentityAuth;
using JwtAuthApi.Models;
using JwtAuthApi1.DbContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _CRUDContext;

        public UserController(ApplicationDbContext ApplicationDbContext)
        {
            _CRUDContext = ApplicationDbContext;
        }

        [HttpGet]
        public IEnumerable<ApplicationUser> Get()
        {
            var x = _CRUDContext.user;
            return x;
        }
    }
}