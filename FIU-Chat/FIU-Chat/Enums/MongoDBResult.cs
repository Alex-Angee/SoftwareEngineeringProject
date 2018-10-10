using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIUChat.Enums
{
    public enum MongoDBResult
    {
        Success = 10,
        Failure = 20,
        AlreadyExists = 30,
        Unknown = 0
    }
}
