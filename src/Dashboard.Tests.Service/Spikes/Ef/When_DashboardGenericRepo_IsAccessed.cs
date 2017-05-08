using System;
using System.Linq;
using Compliance.Common.GenericRepo.Implementation;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Implementation.Ef;
using NUnit.Framework;

namespace Dashboard.Tests.Service.Spikes.Ef
{
    [TestFixture]
    public class When_DashboardGenericRepo_IsAccessed
    {
        [Test]
        public void _SeedData_Creates()
        {
            var repo = new GenericRepo<Group, DashboardContext>(new TestContext());

            Assert.AreEqual(1, repo.Query(g => g.GroupName == "Auditors").ToList().Count);
        }
    }
}
