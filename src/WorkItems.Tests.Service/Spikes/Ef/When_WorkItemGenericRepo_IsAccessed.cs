using System;
using System.Linq;
using Compliance.Common.GenericRepo.Implementation;
using Compliance.WorkItems.Domain;
using Compliance.WorkItems.Implementation.Ef;
using NUnit.Framework;
using WorkItems.Tests.Service.Spikes.Ef.Config;

namespace WorkItems.Tests.Service.Spikes.Ef
{
    [TestFixture]
    public class When_WorkItemGenericRepo_IsAccessed
    {
        [Test]
        public void _SeedData_Creates()
        {
            var repo = new GenericRepo<RecordingItem, WorkItemsContext>(new TestingContext());

            Assert.AreEqual(repo.Query(g => g.LogicalIdentifier == "SampleRecording").ToList().Count, 1);
        }
    }
}
