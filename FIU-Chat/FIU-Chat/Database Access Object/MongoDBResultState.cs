using System;
namespace FIUChat.DatabaseAccessObject
{
    public class MongoDBResultState
    {
        public MongoDBResult Result { get; set; }
        public string Message { get; set; }

        public MongoDBResultState()
        {
        }

        public MongoDBResultState(string message)
        {
            this.Message = message;
        }

        public MongoDBResultState(MongoDBResult result)
        {
            this.Result = result;
        }

        public MongoDBResultState(string message, MongoDBResult result)
        {
            this.Message = message;
            this.Result = result;
        }
    }

    public enum MongoDBResult
    {
        Success = 10,
        Failure = 20,
        AlreadyExists = 30,
        Unknown = 0
    }
}
