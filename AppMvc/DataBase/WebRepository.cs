using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace AppMvc.DataBase
{
    public class WebRepository
    {
        private readonly WebContext context;

        public WebRepository(WebContext context)
        {
            this.context = context;
        }

        //public IEnumerable<User> GetUsers()
        //{
        //    IEnumerable<User> users = context.Users.ToList();
        //    return users;
        //}

        //public void StartTrasaction()
        //{
        //    var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted) ; // Start transaction and set Isolation level

        //    try
        //    {
        //        // ...
        //        // Some SQL scripts
        //        // ...

        //        transaction.CreateSavepoint("savepoint");

        //        // ...
        //        // Some more SQL scripts
        //        // ...

        //        transaction.Commit(); 
        //    }
        //    catch (System.Exception ex)
        //    {
        //        transaction.Rollback();

        //        // OR

        //        //transaction.RollbackToSavepoint("savepoint");
        //    }
            
        //}

        public async Task<IEnumerable<User>> GetUsersAsync() // Async GetUsers
        {
            IEnumerable<User> users = await context.Users.ToListAsync();
            return users;
        }

        public User GetUserById(int id)
        {
            User user = context.Users.FirstOrDefault(u => u.Id == id);
            return user;
        }

        //public async Task<User> GetUserByIdAsync(int id) // Async GetUserById
        //{
        //    User user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        //    return user;
        //}

        public void SaveUser(User user) 
        {
            context.Entry(user).State = EntityState.Added;
            context.SaveChanges();
        }

        public async Task SaveUserAsync(User user) // Async SaveUser
        {
            context.Entry(user).State = EntityState.Added;
            await context.SaveChangesAsync();
        }
    }
}
