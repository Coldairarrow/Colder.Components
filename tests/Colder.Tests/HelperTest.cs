using Colder.DistributedId;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.Common.Tests
{
    [TestClass]
    public class HelperTest
    {
        [TestMethod]
        public void SequentialGuid()
        {
            List<Task> tasks = new List<Task>();
            BlockingCollection<Guid> guids = new BlockingCollection<Guid>();
            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 1000000; j++)
                    {
                        guids.Add(GuidHelper.NewGuid());
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(guids.Distinct().Count(), 4000000);
        }
    }
}
