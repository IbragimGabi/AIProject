using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }

        public User GetUser(int id)
        {
            return _context.Users.SingleOrDefault(m => m.UserId == id);
        }

        public User GetUserByLoginPass(string login, string pass)
        {
            return _context.Users.SingleOrDefault(m => m.UserName == login && m.Password == pass);
        }

        public void UpdateUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Such user doesn't exist");
            }
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }


        public void DeleteUser(int id)
        {
            var user = _context.Users.SingleOrDefault(m => m.UserId == id);
            if (user == null)
            {
                return;
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public User GetUserByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(m => m.UserName == userName);
        }
    }
}
