using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class User : Command
    {
        public User(Guid? Id)
            : base(Id)
        {
            this.ClassDictionary = new Dictionary<string, Dictionary<string, string>>();
        }

        [Required]
        public Entitlement UserEntitlement { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PantherID { get; set; }

        // Maps from Course ID to Section ID
        public Dictionary <string, Dictionary<string, string>> ClassDictionary { get; set; }
    }

    public enum Entitlement
    {
        Student = 10,
        Admin = 20,
        Bot = 30,
        Unknown = 0
    }
}
