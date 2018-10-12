using System;
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
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FIU_Chat.Controllers
{
    public class AccountController : Controller
    {
        private const string SECRET_KEY = "FIUCHATSECRETKEY";
        public static SymmetricSecurityKey SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        public IActionResult Index()
        {
            return View();
        }

        // Post: /login/
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var mapLoginModelToUser = new MapLoginModelToUser();
                var user = await mapLoginModelToUser.MapObject(loginModel);

                // If login user with those credentials does not exist
                if(user == null)
                {
                    // Unsuccessful login
                    var unsuccessfulToken = Json(new
                    {
                        success = false
                    });
                    return Ok(unsuccessfulToken);
                }

                else
                {
                    var result = await this.authenticateUser.Authenticate(user);

                    if(result.Result == FIUChat.Enums.AuthenticateResult.Success)
                    {
                        // SUCCESSFUL LOGIN
                        // Creating and storing cookies

                        var successfulToken = Json(new
                        {
                            data = this.GenerateToken(user.Email, user.PantherID),
                            redirectUrl = Url.Action("Index","Home"),
                            classes = user.ClassDictionary,
                            success = true
                        });
                        return Ok(successfulToken);
                    }
                    else
                    {
                        // Unsuccessful login
                        var unsuccessfulToken = Json(new
                        {
                            success = false
                        });
                        return Ok(unsuccessfulToken);
                    }
                }
            }

            // Unsuccessful login
            var token = Json(new
            {
                success = false
            });
            return Ok(token);
        }

        private string GenerateToken(string email, string pantherId)
        {
            var claimsData = new[] { new Claim(ClaimTypes.Email, email), new Claim(ClaimTypes.Actor, pantherId) };

            var signInCredentials = new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "localhost",
                audience: "localhost",
                expires: DateTime.Now.AddDays(7),
                claims: claimsData,
                signingCredentials: signInCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            loginModel.inputEmail = loginModel.inputEmail.ToLower();
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
                PantherID = user.PantherID,
                ClassDictionary = user.ClassDictionary,
                UserEntitlement = user.UserEntitlement
            };
        }
    }
}
