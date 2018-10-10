using System;
using FIUChat.Enums;

namespace FIUChat.Identity
{
    public class AuthenticateResultState
    {
        public string Message { get; set; }
        public AuthenticateResult Result { get; set; }

        public AuthenticateResultState()
        {
        }

        public AuthenticateResultState(string message)
        {
            this.Message = message;
        }

        public AuthenticateResultState(AuthenticateResult result)
        {
            this.Result = result;
        }

        public AuthenticateResultState(string message, AuthenticateResult result)
        {
            this.Message = message;
            this.Result = result;
        }
    }
}
