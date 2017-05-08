using System;
using System.Linq;
using Compliance.Common.GenericRepo.Implementation;
using Compliance.Queuing.Domain;
using Compliance.Queuing.Implementation.Ef;
using NUnit.Framework;
using Queuing.Tests.Service.Spikes.Ef.Config;

namespace Queuing.Tests.Service.Spikes.Ef
{
    [TestFixture]
    public class When_QueuingGenericRepo_IsAccessed
    {
        [Test]
        public void _SeedData_Creates()
        {
            var repo = new GenericRepo<PullQueue, QueuingContext>(new TestingContext());

            Assert.AreEqual(repo.Query(g => g.QueueName == "FirstQueue").ToList().Count, 1);
        }
    }
}
