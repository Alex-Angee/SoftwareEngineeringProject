using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FIU_Chat.Models;
using FIUChat.DatabaseAccessObject;
using FIUChat.Identity;
using FIUChat.DatabaseAccessObject.CommandObjects;
using System.Linq.Expressions;

namespace FIU_Chat.Controllers
{
    public class AccountController : Controller
    {
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        // GET: /login/
        public async Task<IActionResult> Index(LoginModel loginModel)
        {
            Debug.WriteLine(loginModel.inputEmail);
            if (ModelState.IsValid)
            {
                var mapLoginModelToUser = new MapLoginModelToUser();
                var user = await mapLoginModelToUser.MapObject(loginModel);

                if(user == null)
                {
                    return View();
                }
                else
                {
                    var result = await this.authenticateUser.Authenticate(user);

                    if(result.Result == AuthenticateResult.Success)
                    {
                        // SUCCESSFUL LOGIN
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // 
                        return View();
                    }
                }
            }

            return View();
        }

        public IActionResult LoggedIn()
        {
            return View();    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public class MapLoginModelToUser
    {
        private ServerToStorageFacade serverToStorageFacade;

        public MapLoginModelToUser()
        {
            serverToStorageFacade = new ServerToStorageFacade();
        }


        public async Task<User> MapObject(LoginModel loginModel)
        {
            Expression<Func<User, bool>> expression = x => x.Email == loginModel.inputEmail;

            var user = await this.serverToStorageFacade.ReadObjectByExpression(new User(Guid.NewGuid()), expression);

            if(user == default(Command))
            {
                return null;
            }

            return new User(user.ID)
            {
                Email = loginModel.inputEmail,
                Password = loginModel.inputPassword,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserEntitlement = user.UserEntitlement
            };
        }
    }
}
