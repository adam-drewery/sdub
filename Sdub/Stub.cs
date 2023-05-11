using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sdub;

public abstract class Stub
{
    BindingFlags bindingFlags = BindingFlags.Instance 
        | BindingFlags.FlattenHierarchy 
        | BindingFlags.NonPublic 
        | BindingFlags.Public;
    
    internal ConcurrentBag<(MemberInfo, object[])> Calls { get; } = new();

    internal IDictionary<MemberInfo, Func<object[], object>> ReturnValues { get; } = new ConcurrentDictionary<MemberInfo, Func<object[], object>>();
    
    public static StubMember<TResult> Setup<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = ExpressionEvaluator.Evaluate(methodCall.Object);

        return new StubMember<TResult>((Stub)target, methodInfo);
    }

    public static AsyncStubMember<TResult> Setup<TResult>(Expression<Func<Task<TResult>>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = ExpressionEvaluator.Evaluate(methodCall.Object);

        return new AsyncStubMember<TResult>((Stub)target, methodInfo);
    }

    protected dynamic Invoke([CallerMemberName] string memberName = "")
    {
        var methodInfo = GetType().GetMethod(memberName, bindingFlags, Array.Empty<Type>());
        
        Calls.Add((methodInfo, Array.Empty<object>()));
        var func = ReturnValues.TryGetValue(methodInfo, out var value) ? value : _ => default;
        return func(Array.Empty	<object>());
    }
        
    protected dynamic Invoke(object[] @params, [CallerMemberName] string memberName = "")
    {
        var paramTypes = @params.Select(x => x.GetType()).ToArray();
        var methodInfo = GetType().GetMethod(memberName, bindingFlags, paramTypes);
        
        Calls.Add((methodInfo, @params));
        var func =  ReturnValues.TryGetValue(methodInfo, out var value) ? value : _ => default;
        return func(@params);
    }
    
    public static IEnumerable<object[]> CallsTo<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = (Stub)ExpressionEvaluator.Evaluate(methodCall.Object);

        return target.Calls
            .Where(x => x.Item1 == methodInfo)
            .Select(x => x.Item2);
    }
}