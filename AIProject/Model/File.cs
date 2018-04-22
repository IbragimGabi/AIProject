using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIProject
{
    public class File
    {
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
