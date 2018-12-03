using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FIUChat.ApiModels.UpdateModel;
using FIUChat.DatabaseAccessObject;
using FIUChat.DatabaseAccessObject.CommandObjects;
using FIUChat.Enums;
using FIUChat.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FIU_Chat.Controllers
{
    [Produces("application/json")]
    public class UpdateController : Controller
    {
        private ServerToStorageFacade serverToStorageFacade = new ServerToStorageFacade();
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        [HttpPost]
        [Route("api/[controller]/replacestudentclass")]
        public async Task<MongoDBResultState> ReplaceStudentClass([FromBody] AddClassModel addClass)
        {
            if (!addClass.NewClasses.Any())
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Must contain classes in the dictionary." };
            }

            User user = new User(Guid.NewGuid())
            {
                Email = addClass.Email.ToLower(),
                PantherID = addClass.PantherId,
                Password = addClass.Password
            };

            Expression<Func<User, bool>> expression = x =>
                x.Email == user.Email && x.PantherID == addClass.PantherId;

            var resultExists = await this.serverToStorageFacade.ReadObjectByExpression(user, expression);
            if (resultExists == null)
            {
                return new MongoDBResultState
                {
                    Result = MongoDBResult.AlreadyExists,
                    Message =
                        $"A user with the Panther ID: {addClass.PantherId} and Email: {addClass.Email} does not exists."
                };
            }

            var result = await this.authenticateUser.Authenticate(user);
            if (result.Result != AuthenticateResult.Success)
            {
                return new MongoDBResultState{ Result = MongoDBResult.Failure, Message = "Email/Password is incorrect." };
            }

            resultExists.ClassDictionary.Clear();
            
            foreach(KeyValuePair<string, Dictionary<string, string>> addClasses in addClass.NewClasses)
            {
                List<Dictionary<string, string>> addThis = new List<Dictionary<string, string>>();
                addThis.Add(addClasses.Value);

                resultExists.ClassDictionary.Add(addClasses.Key, addThis);
            }

            return await this.serverToStorageFacade.UpdateObject(resultExists);
        }

        [HttpPost]
        [Route("api/[controller]/addstudentclass")]
        public async Task<MongoDBResultState> AddStudentClass([FromBody] AddClassModel addClass)
        {
            if (!addClass.NewClasses.Any())
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Must contain classes in the dictionary." };
            }

            User user = new User(Guid.NewGuid())
            {
                Email = addClass.Email.ToLower(),
                PantherID = addClass.PantherId,
                Password = addClass.Password
            };

            Expression<Func<User, bool>> expression = x =>
                x.Email == user.Email && x.PantherID == addClass.PantherId;

            var resultExists = await this.serverToStorageFacade.ReadObjectByExpression(user, expression);
            if (resultExists == null)
            {
                return new MongoDBResultState
                {
                    Result = MongoDBResult.AlreadyExists,
                    Message =
                        $"A user with the Panther ID: {addClass.PantherId} and Email: {addClass.Email} does not exists."
                };
            }

            var result = await this.authenticateUser.Authenticate(user);
            if (result.Result != AuthenticateResult.Success)
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Email/Password is incorrect." };
            }

            List<List<Dictionary<string, string>>> addThisClass = new List<List<Dictionary<string, string>>>();
            List<string> keys = new List<string>();

            foreach (KeyValuePair<string, List<Dictionary<string, string>>> addClasses in resultExists.ClassDictionary)
            {
                foreach(KeyValuePair<string, Dictionary<string, string>> newClass in addClass.NewClasses)
                {
                    if(addClasses.Key == newClass.Key)
                    {
                        resultExists.ClassDictionary[addClasses.Key].Add(newClass.Value);
                    }
                    else
                    {
                        addThisClass.Add(new List<Dictionary<string, string>> { newClass.Value });
                        keys.Add(newClass.Key);
                    }
                }
            }

            for(int i = 0; i < keys.Count; i++)
            {
                resultExists.ClassDictionary.Add(keys[i], addThisClass[i]);
            }

            return await this.serverToStorageFacade.UpdateObject(resultExists);
        }

        [HttpPost]
        [Route("api/[controller]/removestudentclass")]
        public async Task<MongoDBResultState> RemoveStudentClass([FromBody] RemoveClassModel removeClass)
        {
            if (!removeClass.RemoveClasses.Any())
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Must contain classes in the dictionary." };
            }

            User user = new User(Guid.NewGuid())
            {
                Email = removeClass.Email.ToLower(),
                PantherID = removeClass.PantherId,
                Password = removeClass.Password
            };

            Expression<Func<User, bool>> expression = x =>
                x.Email == user.Email && x.PantherID == removeClass.PantherId;

            var resultExists = await this.serverToStorageFacade.ReadObjectByExpression(user, expression);
            if (resultExists == null)
            {
                return new MongoDBResultState
                {
                    Result = MongoDBResult.AlreadyExists,
                    Message =
                        $"A user with the Panther ID: {removeClass.PantherId} and Email: {removeClass.Email} does not exists."
                };
            }

            var result = await this.authenticateUser.Authenticate(user);
            if (result.Result != AuthenticateResult.Success)
            {
                return new MongoDBResultState { Result = MongoDBResult.Failure, Message = "Email/Password is incorrect." };
            }

            Dictionary<string, List<Dictionary<string, string>>> doNotRemove = new Dictionary<string, List<Dictionary<string, string>>>();

            foreach (KeyValuePair<string, List<Dictionary<string, string>>> classes in resultExists.ClassDictionary)
            {
                var currentClasses = classes.Value;

                foreach(var currentClass in currentClasses)
                {
                    foreach(KeyValuePair<string, string> doNotemoveThisClass in currentClass)
                    {
                        if (!removeClass.RemoveClasses.Contains(doNotemoveThisClass.Key))
                        {
                            Dictionary<string, string> notRemove = new Dictionary<string, string>();
                            notRemove.Add(doNotemoveThisClass.Key, doNotemoveThisClass.Value);
                            if(!doNotRemove.ContainsKey(classes.Key))
                            {
                                doNotRemove.Add(classes.Key, new List<Dictionary<string, string>>());
                            }
                            doNotRemove[classes.Key].Add(notRemove);
                        }
                    }
                }
            }

            resultExists.ClassDictionary = new Dictionary<string, List<Dictionary<string, string>>>(doNotRemove);

            return await this.serverToStorageFacade.UpdateObject(resultExists);
        }
    }
}