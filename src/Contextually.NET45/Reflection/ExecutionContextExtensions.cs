using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Contextually.Reflection
{
    internal static class ExecutionContextExtensions
    {
        private static readonly Func<ExecutionContext, LogicalCallContext> _getLogicalCallContextFunc;

        static ExecutionContextExtensions()
        {
            var logicalCallContextPropertyInfo = typeof(ExecutionContext).GetProperty("LogicalCallContext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var instanceParameterExpression = Expression.Parameter(typeof(ExecutionContext));
            var memberAccessExpression = Expression.MakeMemberAccess(instanceParameterExpression, logicalCallContextPropertyInfo);
            var lambdaExpression = Expression.Lambda<Func<ExecutionContext, LogicalCallContext>>(memberAccessExpression, instanceParameterExpression);
            _getLogicalCallContextFunc = lambdaExpression.Compile();
        }

        public static LogicalCallContext GetLogicalCallContext(this ExecutionContext executionContext)
            => _getLogicalCallContextFunc(executionContext);
    }
}
