using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductAPIIdentity.DTOs;
using ProductAPIIdentity.Models;
using ProductAPIIdentity.services;

namespace ProductAPIIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        public AuthController(UserManager<ApplicationUser> userManager,TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            var existingUser = await _userManager.FindByNameAsync(register.UserName);
            if (existingUser != null)
            {
                return BadRequest("UserName exists!!");

            }
            var user = new ApplicationUser
            {
                UserName = register.UserName,
                Email = register.Email,
                FullName = register.FullName,
            };
            var result= await _userManager.CreateAsync(user,register.Password);
            //await _userManager.AddToRoleAsync(user, "User");
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "Registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            //check for user existance
            var user= await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return Unauthorized("Register First or Invalid credentials!!");
            }
            var validPassword= await _userManager.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
            {
               return Unauthorized("Register First or Invalid credentials!!");
            }
            var token =  await _tokenService.CreateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok( new AuthResponseDto
            {
                AccessToken = token,
                UserName= model.UserName,
                Roles=roles.ToList(),
            });
        }
    }
}
