using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIUChat.Enums;
using FIUChat.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace FIU_Chat.Controllers
{
    public class HomeController : Controller
    {
        private AuthenticateUser authenticateUser = new AuthenticateUser();
        
        public async Task<IActionResult> Index()
        {
            var request = Request;
            var headers = request.Headers;

            StringValues token;
            if (headers.TryGetValue("Authorization", out token))
            {
                var result = await this.authenticateUser.ValidateToken(token);
                if (result.Result == AuthenticateResult.Success)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Account");
                }
            }
               
            return RedirectToAction("Index", "Account");
        }
    }
}