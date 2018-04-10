using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Password> Passwords { get; set; }

        public User()
        {
            Files = new List<File>();
            Passwords = new List<Password>();
        }
    }
}
