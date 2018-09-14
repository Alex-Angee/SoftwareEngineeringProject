using System;
using System.Collections.Generic;
using FIUChat.DatabaseAccessObject.CommandObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FIUChat.DatabaseAccessObject
{
    public class MongoDB
    {
        private static readonly MongoDB mongoDB = new MongoDB();
        private const string DBName = "FIUChat";

        static MongoDB()
        {
        }

        public static MongoDB Instance
        {
            get
            {
                return mongoDB;
            }
        }

        /// <summary>
        /// Connects to mongo db.
        /// </summary>
        /// <returns><c>true</c>, if to mongo db was connected, <c>false</c> otherwise.</returns>
        /// <param name="entity">Entity.</param>
        private MongoDBResultState ExecuteMongoDBCommand<T>(T entity, MongoDBExecuteCommand mongoDBExecuteCommand)
            where T: Command
        {
            MongoDBResultState mongoDBResultState = new MongoDBResultState();
            try
            {
                var collection = this.ConnectToMongo(entity);
                if (collection == null)
                {
                    return new MongoDBResultState
                    {
                        Result = MongoDBResult.Failure,
                        Message = $"Unable to create collection {entity.GetType().Name}"
                    };
                }

                switch(mongoDBExecuteCommand)
                {
                    case MongoDBExecuteCommand.Create:
                        collection.InsertOne(entity);
                        mongoDBResultState.Message = "Successfully created Student in the database.";
                        mongoDBResultState.Result = MongoDBResult.Success;
                        break;

                    case MongoDBExecuteCommand.Update:
                        collection.ReplaceOne(x => x.ID == entity.ID, entity);
                        break;

                    case MongoDBExecuteCommand.Delete:
                        collection.DeleteOne(x => x.ID == entity.ID);
                        break;

                    default:
                        mongoDBResultState.Message = $"There was an unknown command executed with ID: {entity.ID}.";
                        mongoDBResultState.Result = MongoDBResult.Failure;
                        break;
                }

            }
            catch(MongoDuplicateKeyException dupEx)
            {
                mongoDBResultState.Message = dupEx.Message;
                mongoDBResultState.Result = MongoDBResult.AlreadyExists;
            }
            catch(Exception ex)
            {
                mongoDBResultState.Message = ex.Message;
                mongoDBResultState.Result = MongoDBResult.Failure;
            }
            return mongoDBResultState;
        }

        /// <summary>
        /// Finds the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private T FindObject<T>(T entity)
            where T : Command
        {
            T result;

            try
            {
                var command = entity as Command;

                var collection = this.ConnectToMongo(entity);
                if (collection == null)
                {
                    Console.WriteLine($"Collection: {entity.GetType()} does not exsit.");
                    return null;
                }

                result = collection.Find(x => x.ID == command.ID).FirstOrDefault();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return result;
        }

        /// <summary>
        /// Connects to mongo.
        /// </summary>
        /// <returns>The to mongo.</returns>
        /// <param name="entity">Entity.</param>
        private IMongoCollection<T> ConnectToMongo<T>(T entity)
        {
            IMongoCollection<T> collection = null;
            try
            {
                MongoClientSettings setting = new MongoClientSettings
                {
                    Server = new MongoServerAddress("localhost", 27017)
                };
                MongoClient client = new MongoClient(setting);
                var mongoDbServer = client.GetDatabase(DBName);

                collection = mongoDbServer.GetCollection<T>($"{entity.GetType().Name}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return collection;
        }

        /// <summary>
        /// Sends the message to mongo db.
        /// </summary>
        /// <returns>The message to mongo db.</returns>
        /// <param name="entity">Message.</param>
        private MongoDBResultState SendMessageToMongoDB<T>(T entity)
            where T : Message
        {
            MongoDBResultState mongoDBResultState = new MongoDBResultState();
            try
            {
                MongoClientSettings setting = new MongoClientSettings
                {
                    Server = new MongoServerAddress("localhost", 27017)
                };
                MongoClient client = new MongoClient(setting);
                var mongoDbServer = client.GetDatabase(DBName);

                var collection = mongoDbServer.GetCollection<T>($"{entity.GroupChatName}");

                collection.InsertOne(entity);
                mongoDBResultState.Result = MongoDBResult.Success;
                mongoDBResultState.Message = $"Successfully stored message in the {entity.GroupChatName} collection.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                mongoDBResultState.Message = ex.Message;
                mongoDBResultState.Result = MongoDBResult.Failure;
            }
            return mongoDBResultState;
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public MongoDBResultState CreateObject<T>(T entity)
            where T : Command
        {
            return ExecuteMongoDBCommand(entity, MongoDBExecuteCommand.Create);
        }

        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Id.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T ReadObject<T>(T entity)
            where T : Command
        {
            return this.FindObject(entity);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public MongoDBResultState UpdateObject<T>(T entity)
            where T : Command
        {
            return ExecuteMongoDBCommand(entity, MongoDBExecuteCommand.Update);
        }

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public MongoDBResultState DeleteObject<T>(T entity)
            where T : Command
        {
            return ExecuteMongoDBCommand(entity, MongoDBExecuteCommand.Delete);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="message">Message.</param>
        public MongoDBResultState SendMessage(Message message)
        {
            return this.SendMessageToMongoDB(message);
        }
    }

    /// <summary>
    /// Mongo DBE xecute command.
    /// </summary>
    enum MongoDBExecuteCommand
    {
        Create = 10,
        Read = 20,
        Update = 30,
        Delete = 40,
        Unknown = 50
    }
}
