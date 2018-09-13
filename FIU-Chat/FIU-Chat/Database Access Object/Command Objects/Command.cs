using System;
using MongoDB.Bson.Serialization.Attributes;

namespace FIUChat.DatabaseAccessObject.CommandObjects
{
    public abstract class Command
    {
        public Command(Guid? ID)
        {
            if(ID == null)
            {
                ID = Guid.NewGuid();
            }
            else
            {
                this.ID = ID.Value;
            }
        }

        [BsonId]
        public Guid ID { get; set; }
    }
}
