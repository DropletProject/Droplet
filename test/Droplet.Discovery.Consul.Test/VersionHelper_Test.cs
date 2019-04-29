using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Droplet.Discovery.Consul.Test
{
    [TestClass]
    public class VersionHelper_Test
    {
        [TestMethod]
        public void GetVersionFromTags_Test()
        {
            string myVersion = "1.0.1";

            string version = VersionHelper.GetVersionFromTags(null);
            Assert.AreEqual(version, null);

            version = VersionHelper.GetVersionFromTags(new List<string>());
            Assert.AreEqual(version, null);

            version = VersionHelper.GetVersionFromTags(new List<string>() { "testTag1", "testTag2" });
            Assert.AreEqual(version, null);

            version = VersionHelper.GetVersionFromTags(new List<string>() { VersionHelper.GetVersion(myVersion), "testTag1", "testTag2" });
            Assert.AreEqual(version, myVersion);

        }

        [TestMethod]
        public void SetVersionToTags_Test()
        {
            string myVersion = "1.0.1";
            var tags = VersionHelper.SetVersionToTags(new List<string>(), myVersion);

            Assert.AreEqual(tags.Count, 1);
            Assert.AreEqual(tags[0], VersionHelper.GetVersion(myVersion));

        }


        [TestMethod]
        public void GetVersion_Test()
        {
            string myVersion = "1.0.1";
            
            Assert.AreEqual($"{VersionHelper.VERSION_PREFIX}{myVersion}", VersionHelper.GetVersion(myVersion));

        }
    }
}
