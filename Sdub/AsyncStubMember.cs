using System.Reflection;

namespace Sdub;

public class AsyncStubMember<TMember>
{
    private readonly Stub _stub;
    private readonly MemberInfo _memberInfo;

    public AsyncStubMember(Stub stub, MemberInfo memberInfo)
    {
        _stub = stub;
        _memberInfo = memberInfo;
    }
    
    public void Returns(TMember value)
    {
        _stub.ReturnValues[_memberInfo] = _ => Task.FromResult(value);
    }
    
    public void Returns(Func<object[], Task<TMember>> func)
    {
        _stub.ReturnValues[_memberInfo] = func;
    }
}