#if !NETSTANDARD1_3
using System;
using System.Runtime.ExceptionServices;

namespace Contextually
{
    partial class Relevant
    {
        /// <summary>
        /// Automatically adds the <see cref="Relevant.Info()"/> to any raised exception.
        /// You can get that info back with <see cref="ExceptionExtensions.GetReleventInfo(Exception)"/>.
        /// </summary>
        public static void InfoAutoAddedToExceptions()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
                args.Exception.AttachRelevantInfo();
        }
    }
}
#endif