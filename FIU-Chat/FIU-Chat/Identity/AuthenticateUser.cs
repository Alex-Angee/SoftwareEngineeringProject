using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using FIUChat.DatabaseAccessObject;
using FIUChat.DatabaseAccessObject.CommandObjects;
using FIUChat.Enums;
using FIU_Chat.Controllers;
using Microsoft.IdentityModel.Tokens;

namespace FIUChat.Identity
{
    public class AuthenticateUser
    {
        private ServerToStorageFacade serverToStorageFacade;

        public AuthenticateUser()
        {
            serverToStorageFacade = new ServerToStorageFacade();
        }

        /// <summary>
        /// Hashs the password.
        /// </summary>
        /// <returns>The password.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T HashPassword<T>(T entity)
            where T : Command
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password, salt);

            entity.Password = hashedPassword;
            return entity;
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<AuthenticateResultState> Authenticate<T>(T entity)
            where T : Command
        {
            Expression<Func<T, bool>> expression = x => x.Email == entity.Email;
            var hashedUserPass = this.HashPassword(entity);

            var foundUser = await this.serverToStorageFacade.ReadObjectByExpression(entity, expression);

            if (!BCrypt.Net.BCrypt.Verify(foundUser.Password, hashedUserPass.Password))
            {
                return new AuthenticateResultState
                {
                    Result = AuthenticateResult.Failure,
                    Message = "The email or password is incorrect."
                };
            }
            else
            {
                return new AuthenticateResultState
                {
                    Result = AuthenticateResult.Success,
                    Message = "Successfully Logged in."
                };
            }
        }

        public async Task<AuthenticateResultState> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validateToken;
            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validateToken);
            }
            catch (SecurityTokenException)
            {
                return new AuthenticateResultState
                {
                    Message = "You have an invalid token.",
                    Result = AuthenticateResult.Failure
                };
            }

            return new AuthenticateResultState
            {
                Message = "Valid token.",
                Result = AuthenticateResult.Success
            };
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = "localhost",
                ValidAudience = "localhost",
                IssuerSigningKey = AccountController.SIGNING_KEY // The same key as the one that generate the token
            };
        }
    }
}
