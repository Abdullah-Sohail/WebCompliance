using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Implementation.Ef;
using Compliance.Dashboard.Implementation.Ef.Services;
using Moq;
using NUnit.Framework;

namespace Dashboard.Tests.Service.Spikes.Implementations
{
    [TestFixture]
    public class When_DashProfile_IsCreated
    {
        [Test]
        public void _EmptyId_Throws()
        {
            var repoMoq = new Mock<IGenericRepo<DashProfile, DashboardContext>>();
            var service = new DashProfileService(repoMoq.Object);
            var newProfile = new DashProfile();

            Assert.Throws(typeof(Exception), () => {
                service.CreateProfile(newProfile);
            });

        }

        [Test]
        public void _DuplicateId_Throws()
        {
            var repoMoq = new Mock<IGenericRepo<DashProfile, DashboardContext>>();
            var service = new DashProfileService(repoMoq.Object);
            var testGuid = Guid.NewGuid();
            var newProfile = new DashProfile() { Id = testGuid };

            //Mock that any getbyid returns our profile that should error
            repoMoq.Setup(repo => repo.GetById(It.IsAny<Guid>()))
                .Returns(new DashProfile() { Id = testGuid });

            Assert.Throws(typeof(Exception), () =>
            {
                service.CreateProfile(newProfile);
            });
        }

        [Test]
        public void _DuplicateADName_Throws()
        {
            var repoMoq = new Mock<IGenericRepo<DashProfile, DashboardContext>>();
            var service = new DashProfileService(repoMoq.Object);
            var testName = "testAD\\testName";
            var newProfile = new DashProfile() { Id = Guid.NewGuid(), FirstName = testName };
            var badProfile = new DashProfile() { Id = Guid.NewGuid(), FirstName = testName };

            //Mock that any query returns our profile that should error
            repoMoq.Setup(repo => repo.Query(It.IsAny<Expression<Func<DashProfile, bool>>>(), null))
                .Returns(new List<DashProfile>() { badProfile }.AsQueryable());

            Assert.Throws(typeof(Exception), () =>
            {
                service.CreateProfile(newProfile);
            });
        }
    }
}
