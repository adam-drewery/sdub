namespace Sdub;

public class StubMember<TStub, TMember>
{
    private readonly Stub<TStub> _stub;
    private readonly string _memberName;

    public StubMember(Stub<TStub> stub, string memberName)
    {
        _stub = stub;
        _memberName = memberName;
    }
    
    public void Returns(TMember value)
    {
        _stub.ReturnValues[_memberName] = value;
    }
}