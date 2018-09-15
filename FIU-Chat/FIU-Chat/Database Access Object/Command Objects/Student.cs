using System;
namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class Student : Command
    {
        public Student(Guid? Id)
            : base(Id)
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PantherID { get; set; }
    }
}
