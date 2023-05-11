using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sdub;

public abstract class Stub
{
    internal ConcurrentBag<(string, object[])> Calls { get; } = new();

    internal IDictionary<string, Func<object[], object>> ReturnValues { get; } = new ConcurrentDictionary<string, Func<object[], object>>();
    
    public static StubMember<TResult> Setup<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = ExpressionEvaluator.Evaluate(methodCall.Object);
        var methodName = methodInfo.Name;

        return new StubMember<TResult>((Stub)target, methodName);
    }

    public static AsyncStubMember<TResult> Setup<TResult>(Expression<Func<Task<TResult>>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = ExpressionEvaluator.Evaluate(methodCall.Object);
        var methodName = methodInfo.Name;

        return new AsyncStubMember<TResult>((Stub)target, methodName);
    }

    protected dynamic Invoke([CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, Array.Empty<object>()));
        var func = ReturnValues.TryGetValue(memberName, out var value) ? value : _ => default;
        return func(Array.Empty	<object>());
    }
        
    protected dynamic Invoke(object[] @params, [CallerMemberName] string memberName = "")
    {
        Calls.Add((memberName, @params));
        var func =  ReturnValues.TryGetValue(memberName, out var value) ? value : _ => default;
        return func(@params);
    }
    
    public static IEnumerable<object[]> CallsTo<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = (Stub)ExpressionEvaluator.Evaluate(methodCall.Object);
        var methodName = methodInfo.Name;

        return target.Calls
            .Where(x => x.Item1 == methodName)
            .Select(x => x.Item2);
    }
}