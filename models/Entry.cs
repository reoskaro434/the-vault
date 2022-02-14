using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.models
{
    class Entry
    {
        public string Data { get; set; }
        public string Key { get; set; }
        public DateTime CreationTime { get; set; }
        public Entry(string data, string key, DateTime creationTime)
        {
            Data = data;
            Key = key;
            CreationTime = creationTime;
        }

    }
}
