using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Contextually.Tests
{
    [TestClass]
    [TestCategory("SynchronyousTask")]
    public class SynchronyousTaskTests
    {
        private static Task<NameValueCollection> TaskWithInfo()
        {
            return Task.FromResult(Relevant.Info());
        }

        private static async Task<NameValueCollection> AwaitedTaskWithInfo()
        {
            return await TaskWithInfo();
        }

        [TestMethod]
        public void AwaitingACallWithinASingleBlockReturnsExpectedValues()
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
                actualValues = TaskWithInfo().Result;
            }

            // ASSERT
            Assert.IsNotNull(actualValues, "should have a value");
            Assert.IsInstanceOfType(actualValues, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValues.Count, actualValues.Count, $"should have {expectedValues.Count} pair");
            Assert.AreEqual(expectedValues["Key1"], actualValues["Key1"], "should have same value");
        }

        [TestMethod]
        public void AwaitingNestedCallsWithinASingleBlockReturnsExpectedValues()
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
                actualValues = AwaitedTaskWithInfo().Result;
            }

            // ASSERT
            Assert.IsNotNull(actualValues, "should have a value");
            Assert.IsInstanceOfType(actualValues, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValues.Count, actualValues.Count, $"should have {expectedValues.Count} pair");
            Assert.AreEqual(expectedValues["Key1"], actualValues["Key1"], "should have same value");
        }

        [TestMethod]
        public void AwaitingACallAfterASingleBlockReturnsEmptyCollection()
        {
            // ARRANGE
            NameValueCollection actual;

            // ACT
            using (Relevant.Info(new NameValueCollection { ["Key1"] = "Value1" }))
            {
                var unusedValues = TaskWithInfo().Result;
            }

            actual = TaskWithInfo().Result;

            // ASSERT
            Assert.IsNotNull(actual, "should have a value");
            Assert.IsInstanceOfType(actual, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(0, actual.Count, "should have no pairs");
        }

        [TestMethod]
        public void AwaitingNestedCallsAfterASingleBlockReturnsEmptyCollection()
        {
            // ARRANGE
            NameValueCollection actual;

            // ACT
            using (Relevant.Info(new NameValueCollection { ["Key1"] = "Value1" }))
            {
                var unusedValues = AwaitedTaskWithInfo().Result;
            }

            actual = AwaitedTaskWithInfo().Result;

            // ASSERT
            Assert.IsNotNull(actual, "should have a value");
            Assert.IsInstanceOfType(actual, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(0, actual.Count, "should have no pairs");
        }

        [TestMethod]
        public void AwaitingACallWithinANestedBlockReturnsExpectedValues()
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
                actualValuesLevelOne = TaskWithInfo().Result;

                using (Relevant.Info(valuesLevelTwo))
                {
                    actualValuesLevelTwo = TaskWithInfo().Result;
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
        public void AwaitingNestedCallsWithinANestedBlockReturnsExpectedValues()
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
                actualValuesLevelOne = AwaitedTaskWithInfo().Result;

                using (Relevant.Info(valuesLevelTwo))
                {
                    actualValuesLevelTwo = AwaitedTaskWithInfo().Result;
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
        public void AwaitingACallAfterANestedBlockReturnsExpectedValues()
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
                actualValuesLevelOne = TaskWithInfo().Result;

                using (Relevant.Info(valuesLevelTwo))
                {
                    actualValuesLevelTwo = TaskWithInfo().Result;
                }

                actualValuesAfterLevelTwo = TaskWithInfo().Result;
            }

            // ASSERT
            Assert.IsNotNull(actualValuesAfterLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesAfterLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesAfterLevelTwo.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesAfterLevelTwo["Key1"], "should have same value");
        }

        [TestMethod]
        public void AwaitingNestedCallsAfterANestedBlockReturnsExpectedValues()
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
                actualValuesLevelOne = AwaitedTaskWithInfo().Result;

                using (Relevant.Info(valuesLevelTwo))
                {
                    actualValuesLevelTwo = AwaitedTaskWithInfo().Result;
                }

                actualValuesAfterLevelTwo = AwaitedTaskWithInfo().Result;
            }

            // ASSERT
            Assert.IsNotNull(actualValuesAfterLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesAfterLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesAfterLevelTwo.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreEqual(expectedValuesLevelOne["Key1"], actualValuesAfterLevelTwo["Key1"], "should have same value");
        }

        [TestMethod]
        public void AwaitingNestedCallsOutsideANestedBlockReturnsExpectedValues()
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

            Task<NameValueCollection> levelOneTask;
            Task<NameValueCollection> levelTwoTask;

            // ACT
            using (Relevant.Info(expectedValuesLevelOne))
            {
                levelOneTask = AwaitedTaskWithInfo();

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = AwaitedTaskWithInfo();
                }
            }

            actualValuesLevelOne = levelOneTask.Result;
            actualValuesLevelTwo = levelTwoTask.Result;

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
        public void AwaitingWhenAllNestedCallsOutsideANestedBlockReturnsExpectedValues()
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

            Task<NameValueCollection> levelOneTask;
            Task<NameValueCollection> levelTwoTask;

            // ACT
            using (Relevant.Info(expectedValuesLevelOne))
            {
                levelOneTask = AwaitedTaskWithInfo();

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = AwaitedTaskWithInfo();
                }
            }

            Task.WaitAll(levelOneTask, levelTwoTask);

            actualValuesLevelOne = levelOneTask.Result;
            actualValuesLevelTwo = levelTwoTask.Result;

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
        public void AwaitingNestedCallsInsideADifferentNestedBlockReturnsExpectedValues()
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

            Task<NameValueCollection> levelOneTask;
            Task<NameValueCollection> levelTwoTask;

            // ACT
            using (Relevant.Info(expectedValuesLevelOne))
            {
                levelOneTask = AwaitedTaskWithInfo();

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = AwaitedTaskWithInfo();
                }
            }

            using (Relevant.Info(new NameValueCollection { ["Key1"] = "IncorrectValue" }))
            {
                actualValuesLevelOne = levelOneTask.Result;
                actualValuesLevelTwo = levelTwoTask.Result;
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
        public void AwaitingACallInsideADifferentNestedBlockReturnsExpectedValues()
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

            Task<NameValueCollection> levelOneTask;
            Task<NameValueCollection> levelTwoTask;

            // ACT
            using (Relevant.Info(expectedValuesLevelOne))
            {
                levelOneTask = TaskWithInfo();

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = TaskWithInfo();
                }
            }

            using (Relevant.Info(new NameValueCollection { ["Key1"] = "IncorrectValue" }))
            {
                actualValuesLevelOne = levelOneTask.Result;
                actualValuesLevelTwo = levelTwoTask.Result;
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
        [TestCategory("Threading")]
        public void AwaitingACallInsideADifferentNestedBlockUsingAThreadReturnsExpectedValues()
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

            Task<NameValueCollection> levelOneTask = null;
            Task<NameValueCollection> levelTwoTask;
            Thread assignerThread;

            // ACT
            using (Relevant.Info(expectedValuesLevelOne))
            {
                assignerThread = new Thread(new ThreadStart(() => levelOneTask = TaskWithInfo()));

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = TaskWithInfo();
                }
            }

            using (Relevant.Info(new NameValueCollection { ["Key1"] = "IncorrectValue" }))
            {
                assignerThread.Start();
                
                assignerThread.Join();

                actualValuesLevelOne = levelOneTask.Result;
                actualValuesLevelTwo = levelTwoTask.Result;
            }

            // ASSERT
            Assert.IsNotNull(actualValuesLevelOne, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelOne, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelOne.Count, actualValuesLevelOne.Count, $"should have {expectedValuesLevelOne.Count} pair");
            Assert.AreNotEqual(expectedValuesLevelOne["Key1"], actualValuesLevelOne["Key1"], "should have same value");

            Assert.IsNotNull(actualValuesLevelTwo, "should have a value");
            Assert.IsInstanceOfType(actualValuesLevelTwo, typeof(NameValueCollection), "should be a NameValueCollection");
            Assert.AreEqual(expectedValuesLevelTwo.Count, actualValuesLevelTwo.Count, $"should have {expectedValuesLevelTwo.Count} pair");
            Assert.AreEqual(expectedValuesLevelTwo["Key1"], actualValuesLevelTwo["Key1"], "should have same value");
            Assert.AreEqual(expectedValuesLevelTwo["Key2"], actualValuesLevelTwo["Key2"], "should have same value");
        }
    }
}
