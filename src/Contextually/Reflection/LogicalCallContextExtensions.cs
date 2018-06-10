#if NET45
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Contextually.Reflection
{
    internal static class LogicalCallContextExtensions
    {
        private static readonly Func<LogicalCallContext, Hashtable> _getDatastoreFunc;
        private static readonly Func<LogicalCallContext, Hashtable, bool> _setDatastoreFunc;

        static LogicalCallContextExtensions()
        {
            var datastoreFieldInfo = typeof(LogicalCallContext).GetField("m_Datastore", BindingFlags.Instance | BindingFlags.NonPublic);
            var instanceParameterExpression = Expression.Parameter(typeof(LogicalCallContext));
            var memberAccessExpression = Expression.MakeMemberAccess(instanceParameterExpression, datastoreFieldInfo);
            var getLambdaExpression = Expression.Lambda<Func<LogicalCallContext, Hashtable>>(memberAccessExpression, instanceParameterExpression);
            _getDatastoreFunc = getLambdaExpression.Compile();
            var valueParameterExpression = Expression.Parameter(typeof(Hashtable));
            var assignmentExpression = Expression.Assign(memberAccessExpression, valueParameterExpression);
            var setFunctionBody = Expression.Block(assignmentExpression, Expression.Constant(true));
            var setLambdaExpression = Expression.Lambda<Func<LogicalCallContext, Hashtable, bool>>(setFunctionBody, instanceParameterExpression, valueParameterExpression);
            _setDatastoreFunc = setLambdaExpression.Compile();
        }

        public static Hashtable GetDatastore(this LogicalCallContext context)
            => _getDatastoreFunc(context);

        public static void SetDatastore(this LogicalCallContext context, Hashtable datastore)
            => _setDatastoreFunc(context, datastore);
    }
}
#endif