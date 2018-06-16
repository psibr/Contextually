using System;
using System.Collections.Specialized;

namespace Contextually
{
    /// <summary>
    /// Extension methods for the <see cref="Exception"/> type.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Copies the <see cref="Relevant.Info()"/> to the <see cref="Exception.Data"/>
        /// and returns the same instance of an exception.
        /// You can retrieve it back with <see cref="GetReleventInfo(Exception)"/>.
        /// </summary>
        /// <typeparam name="T">The type of an exception.</typeparam>
        /// <param name="exception">The instance of an exception to attach the relevant info to.</param>
        /// <param name="containerName">Optional name of the container to read from.</param>
        /// <returns>Simply returns the <paramref name="exception"/>.</returns>
        public static T WithRelevantInfo<T>(this T exception, string containerName = null)
            where T : Exception
        {
            var info = Relevant.Info(containerName);
            foreach (string name in info.Keys)
                exception.Data[name] = info[name];
            return exception;
        }

        /// <summary>
        /// Copies the <see cref="Relevant.Info()"/> to the <see cref="Exception.Data"/>.
        /// You can retrieve it back with <see cref="GetReleventInfo(Exception)"/>.
        /// </summary>
        /// <param name="exception">The instance of an exception to attach the relevant info to.</param>
        /// <param name="containerName">Optional name of the container to read from.</param>
        public static void AttachRelevantInfo(this Exception exception, string containerName = null)
            => exception.WithRelevantInfo(containerName);

        /// <summary>
        /// Collects revelant info previously added with <see cref="AttachRelevantInfo(Exception, string)"/>
        /// or <see cref="WithRelevantInfo{T}(T, string)"/>, including inner exceptions.
        /// </summary>
        /// <param name="exception">The exception root to get relevant info from.</param>
        public static NameValueCollection GetReleventInfo(this Exception exception)
        {
            var relevantInfo = new NameValueCollection();
            PopulateReleventInfo(exception, relevantInfo);
            return relevantInfo;

            void PopulateReleventInfo(Exception ex, NameValueCollection info)
            {
                foreach (var key in ex.Data.Keys)
                {
                    // Add name-value pair to the info collection only if:
                    // 1. Both name and value are strings
                    // 2. the info collection does not contain the given name already -
                    //    assume that wraping exception overrides values of inner exceptions.
                    if (key is string name && ex.Data[key] is string value && info[name] == null)
                    {
                        info.Add(name, value);
                    }
                }

                // Gather relevant info recursively.
                if (ex is AggregateException aggregateException)
                {
                    foreach (var innerException in aggregateException.InnerExceptions)
                    {
                        PopulateReleventInfo(innerException, info);
                    }
                }
                else if (ex.InnerException != null)
                {
                    PopulateReleventInfo(ex.InnerException, info);
                }
            }
        }
    }
}
