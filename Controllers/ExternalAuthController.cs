using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductAPIIdentity.Models;
using ProductAPIIdentity.services;
using System.Security.Claims;

namespace ProductAPIIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInMaager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public ExternalAuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,TokenService tokenService)
        {
            _signInMaager = signInManager;
            _userManager = userManager;
            _tokenService= tokenService;
            
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "ExternalAuth");
            var properties = _signInMaager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            //google login Information
            var info = await _signInMaager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("Error Loading Login Information");
            }

            //check whether account liked with identity user
            var result = await _signInMaager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true

                );
            //existing google User
            if (result.Succeeded)
            {
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingUser == null)
                {
                    return BadRequest("Google login Exists but userwas not found!!");
                }
                //generate toke for the user
                var existingUserToken = await _tokenService.CreateTokenAsync(existingUser);
                return Ok(new
                {
                    message = "Google Login Success",
                    accessToken = existingUserToken,
                    email = existingUser.Email,
                    name = existingUser.UserName

                });
            }

            //get google user Info
            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = info.Principal.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email not received");
            }


            //Find the User by google
            var user = await _userManager.FindByNameAsync(name);
            //Created user if it doesn't exists
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest(createResult.Errors);
                }
            }

            //Lik google login to identity
            var loginResult = await _userManager.AddLoginAsync(user, info);
            if (!loginResult.Succeeded)
            {
                return BadRequest(loginResult.Errors);
            }
            //Generate token
            var newUserToken = await _tokenService.CreateTokenAsync(user);

            return Ok(new
            {
                message = "google login Success",
                accessToken = newUserToken,
                email = user.Email,
                name = user.UserName
            });
        }

    }
}
