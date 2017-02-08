using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Contextually.Tests
{
    [TestClass]
    public class NameValueCollectionTests
    {
        [TestMethod]
        public void NameValueCollectionWorksAsIntended()
        {
            // ARRANGE
            var namedValues = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            // ACT
            namedValues.Add(new NameValueCollection { ["Key1"] = "Value2" });

            var values = namedValues.GetValues("Key1");

            // ASSERT
            Assert.AreEqual(1, namedValues.Count, "should have 1 pair.");
            Assert.AreEqual(2, values.Length, "should have 2 values");
            Assert.AreEqual("Value1", values[0], "first value should have lowest index.");
            Assert.AreEqual("Value2", values[1], "second value should be the second index.");
            Assert.AreEqual("Value1,Value2", namedValues["Key1"], "perferred value should be first value.");
        }
    }
}
