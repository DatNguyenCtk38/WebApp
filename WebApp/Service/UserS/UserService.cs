using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Helper;
using WebApp.Models.Entity;

namespace WebApp.Service.UserS
{
    public class UserService : IUserService
    {
        private readonly WebAppContext _dbContext;
        public UserService(WebAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CheckExistEmailUser(string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email.Equals(email));
        }

        public async Task CreateNewUser(User user)
        {
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> FindEmailUserByEmailAndPassword(string email, string password)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == EncryptHelper.EncryptPassword(password));
        }

        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users.Include(x => x.Documents).FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task UpdateUser(User updatedUser)
        {
            var user = await GetUserById(updatedUser.ID);
            if (user != null)
            {
                user.Phone = updatedUser.Phone;
                user.Address = updatedUser.Address;
                user.Birthdate = updatedUser.Birthdate;
                user.Email = updatedUser.Email;
                user.FullName = updatedUser.FullName;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
