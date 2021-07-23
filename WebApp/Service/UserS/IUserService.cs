using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models.Entity;

namespace WebApp.Service.UserS
{
    public interface IUserService
    {
        public Task<bool> CheckExistEmailUser(string email);
        public Task<User> FindEmailUserByEmailAndPassword(string email, string password);
        public Task CreateNewUser(User user);
        public Task UpdateUser(User user);
        public Task<User> GetUserById(int id);
    }
}
