#if NET45
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Contextually.Reflection
{
    internal static class ThreadExtensions
    {
        private static readonly Func<Thread, ExecutionContext> _getMutableExecutionContextFunc;

        static ThreadExtensions()
        {
            var getMutableExecutionContextMethodInfo = typeof(Thread).GetMethod("GetMutableExecutionContext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var instanceParameterExpression = Expression.Parameter(typeof(Thread));
            var functionCallExpression = Expression.Call(instanceParameterExpression, getMutableExecutionContextMethodInfo);
            var lambdaExpression = Expression.Lambda<Func<Thread, ExecutionContext>>(functionCallExpression, instanceParameterExpression);
            _getMutableExecutionContextFunc = lambdaExpression.Compile();
        }

        private static MethodInfo GetMutableExecutionContextMethodInfo =
            typeof(Thread).GetMethod("GetMutableExecutionContext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public static ExecutionContext GetMutableExecutionContext(this Thread thread)
            => _getMutableExecutionContextFunc(thread);
    }
}
#endif