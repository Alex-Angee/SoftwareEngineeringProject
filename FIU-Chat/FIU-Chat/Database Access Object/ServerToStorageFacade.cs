using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        /// Initializes a new instance of the <see cref="ServerToStorageFacade"/> class.
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
        public async Task<MongoDBResultState> CreateObject<T>(T entity)
            where T : Command
        {
            return await this.mongoDB.CreateObject(entity);
        }

        /// <summary>
        /// Reads the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="entity">Entity.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<T> ReadObject<T>(T entity)
            where T : Command
        {
            return await this.mongoDB.ReadObject(entity);
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
            return await this.mongoDB.UpdateObject(entity);
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
            return await this.mongoDB.DeleteObject(entity);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns>The message.</returns>
        /// <param name="message">Message.</param>
        public async Task<MongoDBResultState> SendMessage(Message message)
        {
            return await this.mongoDB.SendMessage(message);
        }

        /// <summary>
        /// Reads the object by expression.
        /// </summary>
        /// <returns>The object by expression.</returns>
        /// <param name="entity">Entity.</param>
        /// <param name="expression">Expression.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task<T> ReadObjectByExpression<T>(T entity, Expression expression)
        {
            return await this.mongoDB.ReadObjectByExpression(entity, expression);
        }
    }
}
