using System;
namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class Student : Command
    {
        public Student(Guid? Id)
            : base(Id)
        {
        }

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PantherID { get; set; }
        public string Email { get; set; }
    }
}
