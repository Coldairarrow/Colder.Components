using Colder.Common.Helpers;
using Colder.DistributedId;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [TestMethod]
        public void Tracking()
        {
            User empty = new User
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            User user = new User
            {
                Id = empty.Id,
                Name = empty.Name,
                Items = new List<User_Item>
                {
                    new User_Item
                    {
                        Id = Guid.NewGuid(),
                        Name = Guid.NewGuid().ToString(),
                        Items = new List<User_Item_Item>
                        {
                            new User_Item_Item
                            {
                                Id = Guid.NewGuid(),
                                Name = Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            };

            //添加
            var tracking = TrackingHelper.Tracking(empty.DeepClone(), user.DeepClone(), null, null);
            Assert.AreEqual(tracking.added.Count, 2);

            //删除
            tracking = TrackingHelper.Tracking(user.DeepClone(), empty.DeepClone(), null, null);
            Assert.AreEqual(tracking.removed.Count, 2);
        }
    }

    [Table(nameof(User))]
    class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User_Item> Items = new List<User_Item>();
    }

    [Table(nameof(User_Item))]
    class User_Item
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User_Item_Item> Items = new List<User_Item_Item>();
    }

    [Table(nameof(User_Item_Item))]
    class User_Item_Item
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
