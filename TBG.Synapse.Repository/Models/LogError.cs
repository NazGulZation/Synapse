using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBG.Synapse.Repository
{
    public class LogError
    {
        [PrimaryKey]
        [StrLength(36)]
        public string ID { get; set; }
        [StrLength(1023)]
        [NotNull]
        public string Message { get; set; }
        [StrLength(255)]
        public string FileName { get; set; }
        [StrLength(255)]
        public int Line { get; set; }
        [StrLength(255)]
        public int Column { get; set; }
        [StrLength(1023)]
        public string Code { get; set; }
        [NotNull]
        public DateTime TimeStamp { get; set; }
    }
}
