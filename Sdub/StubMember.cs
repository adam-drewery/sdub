using System.Reflection;

namespace Sdub;

public class StubMember<TMember>
{
    private readonly Stub _stub;
    private readonly MemberInfo _memberName;

    public StubMember(Stub stub, MemberInfo memberName)
    {
        _stub = stub;
        _memberName = memberName;
    }
    
    public void Returns(TMember value)
    {
        _stub.ReturnValues[_memberName] = _ => value;
    }
    
    public void Returns(Func<object[], object> func)
    {
        _stub.ReturnValues[_memberName] = func;
    }
}