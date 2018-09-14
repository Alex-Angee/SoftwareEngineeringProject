using System;
namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class GroupChat : Command
    {
        public GroupChat() : base(Guid.NewGuid())
        {
        }

        public string GroupChatName { get; set; }
        public string ClassName { get; set; }
        public string CourseId { get; set; }
        public string SectionId { get; set; }
        public string TimeOfCourse { get; set; }
        public string ProfessorName { get; set; }
    }
}
