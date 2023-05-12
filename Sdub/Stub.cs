using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sdub;

public abstract class Stub
{
    private const BindingFlags DefaultBindingFlags = BindingFlags.Instance 
        | BindingFlags.FlattenHierarchy 
        | BindingFlags.NonPublic 
        | BindingFlags.Public;

    private readonly ConcurrentBag<(MemberInfo, object[])> _calls = new();

    internal IDictionary<MemberInfo, Func<object[], object>> ReturnValues { get; } =
        new ConcurrentDictionary<MemberInfo, Func<object[], object>>();

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
        var methodInfo = GetType().GetMethod(memberName, DefaultBindingFlags, Array.Empty<Type>());
        if (methodInfo == null) throw new MissingMethodException(memberName);

        _calls.Add((methodInfo, Array.Empty<object>()));
        var func = ReturnValues.TryGetValue(methodInfo, out var value) ? value : _ => GetDefaultReturnValue(methodInfo);
        return func(Array.Empty<object>());
    }

    protected dynamic Invoke(object[] @params, [CallerMemberName] string memberName = "")
    {
        var paramTypes = @params.Select(x => x.GetType()).ToArray();
        var methodInfo = GetType().GetMethod(memberName, DefaultBindingFlags, paramTypes);
        if (methodInfo == null) throw new MissingMethodException(memberName);

        _calls.Add((methodInfo, @params));
        var func = ReturnValues.TryGetValue(methodInfo, out var value) ? value : _ => GetDefaultReturnValue(methodInfo);
        return func(@params);
    }

    private static object GetDefaultReturnValue(MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;

        if (returnType == typeof(Task))
        {
            return Task.CompletedTask;
        }
        else if (returnType.IsAssignableTo(typeof(Task)) && returnType.IsGenericType)
        {
            var genericType = returnType.GetGenericArguments()[0];
            var defaultValue = GetDefault(genericType);
            var fromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(genericType);
            return fromResultMethod.Invoke(null, new[] { defaultValue });
        }
        else
        {
            return GetDefault(returnType);
        }
    }
    
    public static object GetDefault(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    public static IEnumerable<object[]> CallsTo<TResult>(Expression<Func<TResult>> expression)
    {
        var methodCall = (MethodCallExpression)expression.Body;
        var methodInfo = methodCall.Method;
        var target = (Stub)ExpressionEvaluator.Evaluate(methodCall.Object);

        return target._calls
            .Where(x => x.Item1 == methodInfo)
            .Select(x => x.Item2);
    }
}