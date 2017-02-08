using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace Contextually.Tests
{
    [TestClass]
    [TestCategory("NoUsingBlock")]
    public class NoUsingBlockTests
    {
        [TestMethod]
        public void AccessingInfoWithoutABlockReturnsEmptyCollection()
        {
            // ARRANGE

            // ACT
            var info = Relevant.Info();

            // ASSERT
            Assert.IsNotNull(info, "should have a value");
            Assert.IsInstanceOfType(info, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(0, info.Count, "should have no pairs");
        }

        [TestMethod]
        public void AccessingInfoWithinASingleBlockReturnsExpectedValues()
        {
            // ARRANGE
            var expectedValues = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            NameValueCollection actualValues;

            // ACT
            var infoBlock = Relevant.Info(expectedValues);

            actualValues = Relevant.Info();

            infoBlock.Dispose();

            // ASSERT
            Assert.IsNotNull(actualValues, "should have a value");
            Assert.IsInstanceOfType(actualValues, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValues.Count, actualValues.Count, $"should have {expectedValues.Count} pair");
            Assert.AreEqual(expectedValues["Key1"], actualValues["Key1"], "should have same value");
        }

        [TestMethod]
        public void AccessingInfoAfterASingleBlockReturnsEmptyCollection()
        {
            // ARRANGE
            NameValueCollection actual;

            // ACT
            var infoBlock = Relevant.Info(new NameValueCollection { ["Key1"] = "Value1" });

            var unusedValues = Relevant.Info();

            infoBlock.Dispose();

            actual = Relevant.Info();

            // ASSERT
            Assert.IsNotNull(actual, "should have a value");
            Assert.IsInstanceOfType(actual, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(0, actual.Count, "should have no pairs");
        }

        [TestMethod]
        public void AccessingInfoWithinNestedBlocksReturnsExpectedValues()
        {
            // ARRANGE
            var expectedValuesLevelOne = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            var expectedValuesLevelTwo = new NameValueCollection
            {
                ["Key1"] = "Value1",
                ["Key2"] = "Value2"
            };

            var valuesLevelTwo = new NameValueCollection
            {
                ["Key2"] = "Value2"
            };

            NameValueCollection actualValuesLevelOne;
            NameValueCollection actualValuesLevelTwo;

            // ACT
            var levelOne = Relevant.Info(expectedValuesLevelOne);

            actualValuesLevelOne = Relevant.Info();

            var levelTwo = Relevant.Info(valuesLevelTwo);

            actualValuesLevelTwo = Relevant.Info();

            levelTwo.Dispose();
            levelOne.Dispose();


            // ASSERT
            Assert.IsNotNull(actualValuesLevelOne, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelOne, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesLevelOne.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesLevelOne["Key1"], "should have same value");

            Assert.IsNotNull(actualValuesLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelTwo.Count, actualValuesLevelTwo.Count, $"should have {expectedValuesLevelTwo.Count} pair");
            Assert.AreEqual(expectedValuesLevelTwo["Key1"], actualValuesLevelTwo["Key1"], "should have same value");
            Assert.AreEqual(expectedValuesLevelTwo["Key2"], actualValuesLevelTwo["Key2"], "should have same value");
        }

        [TestMethod]
        public void AccessingInfoAfterANestedBlockReturnsExpectedValues()
        {
            // ARRANGE
            var expectedValuesLevelOne = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            var expectedValuesLevelTwo = new NameValueCollection
            {
                ["Key1"] = "Value1",
                ["Key2"] = "Value2"
            };

            var valuesLevelTwo = new NameValueCollection
            {
                ["Key2"] = "Value2"
            };

            NameValueCollection actualValuesLevelOne;
            NameValueCollection actualValuesLevelTwo;

            NameValueCollection actualValuesAfterLevelTwo;

            // ACT
            var levelOne = Relevant.Info(expectedValuesLevelOne);

            actualValuesLevelOne = Relevant.Info();

            var levelTwo = Relevant.Info(valuesLevelTwo);

            actualValuesLevelTwo = Relevant.Info();

            levelTwo.Dispose();

            actualValuesAfterLevelTwo = Relevant.Info();

            levelOne.Dispose();

            // ASSERT
            Assert.IsNotNull(actualValuesAfterLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesAfterLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesAfterLevelTwo.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesAfterLevelTwo["Key1"], "should have same value");
        }

        [TestMethod]
        public void DisposingOutOfOrderThrows()
        {
            // ARRANGE
            var expectedValuesLevelOne = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            var expectedValuesLevelTwo = new NameValueCollection
            {
                ["Key1"] = "Value1",
                ["Key2"] = "Value2"
            };

            var valuesLevelTwo = new NameValueCollection
            {
                ["Key2"] = "Value2"
            };

            NameValueCollection actualValuesLevelOne;
            NameValueCollection actualValuesLevelTwo;

            NameValueCollection actualValuesAfterLevelTwo;
            Action clean = () => { };

            // ACT
            Action act = () =>
            {
                var levelOne = Relevant.Info(expectedValuesLevelOne);

                actualValuesLevelOne = Relevant.Info();

                var levelTwo = Relevant.Info(valuesLevelTwo);

                actualValuesLevelTwo = Relevant.Info();

                clean = () =>
                {
                    // Proper disposal order. For use after we see exception, so we don't pollute other tests.
                    levelTwo.Dispose();
                    levelOne.Dispose();
                };

                // Out of order disposal.
                levelOne.Dispose();

                actualValuesAfterLevelTwo = Relevant.Info();

                levelTwo.Dispose();
            };

            // ASSERT
            Assert.ThrowsException<OutOfOrderInfoBlockDisposalException>(act);

            // CLEAN
            clean();
        }

        [TestMethod]
        public void MultipleDisposalThrows()
        {
            // ARRANGE
            var expectedValuesLevelOne = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            var expectedValuesLevelTwo = new NameValueCollection
            {
                ["Key1"] = "Value1",
                ["Key2"] = "Value2"
            };

            var valuesLevelTwo = new NameValueCollection
            {
                ["Key2"] = "Value2"
            };

            NameValueCollection actualValuesLevelOne;
            NameValueCollection actualValuesLevelTwo;

            NameValueCollection actualValuesAfterLevelTwo = null;
            Action clean = () => { };

            // ACT
            Action act = () =>
            {
                var levelOne = Relevant.Info(expectedValuesLevelOne);

                actualValuesLevelOne = Relevant.Info();

                var levelTwo = Relevant.Info(valuesLevelTwo);

                actualValuesLevelTwo = Relevant.Info();

                clean = () =>
                {
                    // Proper REMAINING disposal order. For use if we see an exception, so we don't pollute other tests.
                    levelOne.Dispose();
                };

                levelTwo.Dispose();

                // Duplicate disposal
                levelTwo.Dispose();

                actualValuesAfterLevelTwo = Relevant.Info();

                levelOne.Dispose();
            };

            // ASSERT
            Assert.ThrowsException<ObjectDisposedException>(act);

            // CLEAN
            clean();
        }
    }
}
