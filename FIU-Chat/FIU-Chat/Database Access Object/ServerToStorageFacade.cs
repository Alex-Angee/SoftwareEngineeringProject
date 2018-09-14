using System;
using FIUChat.DatabaseAccessObject.CommandObjects;

namespace FIUChat.DatabaseAccessObject
{
    public class ServerToStorageFacade
    {
        /// <summary>
        /// The mongo db.
        /// </summary>
        private MongoDB mongoDB;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FIUChat.DatabaseAccessObject.ServerToStorageFacade"/> class.
        /// </summary>
        public ServerToStorageFacade()
        {
            mongoDB = MongoDB.Instance;
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
            return mongoDB.CreateObject(entity);
        }

        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T ReadObject<T>(T entity)
            where T : Command
        {
            return mongoDB.ReadObject(entity);
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
            return mongoDB.UpdateObject(entity);
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
            return mongoDB.DeleteObject(entity);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="message">Message.</param>
        public MongoDBResultState SendMessage(Message message)
        {
            return mongoDB.SendMessage(message);
        }
    }
}
