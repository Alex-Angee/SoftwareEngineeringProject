using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BCrypt.Net;
using FIUChat.DatabaseAccessObject;
using FIUChat.DatabaseAccessObject.CommandObjects;
using FIUChat.Enums;

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

    }
}
