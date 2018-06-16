using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contextually.Tests
{
    [TestClass]
    [TestCategory("Asynchronyous")]
    public class ExceptionTests
    {
        [TestMethod]
        public async Task ExceptionConveysRelevantInfo()
        {
            // ARRANGE
            var exception = new Exception();

            // ACT
            using (Relevant.Info("key1", "value1"))
            {
                exception.AttachRelevantInfo();
            }

            var actualRelevantInfo = exception.GetReleventInfo();

            // ASSERT
            Assert.IsNotNull(actualRelevantInfo, "should return a value");
            Assert.AreEqual(1, actualRelevantInfo.Count, $"should have 1 pair");
            Assert.AreEqual("value1", actualRelevantInfo["key1"], "value must match");
        }

        [TestMethod]
        public async Task InnerExceptionConveysRelevantInfo()
        {
            // ARRANGE
            var innerException = new Exception("inner error");
            var topException = new Exception("error", innerException);

            // ACT
            using (Relevant.Info("key1", "value1"))
            {
                innerException.AttachRelevantInfo();
            }

            using (Relevant.Info("key2", "value2"))
            {
                topException.AttachRelevantInfo();
            }

            var actualRelevantInfo = topException.GetReleventInfo();

            // ASSERT
            Assert.IsNotNull(actualRelevantInfo, "should return a value");
            Assert.AreEqual(2, actualRelevantInfo.Count, $"should have 2 pairs");
            Assert.AreEqual("value1", actualRelevantInfo["key1"], "value 1 must match");
            Assert.AreEqual("value2", actualRelevantInfo["key2"], "value 2 must match");
        }

        [TestMethod]
        public async Task TopExceptionOverridesRelevantInfo()
        {
            // ARRANGE
            var innerException = new Exception("inner error");
            var topException = new Exception("error", innerException);

            // ACT
            using (Relevant.Info("key", "value to be overriden"))
            {
                innerException.AttachRelevantInfo();
            }

            using (Relevant.Info("key", "value"))
            {
                topException.AttachRelevantInfo();
            }

            var actualRelevantInfo = topException.GetReleventInfo();

            // ASSERT
            Assert.IsNotNull(actualRelevantInfo, "should return a value");
            Assert.AreEqual(1, actualRelevantInfo.Count, $"should have 1 pair");
            Assert.AreEqual("value", actualRelevantInfo["key"], "value must match");
        }

        [TestMethod]
        public async Task AggregateRelevantInfo()
        {
            // ARRANGE
            var innerException1 = new Exception("inner error 1");
            var innerException2 = new Exception("inner error 2");
            var topException = new AggregateException(innerException1, innerException2);

            // ACT
            using (Relevant.Info("key1", "value1"))
            {
                innerException1.AttachRelevantInfo();
            }

            using (Relevant.Info("key2", "value2"))
            {
                innerException2.AttachRelevantInfo();
            }

            using (Relevant.Info("key3", "value3"))
            {
                topException.AttachRelevantInfo();
            }

            var actualRelevantInfo = topException.GetReleventInfo();

            // ASSERT
            Assert.IsNotNull(actualRelevantInfo, "should return a value");
            Assert.AreEqual(3, actualRelevantInfo.Count, $"should have 3 pairs");
            Assert.AreEqual("value1", actualRelevantInfo["key1"], "value 1 must match");
            Assert.AreEqual("value2", actualRelevantInfo["key2"], "value 2 must match");
            Assert.AreEqual("value3", actualRelevantInfo["key3"], "value 3 must match");
        }

        [TestMethod]
        public async Task InfoAutoAddedToExceptions()
        {
            // ARRANGE
            Relevant.InfoAutoAddedToExceptions();

            // ACT
            Exception caughtException;
            using (Relevant.Info("key1", "value1"))
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
            }

            var actualRelevantInfo = caughtException.GetReleventInfo();

            // ASSERT
            Assert.IsNotNull(actualRelevantInfo, "should return a value");
            Assert.AreEqual(1, actualRelevantInfo.Count, $"should have 1 pair");
            Assert.AreEqual("value1", actualRelevantInfo["key1"], "value must match");
        }
    }
}
