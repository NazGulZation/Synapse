using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBG.Synapse.Repository
{
    public class EmployeeTest
    {
        [PrimaryKey]
        public int Id { get; set; }

        [StrLength(100)]
        [NotNull]
        public string Name { get; set; }

        [StrLength(100)]
        [NotNull]
        public string Email { get; set; }

        [StrLength(100)]
        [NotNull]
        public string Department { get; set; }

        [NotNull]
        public DateTime DateOfBirth { get; set; }

        public decimal Salary { get; set; }
    }
}
