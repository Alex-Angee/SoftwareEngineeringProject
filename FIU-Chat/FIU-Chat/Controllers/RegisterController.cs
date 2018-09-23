using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FIUChat.DatabaseAccessObject;
using FIUChat.DatabaseAccessObject.CommandObjects;
using FIUChat.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FIU_Chat.Controllers
{
    [Produces("application/json")]
    public class RegisterController : Controller
    {
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();

        [HttpPost]
        [Route("api/[controller]/registeruser")]
        public async Task<MongoDBResultState> Register([FromBody]User user)
        { 
            if (!user.ClassDictionary.Any() && user.UserEntitlement == Entitlement.Student)
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Must be enrolled in a class to register." };
            }

            if(user.ID == Guid.Empty)
            {
                user.ID = Guid.NewGuid();
            }

            Expression<Func<User, bool>> expression = x =>
                x.Email == user.Email && x.PantherID == user.PantherID;

            var resultExists = await this.serverToStorageFacade.ReadObjectByExpression(user, expression);
            if (resultExists != null)
            {
                return new MongoDBResultState
                {
                    Result = MongoDBResult.AlreadyExists,
                    Message =
                        $"A user with the Panther ID: {user.PantherID} and Email: {user.Email} already exists."
                };
            }

            return await this.serverToStorageFacade.CreateObject(user);;
        }

        [HttpPost]
        [Route("api/[controller]/registerclass")]
        public async Task<MongoDBResultState> RegisterClass([FromBody]GroupChat chat)
        {
            if (chat.ClassName.Equals(string.Empty) || chat.CourseId.Equals(string.Empty) || chat.GroupChatName.Equals(string.Empty) || chat.CourseId.Equals(string.Empty) || chat.TimeOfCourse.Equals(string.Empty) || chat.ProfessorName.Equals(string.Empty))
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Missing a required field." };

            Expression<Func<GroupChat, bool>> expression = x =>
                x.CourseId == chat.CourseId && x.SectionId == chat.SectionId && x.ProfessorName == chat.ProfessorName && x.AcademicYear == chat.AcademicYear;

            var resultExists = await this.serverToStorageFacade.ReadObjectByExpression(chat, expression);
            if (resultExists != null)
            {
                return new MongoDBResultState
                {
                    Result = MongoDBResult.AlreadyExists,
                    Message =
                        $"The Group Chat with Course Id: {chat.CourseId}, Section Id: {chat.SectionId}, with Professor: {chat.ProfessorName} alreay exists."
                };
            }

            return await this.serverToStorageFacade.CreateObject(chat);
        }
    }
}