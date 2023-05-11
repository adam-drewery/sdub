using System.Reflection;

namespace Sdub;

public class StubMember<TMember>
{
    private readonly Stub _stub;
    private readonly MemberInfo _memberInfo;

    public StubMember(Stub stub, MemberInfo memberInfo)
    {
        _stub = stub;
        _memberInfo = memberInfo;
    }
    
    public void Returns(TMember value)
    {
        _stub.ReturnValues[_memberInfo] = _ => value;
    }
    
    public void Returns(Func<object[], object> func)
    {
        _stub.ReturnValues[_memberInfo] = func;
    }
}