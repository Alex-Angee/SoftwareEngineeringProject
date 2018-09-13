using System;
namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public class Message : Command
    {
        public Message() : base(Guid.NewGuid())
        {
        }

        public string GroupChatName { get; set; }
        public string MessageContents { get; set; }
        public DateTime MessageSent { get; set; }
        public string Username { get; set; }
        public string PantherID { get; set; }
        public string Email { get; set; }
    }
}
