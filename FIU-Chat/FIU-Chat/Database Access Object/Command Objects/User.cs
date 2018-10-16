using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FIUChat.Enums;

namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class User : Command
    {
        public User(Guid? Id)
            : base(Id)
        {
            this.ClassDictionary = new Dictionary<string, List<Dictionary<string, string>>>();
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
        [Required]
        public Dictionary <string, List<Dictionary<string, string>>> ClassDictionary { get; set; }
    }
}
