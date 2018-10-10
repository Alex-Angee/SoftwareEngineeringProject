using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIUChat.Enums
{
    /// <summary>
    /// Mongo DBE xecute command.
    /// </summary>
    public enum MongoDBExecuteCommand
    {
        Create = 10,
        Read = 20,
        Update = 30,
        Delete = 40,
        Unknown = 50
    }
}
