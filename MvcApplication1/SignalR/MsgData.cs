using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchivesSocietyAB
{
    public class MsgData
    {
        public MsgData()
        {
            this.To = new List<string>();
        }
        public List<string> To { get; set; }
        public string SenderName { get; set; }
        public object Data { get; set; }
    }
}
