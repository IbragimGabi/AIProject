using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI.DataAccess
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUser(int id);
        void UpdateUser(int id, User user);
        void AddUser(User user);
        void DeleteUser(int id);
        bool UserExists(int id);
        User GetUserByLoginPass(string login, string pass);
    }
}
