 using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        
        public DatabaseCRUD(WebshopContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get database object by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync<T>(int id) where T : class
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

        public async Task<User> GetUserByUserEmail(string email)
        {
            return await context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Insert new row into database and returns number of effected rows
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(T obj)
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
        public Task<int> UpdateAsync<T>(T obj)
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
        public async Task<int> DeleteAsync<T>(T obj)
        {
            context.Remove(obj);
            return await CommitChanges();
        }

       public async Task<int> DeleteAsynById<T>(T obj)
        {
            
            return await CommitChanges();
        }

        public string GetCategoryName(int categorid)
        {
            var categoryName= context.Categories
                .Where(x => x.Id == categorid).
                Select(x => x.Name).FirstOrDefault();
            return (categoryName);
        }

        public async Task<int> CommitChanges()
        { 
            return await context.SaveChangesAsync();
        }

       

    }
}
