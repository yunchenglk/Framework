using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YunChengLK.Framework.Logging
{
    [Serializable]
    public class LogInfo
    {
        public string AppName { get; set; }
        public string Cate { get; set; }
        public string Ip { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public Exception Excep { get; set; }
    }
}
