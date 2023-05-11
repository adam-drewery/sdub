using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sdub;

public abstract class Stub<T>
{
    public IDictionary<string, object[]> Calls { get; } = new ConcurrentDictionary<string, object[]>();

    public IDictionary<string, object> ReturnValues { get; } = new ConcurrentDictionary<string, object>();
    
    public StubMember<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
    {
        var methodInfo = ((MethodCallExpression)expression.Body).Method;
        var target = ((MethodCallExpression)expression.Body).Object;
        var methodName = methodInfo.Name;

        return new StubMember<T, TResult>(this, methodName);
    }

    protected dynamic Invoke([CallerMemberName] string memberName = "")
    {
        return ReturnValues.TryGetValue(memberName, out var value) ? value : default(dynamic);
    }
}