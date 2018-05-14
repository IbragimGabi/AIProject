using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ServerAPI
{
    public class File
    {
        public int FileId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string CheckSum { get; set; }

        public int? UserId { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public User User { get; set; }
    }
}
