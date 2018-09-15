using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FIU_Chat.Models;

namespace FIU_Chat.Controllers
{
    public class LoginController : Controller
    {
        // 
        // GET: /login/

        public IActionResult Index(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine($"Usr: {loginModel.inputEmail} \nPass: {loginModel.inputPassword} \nCheckbox: {loginModel.rememberMe}");
                return RedirectToAction("LoggedIn");
            }

            return View();
        }

        public IActionResult LoggedIn()
        {
            return View();    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
