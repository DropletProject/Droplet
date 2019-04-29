using System.Threading.Tasks;
using Droplet.RawRabbit.AutoSubscribe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Droplet.RawRabbit.Test.AutoSubscribe
{
    [TestClass]
    public class ConsumeAsyncTypeFinder_Test
    {
        [TestMethod]
        public void TestMethod1()
        {
            var ass = ConsumeAsyncTypeFinder.Get(typeof(ConsumeAsyncTypeFinder_Test).Assembly);
            Assert.IsTrue(ass.Count() > 0);
            Assert.IsTrue(ass.Any(s=>s == typeof(TestFinderConsumeAsyncType)));
        }
    }

    public class TestFinderMessage
    {

    }

    public class TestFinderConsumeAsyncType : IConsumeAsync<TestFinderMessage>
    {
        public Task ConsumeAsync(TestFinderMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
