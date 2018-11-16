using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FIUChat.Identity;
using FIUChat.DatabaseAccessObject;
using FIUChat.DatabaseAccessObject.CommandObjects;
using System.Linq.Expressions;

namespace FIUChat.Hubs
{
    public class ChatHub : Hub
    {
        private AuthenticateUser authenticateUser = new AuthenticateUser();
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade(); 

        public async Task SendMessage(string token, string className, string message)
        {
            var user =  await authenticateUser.GetUserFromToken(token);

            var name = user.FirstName + " " + user.LastName;

            await Clients.All.SendAsync("ReceiveMessage", name, message);

            string professorName = "", section= "";

            foreach (KeyValuePair<string, List<Dictionary<string, string>>> classes in user.ClassDictionary)
            {
                var currentClasses = classes.Value;

                foreach (var currentClass in currentClasses)
                {
                    foreach (KeyValuePair<string, string> foundClass in currentClass)
                    {
                        if(foundClass.Key == className)
                        {
                            professorName = classes.Key;
                            section = foundClass.Value;
                        }
                    }
                }
            }

            Expression<Func<GroupChat, bool>> expression = x => x.CourseId == className && x.SectionId == section && x.ProfessorName == professorName;

            var groupChat = await this.serverToStorageFacade.ReadObjectByExpression(new GroupChat(Guid.NewGuid()), expression);

            var storeMessage = new Message();
            storeMessage.Email = user.Email;
            storeMessage.GroupChatName = groupChat.GroupChatName;
            storeMessage.GroupChatID = groupChat.ID;
            storeMessage.MessageContents = message;
            storeMessage.MessageSent = DateTime.UtcNow;
            storeMessage.Username = name;
            storeMessage.PantherID = user.PantherID;

            await serverToStorageFacade.SendMessage(storeMessage);
        }
    }
}

