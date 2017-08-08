using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Contextually.Tests
{
    [TestClass]
    [TestCategory("AsynchronyousAndThreaded")]
    public class AsynchronyousAndThreadedTests
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
        [TestCategory("Threading")]
        public async Task AwaitingACallInsideADifferentNestedBlockUsingAThreadReturnsExpectedValues()
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

                actualValuesLevelOne = await levelOneTask;
                actualValuesLevelTwo = await levelTwoTask;
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

        [TestMethod]
        public async Task AwaitingACallInsideADifferentNestedBlockUsingAStartedThreadReturnsExpectedValues()
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
            Task<NameValueCollection> levelTwoTask = null;
            Thread assignerThread;

            // ACT

            using (Relevant.Info(expectedValuesLevelOne))
            {
                assignerThread = new Thread(new ThreadStart(() => levelOneTask = TaskWithInfo()));

                assignerThread.Start();

                assignerThread.Join();

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = TaskWithInfo();
                }
            }            

            using (Relevant.Info(new NameValueCollection { ["Key1"] = "IncorrectValue" }))
            {
                actualValuesLevelOne = await levelOneTask;
                actualValuesLevelTwo = await levelTwoTask;
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
        public async Task AwaitingACallAndJoiningInsideADifferentNestedBlockUsingAStartedThreadReturnsExpectedValues()
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
            Task<NameValueCollection> levelTwoTask = null;
            Thread assignerThread;

            // ACT

            using (Relevant.Info(expectedValuesLevelOne))
            {
                assignerThread = new Thread(() => levelOneTask = TaskWithInfo());

                assignerThread.Start();

                using (Relevant.Info(valuesLevelTwo))
                {
                    levelTwoTask = TaskWithInfo();
                }
            }

            using (Relevant.Info(new NameValueCollection { ["Key1"] = "IncorrectValue" }))
            {
                assignerThread.Join();

                actualValuesLevelOne = await levelOneTask;
                actualValuesLevelTwo = await levelTwoTask;
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
    }
}
