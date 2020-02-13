using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Context
{
    public class DatabaseCRUD
    {
        private readonly WebshopContext context;

        public DatabaseCRUD()
        {
            context = new WebshopContext();
        }

        /// <summary>
        /// Get database object by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Get<T>(int id) where T : class
        {
            return await context.FindAsync<T>(id);
        }

        /// <summary>
        /// Get user by provided email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User GetUserByUserCredentials(string email, string password)
        {
            return context.Users.Where(x => x.Email == email && x.Password == password).FirstOrDefault();
        }

        /// <summary>
        /// Insert new row into database and returns number of effected rows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<int> Insert<T>(T obj)
        {
            context.Add(obj);
            return await CommitChanges();
        }

        /// <summary>
        /// Update row by provided object and returns number of effected rows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task<int> Update<T>(T obj)
        {
            context.Update(obj);
            return CommitChanges();
        }

        /// <summary>
        /// Remove an object from database and return number of effected rows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<int> Delete<T>(T obj)
        {
            context.Remove(obj);
            return await CommitChanges();
        }

        public async Task<int> CommitChanges()
        { 
            return await context.SaveChangesAsync();
        }

    }
}
