using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace ServerAPI
{
    public class Password
    {
        public int PasswordId { get; set; }
        public string Hash { get; set; }
        public DateTime StatusDate { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
