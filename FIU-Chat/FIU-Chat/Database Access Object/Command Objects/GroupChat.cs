using System;
using System.ComponentModel.DataAnnotations;

namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class GroupChat : Command
    {
        public GroupChat(Guid? ID) 
            : base(ID)
        {
        }

        [Required]
        public string GroupChatName { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public string CourseId { get; set; }

        [Required]
        public string SectionId { get; set; }

        [Required]
        public string TimeOfCourse { get; set; }

        [Required]
        public string ProfessorName { get; set; }
    }
}
