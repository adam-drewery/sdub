using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sdub;

public abstract class Stub
{
    internal ConcurrentBag<(string, object[])> Calls { get; } = new();

    internal IDictionary<string, object> ReturnValues { get; } = new ConcurrentDictionary<string, object>();
    
    public static StubMember<TResult> Setup<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = ExpressionEvaluator.Evaluate(methodCall.Object);
        var methodName = methodInfo.Name;

        return new StubMember<TResult>((Stub)target, methodName);
    }

    protected dynamic Invoke([CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, Array.Empty<object>()));
        return ReturnValues.TryGetValue(memberName, out var value) ? value : default(dynamic);
    }
    
    protected dynamic Invoke(object param1, [CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, new [] { param1 }));
        return ReturnValues.TryGetValue(memberName, out var value) ? value : default(dynamic);
    }
    
    protected dynamic Invoke(object param1, object param2, [CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, new [] { param1, param2 }));
        return ReturnValues.TryGetValue(memberName, out var value) ? value : default(dynamic);
    }
    
    protected dynamic Invoke(object param1, object param2, object param3, [CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, new [] { param1, param2, param3 }));
        return ReturnValues.TryGetValue(memberName, out var value) ? value : default(dynamic);
    }
    
    protected dynamic Invoke(object[] @params, [CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, @params));
        return ReturnValues.TryGetValue(memberName, out var value) ? value : default(dynamic);
    }
    
    public static IList<object[]> CallsTo<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = (Stub)ExpressionEvaluator.Evaluate(methodCall.Object);
        var methodName = methodInfo.Name;

        return target.Calls
            .Where(x => x.Item1 == methodName)
            .Select(x => x.Item2)
            .ToList();
    }
}