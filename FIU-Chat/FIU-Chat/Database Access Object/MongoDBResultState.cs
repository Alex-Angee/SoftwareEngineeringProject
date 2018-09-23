using System;
using FIUChat.Enums;

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
}
