using System;
using System.Collections.Generic;

namespace FIUChat.ApiModels.UpdateModel
{
    public class AddClassModel
    {
        public string Email { get; set; } 
        public string Password { get; set; }
        public string PantherId { get; set; }
        public Dictionary<string, Dictionary<string, string>> NewClasses { get; set; }
    }
}
