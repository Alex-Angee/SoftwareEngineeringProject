using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIUChat.DatabaseAccessObject;
using FIUChat.DatabaseAccessObject.CommandObjects;
using Microsoft.AspNetCore.Mvc;

namespace FIU_Chat.Controllers
{
    [Produces("application/json")]
    public class RegisterController : Controller
    {
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();

        [HttpPost]
        [Route("api/[controller]/register")]
        public async Task<MongoDBResultState> Register([FromBody]User user)
        {
            if (!user.classDictionary.Any())
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Must be enrolled in a class to register." };
            }

            if(user.ID == Guid.Empty)
            {
                user.ID = Guid.NewGuid();
            }

            var result = await this.serverToStorageFacade.CreateObject(user);
            return result;
        }

        [HttpPost]
        [Route("api/[controller]/registerclass")]
        public async Task<MongoDBResultState> RegisterClass([FromBody]GroupChat chat)
        {
            if (chat.ClassName.Equals(string.Empty) || chat.CourseId.Equals(string.Empty) || chat.GroupChatName.Equals(string.Empty) || chat.CourseId.Equals(string.Empty) || chat.TimeOfCourse.Equals(string.Empty) || chat.ProfessorName.Equals(string.Empty))
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Missing a required field." };

            var result = await this.serverToStorageFacade.CreateObject(chat);

            return result;
        }
    }
}