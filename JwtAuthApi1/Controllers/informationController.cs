
using JwtAuthApi1.Context;
using JwtAuthApi1.DbContext;
using JwtAuthApi1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JwtAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class informationController : ControllerBase
    {
        private readonly ApplicationDbContext _CRUDContext;

        public informationController(ApplicationDbContext ApplicationDbContext)
        {
            _CRUDContext = ApplicationDbContext;
        }
        // GET: api/<LocationController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IEnumerable<information> Get()
        {
            return _CRUDContext.information;
        }
    }
}
