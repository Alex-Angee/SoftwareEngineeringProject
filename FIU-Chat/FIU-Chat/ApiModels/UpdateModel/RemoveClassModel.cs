using System;
using System.Collections.Generic;

namespace FIUChat.ApiModels.UpdateModel
{
    public class RemoveClassModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PantherId { get; set; }
        public List<string> RemoveClasses { get; set; }
    }
}
