using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace Contextually.Tests
{
    [TestClass]
    [TestCategory("Synchronyous")]
    public class SynchronyousTests
    {
        [TestMethod]
        public void AccessingNamedInfoWithoutABlockReturnsEmptyCollection()
        {
            // ARRANGE

            // ACT
            var info = Relevant.Info("test");

            // ASSERT
            Assert.IsNotNull(info, "should have a value");
            Assert.IsInstanceOfType(info, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(0, info.Count, "should have no pairs");
        }

        [TestMethod]
        public void AccessingNamedInfoWithinASingleBlockReturnsExpectedValues()
        {
            // ARRANGE
            var expectedValues = new NameValueCollection
            {
                ["Key1"] = "Value1"
            };

            NameValueCollection actualValues;

            // ACT
            using (Relevant.Info(expectedValues, "test"))
            {
                actualValues = Relevant.Info("test");
            }

            // ASSERT
            Assert.IsNotNull(actualValues, "should have a value");
            Assert.IsInstanceOfType(actualValues, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValues.Count, actualValues.Count, $"should have {expectedValues.Count} pair");
            Assert.AreEqual(expectedValues["Key1"], actualValues["Key1"], "should have same value");
        }

                [TestMethod]
        public void AccessingNamedInfoWithinNestedBlocksReturnsExpectedValues()
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
            using (Relevant.Info(expectedValuesLevelOne, "test"))
            {
                actualValuesLevelOne = Relevant.Info("test");

                using (Relevant.Info(valuesLevelTwo, "test"))
                {
                    actualValuesLevelTwo = Relevant.Info("test");
                }
            }

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

        public void AccessingNamedInfoWithinDifferentNestedBlocksReturnsExpectedValues()
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
            NameValueCollection actualValuesLevelOneInner;

            // ACT
            using (Relevant.Info(expectedValuesLevelOne))
            {
                actualValuesLevelOne = Relevant.Info();

                using (Relevant.Info(valuesLevelTwo, "test"))
                {
                    actualValuesLevelTwo = Relevant.Info("test");
                    actualValuesLevelOneInner = Relevant.Info();
                }
            }

            // ASSERT
            Assert.IsNotNull(actualValuesLevelOne, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelOne, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesLevelOne.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesLevelOne["Key1"], "should have same value");

            Assert.IsNotNull(actualValuesLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(valuesLevelTwo.Count, actualValuesLevelTwo.Count, $"should have {valuesLevelTwo.Count} pair");
            Assert.AreEqual(valuesLevelTwo["Key2"], actualValuesLevelTwo["Key2"], "should have same value");

            Assert.IsNotNull(actualValuesLevelOne, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelOneInner, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesLevelOneInner.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesLevelOneInner["Key1"], "should have same value");
        }

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
            using (Relevant.Info(expectedValues))
            {
                actualValues = Relevant.Info();
            }

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
            using (Relevant.Info(new NameValueCollection { ["Key1"] = "Value1" }))
            {
                var unusedValues = Relevant.Info();
            }

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
            using (Relevant.Info(expectedValuesLevelOne))
            {
                actualValuesLevelOne = Relevant.Info();

                using (Relevant.Info(valuesLevelTwo))
                {
                    actualValuesLevelTwo = Relevant.Info();
                }
            }

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
            using (Relevant.Info(expectedValuesLevelOne))
            {
                actualValuesLevelOne = Relevant.Info();

                using (Relevant.Info(valuesLevelTwo))
                {
                    actualValuesLevelTwo = Relevant.Info();
                }

                actualValuesAfterLevelTwo = Relevant.Info();
            }

            // ASSERT
            Assert.IsNotNull(actualValuesAfterLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesAfterLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesAfterLevelTwo.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesAfterLevelTwo["Key1"], "should have same value");
        }

        //[TestMethod]
        //public void AccessingInfoAfterANestedBlockReturnsExpectedValuesForKey()
        //{
        //    // ARRANGE
        //    var expectedValuesLevelOne = new NameValueCollection
        //    {
        //        ["Key1"] = "Value1"
        //    };

        //    var expectedValuesLevelTwo = new NameValueCollection
        //    {
        //        ["Key1"] = "Value1",
        //        ["Key1"] = "Value2"
        //    };

        //    var valuesLevelTwo = new NameValueCollection
        //    {
        //        ["Key1"] = "Value2"
        //    };

        //    NameValueCollection actualValuesLevelOne;
        //    NameValueCollection actualValuesLevelTwo;

        //    NameValueCollection actualValuesAfterLevelTwo;

        //    // ACT
        //    using (Relevant.Info(expectedValuesLevelOne))
        //    {
        //        actualValuesLevelOne = Relevant.Info();

        //        using (Relevant.Info(valuesLevelTwo))
        //        {
        //            actualValuesLevelTwo = Relevant.Info();
        //        }

        //        actualValuesAfterLevelTwo = Relevant.Info();
        //    }

        //    // ASSERT
        //    Assert.IsNotNull(actualValuesAfterLevelTwo, "should have a value");
        //    Assert.IsInstanceOfType(actualValuesAfterLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
        //    Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesAfterLevelTwo.Count, $"should have {expectedValuesLevelOne.Count} pair");
        //    Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesAfterLevelTwo["Key1"], "should have same value");
        //}
    }
}
