using JwtAuthApi.IdentityAuth;
using JwtAuthApi.Models;
using JwtAuthApi1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status ="Error", Message ="User Already Exists!" });
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Role = model.Role,
                Loc_EMP = model.Loc_EMP,
                JOB_title = model.JOB_title,
                emp_id = model.emp_id,
                LastSeen = model.LastSeen,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status ="Error", Message =string.Join(",", errors) });
            }
            return Ok(new Response { Status = "Success", Message ="User Created Successfully!" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:secretKey"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message ="User does not exists!" });
            }
            if (string.Compare(model.NewPassword, model.ConfirmNewPassword) !=0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message ="The new password and confirm new password does not match!" });
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = string.Join(",", errors) });
            }
            return Ok(new Response { Status="Success", Message="Password Changed successfully!" });
        }

        [HttpPost]
        [Route("reset-password-token")]
        public async Task<IActionResult> ResetPasswordToken([FromBody] ResetPasswordTokenModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user ==null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message ="User does not exists!" });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //Best practice send token to user email and generate url, the following is for only example
            return Ok(new { token = token });
        }
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user ==null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message ="User does not exists!" });
            }

            if (string.Compare(model.NewPassword, model.ConfirmNewPassword) !=0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message ="The new password and confirm new password does not match!" });
            }

            if (string.IsNullOrEmpty(model.Token))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message ="Invalid Token!" });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = string.Join(",", errors) });
            }
            return Ok(new Response { Status="Success", Message="Password Reseted successfully!" });
        }
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await HelpAddRole(model);
            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }
            return Ok(model);
        }
        [NonAction]
        public async Task<string> HelpAddRole(AddRoleModel addRole)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(addRole.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(addRole.RoleName))
            {
                return "Role or user name is invalid";
            }
            if (await _userManager.IsInRoleAsync(user, addRole.RoleName))
            {
                return "User already assigned to this role";
            }
            var result = await _userManager.AddToRoleAsync(user, addRole.RoleName);
            if (result.Succeeded)
            {
                return String.Empty;
            }
            return "Sonething went wrong";
        }
    }
}