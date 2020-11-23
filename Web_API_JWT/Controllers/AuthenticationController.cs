using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Web_API_JWT.Model;

namespace Web_API_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
           var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse() { Status = "Error", Message = "User Already Exists" });
            }
            else
            {
                var user = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName=model.UserName,
                    SecurityStamp=Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created, new APIResponse() { Status = "Success", Message = "User Created Successfully" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse() { Status = "Error", Message = "User Creation Failed" });
                }
            }
        }


        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRole = await _userManager.GetRolesAsync(user);

                var claimns = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in userRole)
                {
                    claimns.Add(new Claim(ClaimTypes.Role, role));
                }

                var IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:IssuerSigningKey"]));

                var TokenDescriptor = new SecurityTokenDescriptor
                {                    
                    Subject= new ClaimsIdentity(claimns),
                    Expires = DateTime.Now.AddHours(+3),
                    SigningCredentials = new SigningCredentials(IssuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var token = new JwtSecurityTokenHandler().CreateToken(TokenDescriptor);
                var retToken = new JwtSecurityTokenHandler().WriteToken(token);

                return StatusCode(StatusCodes.Status200OK, new { token = retToken });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new APIResponse() { Status = "Failed", Message = "User Not Exists" });
            }
        }
    }
}