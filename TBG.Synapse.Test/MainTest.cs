using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBG.Synapse.Services;

namespace TBG.Synapse.Test
{
    [TestFixture]
    internal class MainTest
    {
        [Test]
        public void TestLoadFull()
        {
            var obj = Helper.LoadCsvToJson("Dataset", "data_revenue.csv");

        }


    }
}
