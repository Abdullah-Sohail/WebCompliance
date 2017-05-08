using System;
using Compliance.Dashboard.Domain;
using NUnit.Framework;

namespace Dashboard.Tests.Domain.Spikes.Models
{
    [TestFixture]
    public class When_DashProfile_IsCreated
    {
        [Test]
        public void _Identity_IsGenerated()
        {
            var newProfile = DashProfile.Create("someusername");
            
            Assert.IsInstanceOf<Guid>(newProfile.Id);

            Assert.AreNotEqual(newProfile.Id, Guid.Empty);

        }
    }
}
