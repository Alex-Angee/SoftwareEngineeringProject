<<<<<<< Updated upstream
<<<<<<< Updated upstream
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FIU_Chat.Models;
using FIUChat.DatabaseAccessObject;
using FIUChat.Identity;
using FIUChat.DatabaseAccessObject.CommandObjects;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using FIUChat.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FIU_Chat.Controllers
{
    public class AccountController : Controller
    {
        private const string SECRET_KEY = "FIUCHATSECRETKEY";
        public static readonly SymmetricSecurityKey SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        public IActionResult Index()
        {
            return View();
        }

        // Post: /login/
        [HttpPost]
        [Route("/[controller]/Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            Debug.WriteLine(loginModel.inputEmail);
            if (ModelState.IsValid)
            {
                var mapLoginModelToUser = new MapLoginModelToUser();
                var user = await mapLoginModelToUser.MapObject(loginModel);

                if(user == null)
                {
                    return Ok();
                }
                else
                {
                    var result = await this.authenticateUser.Authenticate(user);

                    if(result.Result == FIUChat.Enums.AuthenticateResult.Success)
                    {
                        // SUCCESSFUL LOGIN
                        // Creating and storing cookies
                        var token = this.GenerateToken(user.Email, user.PantherID, user.UserEntitlement);
                        return Json(new
                        {
                            access_token = token,
                            success = true
                        });
                    }
                    else
                    {
                        // Unsuccessful login
                        return Unauthorized();
                    }
                }
            }

            return BadRequest();
        }

        public IActionResult LoggedIn()
        {
            return View();    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private string GenerateToken(string email, string pantherId, Entitlement userEntitlement)
        {
            var token = new JwtSecurityToken(
                issuer:"localhost",
                audience: "localhost",
                claims: new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Sid, pantherId),
                    new Claim(ClaimTypes.Actor, userEntitlement.ToString()),
                },
                expires: DateTime.Now.AddDays(7),
                signingCredentials: new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
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
                PantherID = user.PantherID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserEntitlement = user.UserEntitlement
            };
        }
    }
}
=======
=======
>>>>>>> Stashed changes
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FIU_Chat.Models;
using FIUChat.DatabaseAccessObject;
using FIUChat.Identity;
using FIUChat.DatabaseAccessObject.CommandObjects;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using FIUChat.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FIU_Chat.Controllers
{
    public class AccountController : Controller
    {
        private const string SECRET_KEY = "FIUCHATSECRETKEY";
        public static readonly SymmetricSecurityKey SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        public IActionResult Index()
        {
            return View();
        }

        // Post: /login/
        [HttpPost]
        [Route("/[controller]/Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var mapLoginModelToUser = new MapLoginModelToUser();
                var user = await mapLoginModelToUser.MapObject(loginModel);

                // If login user with those credentials does not exist
                if(user == null)
                {
                    return Ok();
                }

                else
                {
                    var result = await this.authenticateUser.Authenticate(user);

                    if(result.Result == FIUChat.Enums.AuthenticateResult.Success)
                    {
                        // SUCCESSFUL LOGIN
                        // Creating and storing cookies
                        var token = this.GenerateToken(user.Email, user.PantherID, user.UserEntitlement);
                        return Json(new
                        {
                            access_token = token,
                            success = true
                        });
                    }
                    else
                    {
                        // Unsuccessful login
                        return Unauthorized();
                    }
                }
            }

            return BadRequest();
        }

        public IActionResult LoggedIn()
        {
            return View();    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private string GenerateToken(string email, string pantherId, Entitlement userEntitlement)
        {
            var token = new JwtSecurityToken(
                issuer:"localhost",
                audience: "localhost",
                claims: new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Sid, pantherId),
                    new Claim(ClaimTypes.Actor, userEntitlement.ToString()),
                },
                expires: DateTime.Now.AddDays(7),
                signingCredentials: new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
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
                PantherID = user.PantherID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserEntitlement = user.UserEntitlement
            };
        }
    }
}
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
