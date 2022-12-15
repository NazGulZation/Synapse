using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBG.Synapse.Repository
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {

    }
    public class NotNullAttribute : Attribute
    {

    }
    public class StrLengthAttribute : Attribute
    {
        public int Length { get; set; }

        public StrLengthAttribute(int length)
        {
            Length = length;
        }
    }
}
