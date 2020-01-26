
using System.Threading.Tasks;
using IdentityWithoutEF.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWithoutEF.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        readonly UserManager<ApplicationUser> _userMan;
        // readonly SignInManager<ApplicationUser> _signInMan;

        public AccountController(UserManager<ApplicationUser> userMan)
        {
            _userMan = userMan;
            // _signInMan = signInMan;
        }

        [HttpPost]
        public async Task<IdentityResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userMan.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    System.Console.WriteLine("fucking woot!!");
                }

                return result;
            }

            return null;
        }
    }
}