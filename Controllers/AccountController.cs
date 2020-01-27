
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityWithoutEF.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityWithoutEF.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        readonly UserManager<ApplicationUser> _userMan;
        readonly SignInManager<ApplicationUser> _signInMan;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userMan, SignInManager<ApplicationUser> signInMan, IConfiguration configuration)
        {
            _userMan = userMan;
            _signInMan = signInMan;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<string> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userMan.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var createdUser = await _userMan.FindByNameAsync(model.Email);

                    System.Console.WriteLine("logged in");
                    return AuthenticateUser(user);
                }
            }

            return null;
        }

        [HttpPost]
        public async Task<string> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInMan.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userMan.FindByNameAsync(model.Email);

                    System.Console.WriteLine("logged in");
                    return AuthenticateUser(user);
                }
            }

            return null;
        }

        private string AuthenticateUser(ApplicationUser user)
        {
            if (User == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var temp = _configuration["Secrets:TokenKey"];
            var key = Encoding.ASCII.GetBytes(temp);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}