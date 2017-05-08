using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using Compliance.Audio.Tests.Spikes.Ef.Config;
using Compliance.Audio.Implementation.Ef.Persistence.Data;

namespace Compliance.Audio.Tests.Spikes.Ef
{
    [TestFixture]
    public class When_RecordingRepo_IsAccessed
    {
        [Test]
        public void _Context_IsValid()
        {
            var ctx = new TestingContext().GetContext();
            var existingRecording = Guid.Parse("88A96F46-8225-4799-9F13-1F5145814545");
            var aRecord = ((RecordingContext)ctx).Recordings
                .Include("AccountReferences")
                .Include("CustomerReferences")
                .Include("DeskReferences")
                .Include("UserReferences")
                .Where(r => r.Id == existingRecording).FirstOrDefault();

            Assert.IsNotNull(aRecord);
            Assert.IsNotNull(aRecord.AccountReferences);
            Assert.IsNotNull(aRecord.CustomerReferences);
            Assert.IsNotNull(aRecord.DeskReferences);
            Assert.IsNotNull(aRecord.UserReferences);
        }
    }
}
