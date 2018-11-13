using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FIUChat.Enums;
using FIUChat.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace FIU_Chat.Controllers
{
    [Produces("application/json")]
    public class ClassesController : Controller
    {
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        public async Task<IActionResult> Index()
        {
            var request = Request;
            var headers = request.Headers;

            StringValues token;
            if (headers.TryGetValue("Authorization", out token))
            {
                var result = this.authenticateUser.ValidateToken(token);
                if (result.Result == AuthenticateResult.Success)
                {
                    var successfulToken = Json(new
                    {
                        displayPage = View("Index"),
                        classes = await this.GetUserClasses(token),
                        success = true
                    });
                    return Ok(successfulToken);
                }
                else
                {
                    return RedirectToAction("Index", "Account");
                }
            }

            return RedirectToAction("Index", "Account");
        }

        private async Task<Dictionary<string, List<Dictionary<string, string>>>> GetUserClasses(string token)
        {
            return await this.authenticateUser.GetUserDictionaryFromToken(token);
        }
    }
}