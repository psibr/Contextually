using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contextually.Tests.Net45
{
    [TestClass]
    [TestCategory("AppDomain")]
    public class AppDomainTests
    {
        [TestMethod]
        public void InfoBlockTraversesAppDomainsCorrectly()
        {
            NameValueCollection blockValues;
            var key = "SomeKey";
            var value = "SomeValue";

            using (Relevant.Info(new NameValueCollection { { key, value } }))
            {
                var tempDomain = AppDomain.CreateDomain("tempDomain", 
                    AppDomain.CurrentDomain.Evidence,
                    AppDomain.CurrentDomain.BaseDirectory, 
                    AppDomain.CurrentDomain.RelativeSearchPath,
                    AppDomain.CurrentDomain.ShadowCopyFiles);

                blockValues = Relevant.Info();
            }

            Assert.IsNotNull(blockValues);
            Assert.AreEqual(1, blockValues.Count);
            Assert.AreEqual(key, blockValues.GetKey(0));

            var values = blockValues.GetValues(key);
            Assert.IsNotNull(values);
            Assert.AreEqual(1, values.Length);
            Assert.AreEqual(value, values[0]);
        }

    }
}
