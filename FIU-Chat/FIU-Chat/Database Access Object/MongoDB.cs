using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FIUChat.DatabaseAccessObject.CommandObjects;
using FIUChat.Enums;
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
        private async Task<MongoDBResultState> ExecuteMongoDBCommand<T>(T entity, MongoDBExecuteCommand mongoDBExecuteCommand)
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
                        await collection.InsertOneAsync(entity);
                        mongoDBResultState.Message = $"Successfully created {entity.GetType()} in the database.";
                        mongoDBResultState.Result = MongoDBResult.Success;
                        break;

                    case MongoDBExecuteCommand.Update:
                        await collection.ReplaceOneAsync(x => x.ID == entity.ID, entity);
                        mongoDBResultState.Message = $"Successfully updated {entity.GetType()} in the collection.";
                        mongoDBResultState.Result = MongoDBResult.Success;
                        break;

                    case MongoDBExecuteCommand.Delete:
                        await collection.DeleteOneAsync(x => x.ID == entity.ID);
                        mongoDBResultState.Message = $"Successfully deleted {entity.GetType()} in the collection.";
                        mongoDBResultState.Result = MongoDBResult.Success;
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
        private async Task<T> FindObject<T>(T entity)
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
                    return default(T);
                }

                result = (T)await collection.FindAsync(x => x.ID == command.ID);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }

            return result;
        }

        /// <summary>
        /// Finds the object by expression.
        /// </summary>
        /// <returns>The object by expression.</returns>
        /// <param name="entity">Entity.</param>
        /// <param name="expression">Expression.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private async Task<T> FindObjectByExpression<T>(T entity, Expression<Func<T, bool>> expression)
        {
            T result;

            try
            {
                var command = entity as Command;

                var collection = this.ConnectToMongo(entity);
                if (collection == null)
                {
                    Console.WriteLine($"Collection: {entity.GetType()} does not exsit.");
                    return default(T);
                }

                var filterDefinition = Builders<T>.Filter.Where(expression);
                var results = await collection.Find(filterDefinition).ToListAsync();
                result = results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
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
        private async Task<MongoDBResultState> SendMessageToMongoDB<T>(T entity)
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

                var collection = mongoDbServer.GetCollection<T>($"{entity.GroupChatName}Messages");

                await collection.InsertOneAsync(entity);
                mongoDBResultState.Result = MongoDBResult.Success;
                mongoDBResultState.Message = $"Successfully stored message in the {entity.GroupChatName}Messages collection.";
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
        public async Task<MongoDBResultState> CreateObject<T>(T entity)
            where T : Command
        {
            return await ExecuteMongoDBCommand(entity, MongoDBExecuteCommand.Create);
        }

        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Id.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<T> ReadObject<T>(T entity)
            where T : Command
        {
            return await this.FindObject(entity);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<MongoDBResultState> UpdateObject<T>(T entity)
            where T : Command
        {
            return await ExecuteMongoDBCommand(entity, MongoDBExecuteCommand.Update);
        }

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<MongoDBResultState> DeleteObject<T>(T entity)
            where T : Command
        {
            return await ExecuteMongoDBCommand(entity, MongoDBExecuteCommand.Delete);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="message">Message.</param>
        public async Task<MongoDBResultState> SendMessage(Message message)
        {
            return await this.SendMessageToMongoDB(message);
        }

        /// <summary>
        /// Reads the object by expression.
        /// </summary>
        /// <returns>The object by expression.</returns>
        /// <param name="entity">Entity.</param>
        /// <param name="expression">Expression.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<T> ReadObjectByExpression<T>(T entity, Expression<Func<T, bool>> expression)
        {
            return await this.FindObjectByExpression(entity, expression);
        }
    }
}
